using CherryUI.UiAnimation;
using DG.Tweening;

namespace CherryUI.InteractiveElements.Widgets
{
    public class WidgetElement : InteractiveElementBase
    {
        public virtual Sequence Show()
        {
            var seq = CreateSequence(animators, Purpose.Show);
            seq.AppendCallback(OnShowComplete);
            return seq;
        }

        public virtual Sequence Hide()
        {
            var seq = CreateSequence(animators, Purpose.Hide);
            seq.AppendCallback(OnHideComplete);
            return seq;
        }
        
    }
}