using Infrastructure.PSM.Core;
using Infrastructure.PSM.States;
using Infrastructure.Services.AssetsAddressables;
using Infrastructure.Services.CoroutineRunner;
using Infrastructure.Services.Localisation;
using Infrastructure.Services.Metric;
using Infrastructure.Services.Progress;
using Infrastructure.Services.SaveLoad;
using Infrastructure.Services.ScreenshotsOfSaves;
using Infrastructure.Services.Sounds;
using Infrastructure.Services.UIFactory;
using Infrastructure.Services.WindowsService;

namespace Infrastructure
{
    public class Bootstrap
    {
        public Bootstrap(
            IAssetsAddressablesProviderService assetsAddressablesProviderService,
            IScreenshotsOfSavesService screenshotsOfSavesService,
            ICoroutineRunnerService coroutineRunnerService,
            ILocalisationService localisationService,
            ISaveLoadService saveLoadLocalService,
            IUIFactoryService uiFactoryService,
            IProgressService progressService,
            IMetricService metricStubService,
            IWindowService windowService,
            ISoundService soundService
        )
        {
            StateMachine = new StateMachine<Bootstrap>(
                new BootstrapState(this),
                new InitializationState(this, progressService, screenshotsOfSavesService),
                new LanguageSelectionState(this, windowService, localisationService)
            );

            StateMachine.SwitchState<BootstrapState>();
        }

        public readonly StateMachine<Bootstrap> StateMachine;
    }
}