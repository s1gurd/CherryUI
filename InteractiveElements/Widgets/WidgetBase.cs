using System;
using System.Collections.Generic;
using CherryUI.UiAnimation;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CherryUI.InteractiveElements.Widgets
{
    [DisallowMultipleComponent]
    public abstract class WidgetBase : InteractiveElementBase
    {
        [Title("Widget states and settings")]
        [SerializeField] private WidgetStartupBehaviour startupBehaviour;
        [SerializeField] private bool reInitOnEveryEnable;

        [InfoBox("First element will become current state")]
        [SerializeField]
        private List<WidgetState> widgetStates = new();

        [SerializeField] private bool forceDisableElementAfterHide;

        public int CurrentState => _currentState;
        public int StatesCount => widgetStates.Count;
        public bool Inited => _inited;
        public bool Playing => _playing;

        private int _currentState;
        private bool _inited;
        private bool _playing;
        private Sequence _currentSequence;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (Inited && reInitOnEveryEnable)
            {
                Init();
            }
        }

        protected virtual void Start()
        {
            if (!Inited)
            {
                Init();
            }
        }

        public delegate void OnStateChangedDelegate(string newStateName);
        public event OnStateChangedDelegate StateChanging;
        public event OnStateChangedDelegate StateChanged;

        public void SetState(int state)
        {
            if (!Inited)
            {
                Init();
            }

            if (state == _currentState) return;

            if (state >= widgetStates.Count || state < 0)
            {
                Debug.LogError($"[Widget] Not found requested state: {state}, aborting!");
                return;
            }

            _currentState = state;
            if (_playing)
            {
                _currentSequence.Kill(true);
            }
            else
            {
                _playing = true;
            }

            var seq = DOTween.Sequence();
            _currentSequence = seq;

            for (var i = 0; i < widgetStates.Count; i++)
            {
                seq.Insert(0,
                    i == _currentState
                        ? SetElementsInState(widgetStates[i], true)
                        : SetElementsInState(widgetStates[i], false));
            }

            StateChanging?.Invoke(widgetStates[state].stateName);

            seq.AppendCallback(
                () =>
                {
                    _playing = false;
                    StateChanged?.Invoke(widgetStates[state].stateName);
                });
        }

        public void SetState(string stateName)
        {
            if (!Inited)
            {
                Init();
            }

            var entries = widgetStates.FindAll(s => s.stateName.Equals(stateName, StringComparison.Ordinal));
            switch (entries.Count)
            {
                case 0:
                    Debug.LogError($"[Widget] Not found requested state: \'{stateName}\', aborting!");
                    return;
                case > 1:
                    Debug.LogError($"[Widget] Found {entries.Count} states with identical name: \'{stateName}\', aborting!");
                    return;
                default:
                    var index = widgetStates.IndexOf(entries[0]);
                    SetState(index);
                    break;
            }
        }

        public string GetStateName(int state)
        {
            if (state >= widgetStates.Count || state < 0)
            {
                Debug.LogError($"[Widget] Not found requested state: {state}, aborting!");
                return null;
            }

            return widgetStates[state].stateName;
        }

        public void ForceCompleteTransition() => _currentSequence?.Complete(true);

        private Sequence SetElementsInState(WidgetState state, bool show)
        {
            var seq = DOTween.Sequence();

            foreach (var element in state.stateElements)
            {
                if (show)
                {
                    element.gameObject.SetActive(true);
                    seq.Insert(0, element.Show());
                }
                else
                {
                    seq.Insert(0, element.Hide());
                    if (forceDisableElementAfterHide)
                    {
                        seq.AppendCallback(() => element.gameObject.SetActive(false));
                    }
                }
            }

            return seq;
        }

        protected virtual void Init()
        {
            _currentState = 0;
            _playing = true;
            var seq = DOTween.Sequence();
            _currentSequence = seq;

            if (startupBehaviour == WidgetStartupBehaviour.SequentiallyExecuteShowOnSelfAndCurrentState ||
                startupBehaviour == WidgetStartupBehaviour.SimultaneouslyExecuteShowOnSelfAndCurrentState)
                seq.Append(CreateSequence(animators, Purpose.Show));
            for (var i = 0; i < widgetStates.Count; i++)
            {
                var innerSeq = DOTween.Sequence();

                foreach (var element in widgetStates[i].stateElements)
                {
                    element.gameObject.SetActive(true);

                    if (i == _currentState)
                    {
                        innerSeq.Insert(0, element.Show());
                    }
                    else
                    {
                        var elementHide = element.Hide();
                        if (forceDisableElementAfterHide)
                        {
                            elementHide.AppendCallback(() => element.gameObject.SetActive(false));
                        }
                        elementHide.Complete(true);
                    }
                }
                switch (startupBehaviour)
                {
                    case WidgetStartupBehaviour.ExecuteShowOnCurrentState:
                    case WidgetStartupBehaviour.SimultaneouslyExecuteShowOnSelfAndCurrentState:
                        seq.Insert(0, innerSeq);
                        break;
                    case WidgetStartupBehaviour.SequentiallyExecuteShowOnSelfAndCurrentState:
                        seq.Append(innerSeq);
                        break;
                    case WidgetStartupBehaviour.JustSetCurrentState:
                        innerSeq.Complete(true);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                var stateIndex = i;
                foreach (var activationButton in widgetStates[i].activationButtons)
                {
                    activationButton.onClick.AddListener(() => SetState(stateIndex));
                }
            }

            seq.AppendCallback(() => _playing = false);
            _inited = true;
        }
    }
}