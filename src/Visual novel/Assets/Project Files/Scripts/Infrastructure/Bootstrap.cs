using Infrastructure.PSM.Core;
using Infrastructure.PSM.States;
using Infrastructure.Services.AssetsAddressables;
using Infrastructure.Services.CoroutineRunner;
using Infrastructure.Services.LocalisationDataLoad;
using Infrastructure.Services.LocalizationUI;
using Infrastructure.Services.Metric;
using Infrastructure.Services.Progress;
using Infrastructure.Services.SaveLoad;
using Infrastructure.Services.Sounds;
using Infrastructure.Services.UIFactory;
using Infrastructure.Services.WindowsService;

namespace Infrastructure
{
    public class Bootstrap
    {
        public Bootstrap(
            ICoroutineRunnerService coroutineRunnerServiceService,
            IAssetsAddressablesProviderService assetsAddressablesProviderService,
            IUIFactoryService uiFactoryService,
            ILocalisationDataLoadService localisationDataLoadService,
            ILocalizerUIService localizerUIService,
            ISaveLoadService saveLoadLocalService,
            IProgressService progressService,
            IMetricService metricStubService,
            IWindowService windowService,
            ISoundService soundService
        )
        {
            StateMachine = new StateMachine<Bootstrap>(
                new BootstrapState(this),
                new LanguageSelectionState(this, windowService, localisationDataLoadService),
                new InitializationState(this, progressService)
            );

            StateMachine.SwitchState<BootstrapState>();
        }

        public readonly StateMachine<Bootstrap> StateMachine;
    }
}