using System;

namespace CherryUI.UiAnimation
{
    [Serializable]
    public class UiAnimationSettings
    {
        public UiAnimationBase animator;
        public float delay = 0f;
        public LaunchMode launchMode;
    }

    [Serializable]
    public enum LaunchMode
    {
        AtGlobalAnimationStart = 0,
        AfterPreviousAnimatorStart = 1,
        AfterPreviousAnimatorFinished = 2
    }

    [Serializable]
    public enum Purpose
    {
        Show = 0,
        Hide = 1
    }
}