using UnityEngine;

namespace CherryUI.DependencyManager
{
    [DefaultExecutionOrder(-1000)][RequireComponent(typeof(DependenciesDisposer))]
    public abstract class InstallerMonoBehaviour : MonoBehaviour
    {
        protected DependencyContainer Container => DependencyContainer.Instance;

        protected abstract void Install();

        private void Awake()
        {
            Install();
        }
    }
}