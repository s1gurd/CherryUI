using System;
using System.Collections.Generic;
using CherryUI.UiAnimation;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CherryUI.InteractiveElements
{
    public abstract partial class InteractiveElementBase
    {
        [Title("Animation Settings")] [SerializeField]
        protected List<UiAnimationSettings> animators;

        protected Sequence CreateSequence(List<UiAnimationSettings> anims, Purpose purpose)
        {
            var result = DOTween.Sequence();

            foreach (var anim in anims)
            {
                Func<float, Tween> action;
                switch (purpose)
                {
                    case Purpose.Show:
                        action = d => anim.animator.Show(d);
                        break;
                    case Purpose.Hide:
                        action = d => anim.animator.Hide(d);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                switch (anim.launchMode)
                {
                    case LaunchMode.AtGlobalAnimationStart:
                        result.Insert(0, action.Invoke(anim.delay));
                        break;
                    case LaunchMode.AfterPreviousAnimatorStart:
                        result.Join(action.Invoke(anim.delay));
                        break;
                    case LaunchMode.AfterPreviousAnimatorFinished:
                        result.Append(action.Invoke(anim.delay));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return result;
        }

        protected virtual void OnShowComplete()
        {
        }

        protected virtual void OnHideComplete()
        {
        }
    }
}