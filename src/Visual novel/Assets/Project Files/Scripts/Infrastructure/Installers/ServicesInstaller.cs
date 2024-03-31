using Infrastructure.Services.AssetsAddressables;
using Infrastructure.Services.Authenticate;
using Infrastructure.Services.CoroutineRunner;
using Infrastructure.Services.DialogueStories;
using Infrastructure.Services.Localisation;
using Infrastructure.Services.Metric;
using Infrastructure.Services.Progress;
using Infrastructure.Services.SaveLoad;
using Infrastructure.Services.ScreenshotsOfSaves;
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
            BindCoroutineRunnerService();
            BindAssetsAddressablesProviderService();
            BindScreenshotsOfSavesService();
            BindDialogueHistoryService();
            BindAuthenticatedService();
            BindLocalisationService();
            BindMetricStubService();
            BindUIFactoryService();
            BindSaveLoadService();
            BindProgressService();
            BindWindowService();
            BindSoundService();
        }

        private void BindCoroutineRunnerService()
        {
            Container.Bind<ICoroutineRunnerService>().FromInstance(this).NonLazy();
        }

        private void BindAssetsAddressablesProviderService()
        {
            Container.BindInterfacesTo<AssetsAddressablesProviderService>().AsSingle().NonLazy();
        }

        private void BindAuthenticatedService()
        {
#if YG_SERVICES
            Container.BindInterfacesTo<AuthenticatedYGService>().AsSingle().NonLazy();
#else
            Container.BindInterfacesTo<AuthenticatedStubService>().AsSingle().NonLazy();
#endif
        }

        private void BindScreenshotsOfSavesService()
        {
            Container.BindInterfacesTo<ScreenshotsOfSavesService>().AsSingle().NonLazy();
        }

        private void BindDialogueHistoryService()
        {
            Container.BindInterfacesTo<DialogueHistoryService>().AsSingle().NonLazy();
        }

        private void BindLocalisationService()
        {
            Container.BindInterfacesTo<LocalisationService>().AsSingle().NonLazy();
        }

        private void BindMetricStubService()
        {
#if YG_SERVICES
            Container.BindInterfacesTo<MetricYGService>().AsSingle().NonLazy();
#else
            Container.BindInterfacesTo<MetricStubService>().AsSingle().NonLazy();
#endif
        }

        private void BindUIFactoryService()
        {
            Container.BindInterfacesTo<UIFactoryService>().AsSingle().NonLazy();
        }

        private void BindSaveLoadService()
        {
#if YG_SERVICES
            Container.BindInterfacesTo<SaveLoadDataYGService>().AsSingle().NonLazy();
#else
            Container.BindInterfacesTo<SaveLoadLocalService>().AsSingle().NonLazy();
#endif
        }

        private void BindProgressService()
        {
            Container.BindInterfacesTo<ProgressService>().AsSingle().NonLazy();
        }

        private void BindWindowService()
        {
            Container.BindInterfacesTo<WindowService>().AsSingle().NonLazy();
        }

        private void BindSoundService()
        {
            Container.BindInterfacesTo<SoundService>().AsSingle().NonLazy();
        }
    }
}