using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CherryUI.UiAnimation.Animators
{
    public class UiSlide : UiAnimationBase
    {
        [InfoBox("Delta is counted as a ratio to target transform dimensions")]
        [SerializeField] private Vector2 positionDelta;
        private List<(RectTransform rectTransform, Vector3 baseValue, Vector3 endValue)> _targetGroups = new();

        protected override void OnEnable()
        {
            if (!Inited)
            {
                foreach (var target in Targets)
                {
                    var endPosition = target.localPosition;
                    var startPosition = endPosition + new Vector3(target.rect.width * positionDelta.x,
                        target.rect.height * positionDelta.y, 0f);
                    _targetGroups.Add((target, startPosition, endPosition));
                }

                MainSequence = DOTween.Sequence();

                Inited = true;
            }
            foreach (var group in _targetGroups)
            {
                group.rectTransform.localPosition = group.baseValue;
            }
        }

        public override Sequence Show(float delay = 0f)
        {
            MainSequence = MainSequence.ReCreate();

            Slide(delay, true);

            return MainSequence;
        }

        public override Sequence Hide(float delay = 0f)
        {
            MainSequence = MainSequence.ReCreate();

            Slide(delay, false);

            return MainSequence;
        }

        private void Slide(float delay, bool slideIn)
        {
            foreach (var tuple in _targetGroups)
            {
                MainSequence.Insert(0, slideIn
                    ? tuple.rectTransform.DOLocalMove(tuple.endValue, duration).SetEase(showEasing)
                    : tuple.rectTransform.DOLocalMove(tuple.baseValue, duration).SetEase(hideEasing)
                );
            }

            if (delay > 0f)
            {
                MainSequence.PrependInterval(delay);
            }
        }
    }
}