using Features.Infrastructure.ProjectStateMachine.Base;
using UnityEngine.SceneManagement;
using Zenject;

namespace Infrastructure.PSM.States
{
    public class BootstrapState : IState<Bootstrap>, IInitializable
    {
        public Bootstrap Initializer { get; }

        public BootstrapState(Bootstrap initializer)
        {
            Initializer = initializer;
        }

        public void Initialize()
        {
            SceneManager.LoadScene("1.Loading");
            // Initializer.StateMachine.SwitchState<InitializationState>();
        }
    }
}