using Infrastructure.Services.AssetsAddressables;
using Infrastructure.Services.CoroutineRunner;
using Infrastructure.Services.LocalisationDataLoad;
using Infrastructure.Services.LocalizationUI;
using Infrastructure.Services.Metric;
using Infrastructure.Services.Progress;
using Infrastructure.Services.SaveLoadData;
using Infrastructure.Services.Sounds;
using Infrastructure.Services.UIFactory;
using Zenject;

namespace Infrastructure.Installers
{
    public class ServicesInstaller : MonoInstaller, ICoroutineRunnerService 
    {
        public override void InstallBindings() 
        {
            BindCoroutineRunnerServiceService();
            BindAssetsAddressablesProviderService();
            BindUIFactoryService();
            BindLocalisationDataLoadService();
            BindLocalizerUIServiceService();
            BindSaveLoadDataLocalService();
            BindProgressService();
            BindMetricStubService();
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
            Container.BindInterfacesTo<LocalisationDataLoadService>().AsSingle();
        }

        private void BindLocalizerUIServiceService()
        {
            Container.BindInterfacesTo<LocalizerUIService>().AsSingle();
        }

        private void BindSaveLoadDataLocalService()
        {
            Container.BindInterfacesTo<SaveLoadDataLocalService>().AsSingle();
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
            Container.BindInterfacesTo<SoundsService>().AsSingle();
        }
    }
}