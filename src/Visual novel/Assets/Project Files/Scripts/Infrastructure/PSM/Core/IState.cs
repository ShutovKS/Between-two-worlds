namespace Features.Infrastructure.ProjectStateMachine.Base
{
    public interface IState<out TInitializer>
    {
        public TInitializer Initializer { get; }
    }
}