using System;
using System.Collections.Generic;
using System.Linq;
using CherryUI.DependencyManager;
using CherryUI.UiAnimation;
using CherryUI.Views;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CherryUI.InteractiveElements.Presenters
{
    [DisallowMultipleComponent]
    public abstract class PresenterBase : InteractiveElementBase
    {
        [Inject] protected readonly ViewService ViewService;

        [Title("Hierarchy settings")]
        [SerializeField] private Canvas childrenContainer;

        [InfoBox("First presenter will be default")]
        [SerializeField, DrawWithUnity] protected List<PresenterBase> childPresenters = new();

        public Canvas ChildrenContainer => childrenContainer;
        public List<PresenterBase> ChildPresenters => childPresenters;

        [HideInInspector] public List<PresenterBase> uiPath = new();
        [HideInInspector] public PresenterBase currentChild;

        protected PresenterBase UiRoot => null;
        protected PresenterBase UiParent => uiPath.Last();
        protected PresenterBase UiThis => this;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (this is not RootPresenter
                && childrenContainer
                && childrenContainer.GetComponentInParent<PresenterBase>() != this)
            {
                throw new Exception(
                    $"[Presenter - {this.gameObject.name}] Children container {childrenContainer.gameObject} must be a child of this Game Object!");
            }
        }

        public virtual void ShowFrom(PresenterBase previous, bool skipAnimation = false)
        {
            this.transform.SetAsLastSibling();
            var seq = CreateSequence(animators, Purpose.Show);
            seq.AppendCallback(OnShowComplete);
            if (skipAnimation) seq.Complete(true);
        }

        public virtual void HideTo(PresenterBase next, bool skipAnimation = false)
        {
            var seq = CreateSequence(animators, Purpose.Hide);
            seq.AppendCallback(() => next.transform.SetAsLastSibling());
            seq.AppendCallback(OnHideComplete);
            if (skipAnimation) seq.Complete(true);
        }
    }
}