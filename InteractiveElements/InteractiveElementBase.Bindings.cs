using System;
using System.Collections.Generic;
using CherryUI.DataModels;
using CherryUI.DependencyManager;
using UnityEngine;
using UnityEngine.Events;

namespace CherryUI.InteractiveElements
{
    public abstract partial class InteractiveElementBase : InjectMonoBehaviour
    {
        [Inject] protected readonly ModelService ModelService;

        public readonly List<DownwardBindingHandler> DownwardHandlers = new();
        public readonly Dictionary<IUpwardBinding, Action> UpwardBindingHandlers = new();

        protected virtual void OnDestroy()
        {
            ReleaseAllDownwardBindings();
            ReleaseAllUpwardBindings();
        }
        
        //TODO: Move all this stuff to ModelService or make separate instanced BindingService

        protected void ReleaseAllDownwardBindings()
        {
            for (var i = DownwardHandlers.Count - 1; i >= 0; i--)
            {
                ReleaseDownwardBinding(DownwardHandlers[i]);
            }
        }

        protected void ReleaseDownwardBinding(DownwardBindingHandler handler)
        {
            if (handler?.Model == null) return;
            handler.Model.RemoveDownwardBinding(handler.CallbackDownward);
            DownwardHandlers.Remove(handler);
        }
        
        protected void ReleaseAllUpwardBindings()
        {
            foreach (var kvp in UpwardBindingHandlers)
            {
                kvp.Value.Invoke();
            }
            UpwardBindingHandlers.Clear();
        }
        
        protected void ReleaseUpwardBinding(IUpwardBinding handler)
        {
            UpwardBindingHandlers[handler].Invoke();
            UpwardBindingHandlers.Remove(handler);
        }
    }
}