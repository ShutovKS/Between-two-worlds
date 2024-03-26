using Infrastructure.PSM.States;
using Zenject;

namespace Infrastructure.Installers
{
    public class GameInstanceInstaller : MonoInstaller 
    { 
        public override void InstallBindings() 
        { 
            BindGameInstance(); 
        } 
 
        private void BindGameInstance() 
        { 
            Container.Bind<Bootstrap>().AsSingle().NonLazy(); 
            Container.Bind<IInitializable>().To<BootstrapState>().AsSingle().NonLazy();
        } 
    } 
}