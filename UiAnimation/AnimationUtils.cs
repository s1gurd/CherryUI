using DG.Tweening;

namespace CherryUI.UiAnimation
{
    public static class AnimationUtils
    {
        public static Sequence ReCreate(this Sequence seq, bool complete = false)
        {
            seq.Kill(complete);
            seq = DOTween.Sequence();
            return seq;
        }
    }
}