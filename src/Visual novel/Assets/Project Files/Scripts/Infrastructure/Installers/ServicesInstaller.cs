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
            Container.BindInterfacesTo<AuthenticatedStubService>().AsSingle().NonLazy();
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
            Container.BindInterfacesTo<MetricStubService>().AsSingle().NonLazy();
        }

        private void BindUIFactoryService()
        {
            Container.BindInterfacesTo<UIFactoryService>().AsSingle().NonLazy();
        }

        private void BindSaveLoadService()
        {
            Container.BindInterfacesTo<SaveLoadLocalService>().AsSingle().NonLazy();
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