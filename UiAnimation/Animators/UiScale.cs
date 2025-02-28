using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace CherryUI.UiAnimation.Animators
{
    public class UiScale : UiAnimationBase
    {
        [SerializeField] private UiAnimatorEndValueTypes type;
        [SerializeField] private Vector3 value;

        private List<(RectTransform rectTransform, Vector3 baseValue, Vector3 endValue)> _targetGroups = new();

        protected override void OnEnable()
        {
            if (!Inited)
            {
                foreach (var target in Targets)
                {
                    Vector3 startValue;
                    Vector3 endValue;

                    if (type == UiAnimatorEndValueTypes.To)
                    {
                        startValue = target.localScale;
                        endValue = value;
                    }
                    else
                    {
                        startValue = value;
                        endValue = target.localScale;
                    }

                    _targetGroups.Add((target, startValue, endValue));
                }

                MainSequence = DOTween.Sequence();

                Inited = true;
            }

            foreach (var group in _targetGroups)
            {
                group.rectTransform.localScale = group.Item2;
            }
        }

        public override Sequence Show(float delay = 0f)
        {
            MainSequence = MainSequence.ReCreate();

            Scale(delay, true);

            return MainSequence;
        }

        public override Sequence Hide(float delay = 0f)
        {
            MainSequence = MainSequence.ReCreate();

            Scale(delay, false);

            return MainSequence;
        }

        private void Scale(float delay, bool show)
        {
            foreach (var tuple in _targetGroups)
            {
                MainSequence.Insert(
                    0,
                    show
                        ? tuple.rectTransform.DOScale(tuple.endValue, duration).SetEase(showEasing)
                        : tuple.rectTransform.DOScale(tuple.baseValue, duration).SetEase(hideEasing));

            }

            if (delay > 0f)
            {
                MainSequence.PrependInterval(delay);
            }
        }
    }
}