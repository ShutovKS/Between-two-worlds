namespace Features.Infrastructure.ProjectStateMachine.Base
{
    public interface IEnterableWithOneArg<in T0> 
    {
        public void OnEnter(T0 startSceneController);
    }
}