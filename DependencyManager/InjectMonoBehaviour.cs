using UnityEngine;

namespace CherryUI.DependencyManager
{
    public abstract class InjectMonoBehaviour : MonoBehaviour, IInjectTarget
    {
        private bool _injected;
        
        protected virtual void OnEnable()
        {
            if (_injected) return;
            
            DependencyContainer.Instance.InjectDependencies(this);
            _injected = true;
        }
    }
}