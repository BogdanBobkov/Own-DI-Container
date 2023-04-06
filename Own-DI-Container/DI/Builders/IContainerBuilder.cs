namespace DI.Builders
{
    public interface IContainerBuilder
    {
        void Register<T>(T @object);
        T Create<T>(params object[] args) where T : new();
    }
}