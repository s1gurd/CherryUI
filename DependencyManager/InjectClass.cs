namespace CherryUI.DependencyManager
{
    public abstract class InjectClass : IInjectTarget
    {
        protected InjectClass()
        {
            DependencyContainer.Instance.InjectDependencies(this);
        }
    }
}