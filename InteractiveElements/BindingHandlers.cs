using System;
using CherryUI.DataModels;
using UnityEngine.Events;

namespace CherryUI.InteractiveElements
{
    public class DownwardBindingHandler
    {
        public readonly DataModelBase Model;
        public Delegate CallbackDownward { get; }
            
        public DownwardBindingHandler(DataModelBase model, Delegate callback)
        {
            Model = model;
            CallbackDownward = callback;
        }
    }
    
    public class UpwardBindingHandler<T> : IUpwardBinding
    {
        public readonly UnityEvent<T> UIHandler;
        public void CallbackUpward(T value) => _callbackUpward(value);
        private readonly UnityAction<T> _callbackUpward;
            
        public UpwardBindingHandler(UnityEvent<T> uiHandler, UnityAction<T> callback)
        {
            UIHandler = uiHandler;
            _callbackUpward = callback;
        }
    }

    public interface IUpwardBinding
    {
    }
}