using DG.Tweening;

namespace CherryUI.UiAnimation.Animators
{
    public class UiActive : UiAnimationBase
    {
        protected override void OnEnable()
        {
            if (!Inited)
            {
                MainSequence = DOTween.Sequence();

                Inited = true;
            }
        }

        public override Sequence Show(float delay = 0)
        {
            MainSequence = MainSequence.ReCreate();
            return MainSequence.AppendInterval(duration + delay).AppendCallback(() => SetActive(true));
        }

        public override Sequence Hide(float delay = 0)
        {
            MainSequence = MainSequence.ReCreate();
            return MainSequence.AppendInterval(duration + delay).AppendCallback(() => SetActive(false));
        }

        private void SetActive(bool active)
        {
            foreach (var target in Targets)
            {
                target.gameObject.SetActive(active);
            }
        }
    }
}