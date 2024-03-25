namespace Infrastructure.PSM.Core
{
    public interface IEnterableWithOneArg<in T0> 
    {
        public void OnEnter(T0 startSceneController);
    }
}