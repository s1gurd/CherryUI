using System;
using CherryUI.InteractiveElements;
using UnityEngine.Events;

namespace CherryUI.DataModels
{
    public class Accessor<T>
    {
        private readonly DataModelBase _model;
        private readonly string _memberName;

        public Accessor(DataModelBase model, string memberName)
        {
            _memberName = memberName;
            _model = model;
        }

        public DownwardBindingHandler BindDownwards(Action<T> callback)
        {
            var handler = new DownwardBindingHandler(_model, callback);
            var interactiveElementTarget = callback.Target as InteractiveElementBase;
            if (interactiveElementTarget != null)
            {
                interactiveElementTarget.DownwardHandlers.Add(handler);
            }
            _model.AddDownWardBinding(_memberName, handler.CallbackDownward);
            return handler;
        }
        
        public UpwardBindingHandler<T> BindUpwards(UnityEvent<T> evt)
        {
            UnityAction<T> callback = value => _model.SetValue(_memberName, value);
            var handler = new UpwardBindingHandler<T>(evt, callback);
            evt.AddListener(handler.CallbackUpward);
            var interactiveElementTarget = callback.Target as InteractiveElementBase;
            if (interactiveElementTarget != null)
            {
                interactiveElementTarget.UpwardBindingHandlers.Add(handler, () => {evt.RemoveListener(handler.CallbackUpward);});
            }
            _model.AddUpwardDisposeAction(() => evt.RemoveListener(handler.CallbackUpward));
            return handler;
        }
    }
}