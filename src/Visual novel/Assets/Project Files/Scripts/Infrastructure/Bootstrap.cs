using Infrastructure.PSM.Core;
using Infrastructure.PSM.States;
using Infrastructure.Services.AssetsAddressables;
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

namespace Infrastructure
{
    public class Bootstrap
    {
        public Bootstrap(
            IAssetsAddressablesProviderService assetsAddressablesProviderService,
            IScreenshotsOfSavesService screenshotsOfSavesService,
            ICoroutineRunnerService coroutineRunnerService,
            IDialogueHistoryService dialogueHistoryService,
            ILocalisationService localisationService,
            IUIFactoryService uiFactoryService,
            ISaveLoadService saveLoadService,
            IProgressService progressService,
            IMetricService metricService,
            IWindowService windowService,
            ISoundService soundService
        )
        {
            StateMachine = new StateMachine<Bootstrap>(
                new BootstrapState(this),
                new LanguageSelectionState(this, windowService, localisationService),
                new InitializationState(this, progressService, screenshotsOfSavesService),
                new MenuState(this, windowService, progressService, soundService),
                new SaveMenuState(this, progressService, windowService, screenshotsOfSavesService),
                new LoadMenuState(this, progressService, windowService),
                new GameplayState(this, windowService, progressService, coroutineRunnerService, soundService,
                    localisationService, dialogueHistoryService),
                new LastWordsState(this, windowService, metricService, localisationService)
            );

            StateMachine.SwitchState<BootstrapState>();
        }

        public readonly StateMachine<Bootstrap> StateMachine;
    }
}