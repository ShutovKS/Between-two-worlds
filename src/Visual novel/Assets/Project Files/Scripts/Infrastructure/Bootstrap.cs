using Features.Infrastructure.ProjectStateMachine.Base;
using Infrastructure.PSM.States;
using Infrastructure.Services.AssetsAddressables;
using Infrastructure.Services.CoroutineRunner;
using Infrastructure.Services.LocalisationDataLoad;
using Infrastructure.Services.LocalizationUI;
using Infrastructure.Services.Metric;
using Infrastructure.Services.Progress;
using Infrastructure.Services.SaveLoadData;
using Infrastructure.Services.Sounds;
using Infrastructure.Services.UIFactory;

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
            ISaveLoadDataService saveLoadDataLocalService,
            IProgressService progressService,
            IMetricService metricStubService,
            ISoundsService soundsService
        )
        {
            StateMachine = new StateMachine<Bootstrap>(
                new BootstrapState(this)
            );
            
            StateMachine.SwitchState<BootstrapState>();
        }

        public readonly StateMachine<Bootstrap> StateMachine;
    }
}