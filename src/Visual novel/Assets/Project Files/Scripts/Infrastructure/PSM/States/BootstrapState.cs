using Data.Constant;
using Infrastructure.PSM.Core;
using Infrastructure.Services.Metric;
using UnityEngine.SceneManagement;
using Zenject;

namespace Infrastructure.PSM.States
{
    public class BootstrapState : IState<Bootstrap>, IInitializable
    {
        public BootstrapState(Bootstrap initializer, IMetricService metricService)
        {
            Initializer = initializer;
            _metricService = metricService;
        }

        public Bootstrap Initializer { get; }
        private readonly IMetricService _metricService;

        public void Initialize()
        {
            _metricService.SendEvent(MetricEventType.Started);
            SceneManager.LoadScene(ScenesNames.EMPTY_SCENE);
            Initializer.StateMachine.SwitchState<LanguageSelectionState>();
        }
    }
}