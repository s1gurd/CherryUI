﻿using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace CherryUI.UiAnimation.Animators
{
    public class UiFade : UiAnimationBase
    {
        private List<(CanvasGroup canvasGroup, float baseAlpha)> _targetGroups = new();

        protected override void OnEnable()
        {
            if (!Inited)
            {
                foreach (var target in Targets)
                {
                    var canvasGroup = target.GetComponent<CanvasGroup>();
                    if (canvasGroup)
                    {
                        _targetGroups.Add((canvasGroup, canvasGroup.alpha));
                        canvasGroup.alpha = 0f;
                    }
                }

                MainSequence = DOTween.Sequence();
            }

            Inited = true;

            foreach (var group in _targetGroups)
            {
                group.canvasGroup.alpha = 0f;
            }
        }

        public override Sequence Show(float delay = 0f)
        {
            MainSequence = MainSequence.ReCreate();

            Fade(delay, true);

            return MainSequence;
        }

        public override Sequence Hide(float delay = 0f)
        {
            MainSequence = MainSequence.ReCreate();

            Fade(delay, false);

            return MainSequence;
        }

        private void Fade(float delay, bool fadeIn)
        {
            foreach (var tuple in _targetGroups)
            {
                MainSequence.Insert(0,
                    fadeIn
                        ? tuple.canvasGroup.DOFade(tuple.baseAlpha, duration).SetEase(showEasing)
                        : tuple.canvasGroup.DOFade(0f, duration).SetEase(hideEasing));
            }

            if (delay > 0f)
            {
                MainSequence.PrependInterval(delay);
            }
        }
    }
}