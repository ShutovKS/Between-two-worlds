using Infrastructure.Services.AssetsAddressables;
using Infrastructure.Services.CoroutineRunner;
using Infrastructure.Services.Localisation;
using Infrastructure.Services.Metric;
using Infrastructure.Services.Progress;
using Infrastructure.Services.SaveLoad;
using Infrastructure.Services.Sounds;
using Infrastructure.Services.UIFactory;
using Infrastructure.Services.WindowsService;
using Zenject;

namespace Infrastructure.Installers
{
    public class ServicesInstaller : MonoInstaller, ICoroutineRunnerService 
    {
        public override void InstallBindings() 
        {
            BindAssetsAddressablesProviderService();
            BindCoroutineRunnerServiceService();
            BindLocalisationDataLoadService();
            BindSaveLoadDataLocalService();
            BindMetricStubService();
            BindUIFactoryService();
            BindProgressService();
            BindWindowService();
            BindSoundsService();
        }

        private void BindCoroutineRunnerServiceService()
        {
            Container.Bind<ICoroutineRunnerService>().FromInstance(this); 
        }

        private void BindAssetsAddressablesProviderService()
        {
            Container.BindInterfacesTo<AssetsAddressablesProviderService>().AsSingle();
        }

        private void BindUIFactoryService()
        {
            Container.BindInterfacesTo<UIFactoryService>().AsSingle();
        }

        private void BindLocalisationDataLoadService()
        {
            Container.BindInterfacesTo<LocalisationService>().AsSingle();
        }
        
        private void BindSaveLoadDataLocalService()
        {
            Container.BindInterfacesTo<SaveLoadLocalService>().AsSingle();
        }

        private void BindProgressService()
        {
            Container.BindInterfacesTo<ProgressService>().AsSingle();
        }

        private void BindMetricStubService()
        {
            Container.BindInterfacesTo<MetricStubService>().AsSingle();
        }

        private void BindSoundsService()
        {
            Container.BindInterfacesTo<SoundService>().AsSingle();
        }
        
        private void BindWindowService()
        {
            Container.BindInterfacesTo<WindowService>().AsSingle();
        }
    }
}