using Data.Constant;
using Infrastructure.PSM.Core;
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
            SceneManager.LoadScene(ScenesNames.EMPTY_SCENE);
            Initializer.StateMachine.SwitchState<LanguageSelectionState>();
        }
    }
}