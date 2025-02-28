using CherryUI.InteractiveElements.Widgets;
using CherryUI.UiAnimation;
using DG.Tweening;

namespace CherryUI.InteractiveElements.Populators
{
    public abstract class PopulatorElementBase<T> : WidgetElement where T : class
    {
        protected T Data;

        public virtual void SetData(T data)
        {
            Data = data;
        } 

        public virtual Sequence Refresh()
        {
            var seq = CreateSequence(animators, Purpose.Hide);
            seq.Append(CreateSequence(animators, Purpose.Show));
            seq.AppendCallback(OnRefreshComplete);
            return seq;
        }
        
        protected virtual void OnRefreshComplete()
        {
        }
    }
}