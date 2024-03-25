namespace Infrastructure.PSM.Core
{
    public interface IState<out TInitializer>
    {
        public TInitializer Initializer { get; }
    }
}