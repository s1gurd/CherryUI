using UnityEngine;

namespace CherryUI.DependencyManager
{
    [DefaultExecutionOrder(10000)]
    public class DependenciesDisposer : MonoBehaviour
    {
        private void OnDestroy()
        {
            DependencyContainer.Instance.Dispose();
        }
    }
}