using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace CherryUI.DataModels
{
    [Serializable]
    public abstract class DataModelBase : IDisposable
    {
        private readonly Dictionary<string, Delegate> _callbacks = new ();
        private Action _upwardDisposeActions = () => { };
        private bool _debugMode;

        protected Dictionary<string, Delegate> Getters = new ();
        protected Dictionary<string, Delegate> Setters = new ();
        
        [JsonIgnore]
        public bool Ready
        {
            get => _ready;
            set { _ready = value; SendDownward(nameof(Ready), value); }
        }

        private bool _ready;

        protected DataModelBase()
        {
            Getters.Add(nameof(Ready), new Func<bool>(() => Ready));
            Setters.Add(nameof(Ready), new Action<bool>(o => Ready = o));
            ReadyAccessor = new Accessor<bool>(this, nameof(Ready));
        }
        
        
        public Accessor<bool> ReadyAccessor;

        public void AddDownWardBinding(string memberName, Delegate callback)
        {
            if (!_callbacks.ContainsKey(memberName))
            {
                _callbacks.Add(memberName, callback);
            } 
            else
            {
                _callbacks[memberName] = Delegate.Combine(_callbacks[memberName], callback);
            }
            
            if (Getters.TryGetValue(memberName, out var a) && a != null)
            { 
                callback.DynamicInvoke(a.DynamicInvoke());
            }
        }

        public void RemoveDownwardBinding(Delegate callback)
        {
            var keys = _callbacks.Where(kvp => kvp.Value != null && kvp.Value.GetInvocationList().Contains(callback)).Select(kvp => kvp.Key).ToList();
            for (var i=0; i < keys.Count(); i++)
            {
                _callbacks[keys[i]] = Delegate.Remove(_callbacks[keys[i]], callback);
            }
        }

        public void SetValue<T>(string memberName, T value)
        {
            if (Setters.TryGetValue(memberName, out var action) && action != null)
            {
                if (_debugMode)
                {
                    Debug.Log($"[{this.GetType().Name}] Received upwards {memberName} = {value?.ToString()}");
                }
                action.DynamicInvoke(value);
            }
            else
            {
                Debug.LogError($"[{this.GetType().Name}] Not found member with name {memberName} while trying to set its value");
            }
        }

        protected void SendDownward(string memberName, object value)
        {
            if (_debugMode)
            {
                Debug.Log($"[{this.GetType().Name}] Send downwards {memberName} = {value?.ToString()}");
            }
            if (!_callbacks.TryGetValue(memberName, out var action)) return;
            action.DynamicInvoke(new object[]{value});
        }
        
        protected void SendDownward<T>(string memberName, T value)
        {
            if (_debugMode)
            {
                Debug.Log($"[{this.GetType().Name}] Send downwards {memberName} = {value?.ToString()}");
            }
            if (!_callbacks.TryGetValue(memberName, out var action)) return;
            action.DynamicInvoke(new object[]{value});
        }

        public void InvokeDownwardBinding(string memberName)
        {
            if (Getters.TryGetValue(memberName, out var getter))
            {
                var value = getter.DynamicInvoke();
                SendDownward(memberName, value);
            }
            else
            {
                Debug.LogError($"[{this.GetType().Name}] Not found member with name {memberName} while trying to invoke its downward bindings!");
            }
        }

        public void InvokeAllDownwardsBindings()
        {
            foreach (var kvp in Getters)
            {
                SendDownward(kvp.Key, kvp.Value.DynamicInvoke());
            }
        }

        public void AddUpwardDisposeAction(Action action)
        {
            _upwardDisposeActions += action;
        }

        public void SetDebugMode(bool value)
        {
            _debugMode = value;
        }

        public void Dispose()
        {
            _upwardDisposeActions.Invoke();
        }
    }
}