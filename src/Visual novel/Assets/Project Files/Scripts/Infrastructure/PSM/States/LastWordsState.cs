using System;
using System.Threading.Tasks;
using Data.Constant;
using Infrastructure.PSM.Core;
using Infrastructure.Services.Localisation;
using Infrastructure.Services.Metric;
using Infrastructure.Services.WindowsService;
using UI.Background;
using UI.LastWords;
using UnityEngine;

namespace Infrastructure.PSM.States
{
    public class LastWordsState : IState<Bootstrap>, IEnterableWithOneArg<string>, IExitable
    {
        public LastWordsState(Bootstrap initializer, IWindowService windowService, IMetricService metricService,
            ILocalisationService localisationService)
        {
            _windowService = windowService;
            _metricService = metricService;
            _localisationService = localisationService;
            Initializer = initializer;
        }

        public Bootstrap Initializer { get; }
        private readonly IWindowService _windowService;
        private readonly IMetricService _metricService;
        private readonly ILocalisationService _localisationService;

        public async void OnEnter(string endingNumber)
        {
            await OpenUI(endingNumber);
        }

        public void OnExit()
        {
            _windowService.Close(WindowID.LastWords);
            _windowService.Close(WindowID.Background);
        }

        private async Task OpenUI(string actionTrigger)
        {
            var backgroundUI = await _windowService.OpenAndGetComponent<BackgroundUI>(WindowID.Background);
            backgroundUI.SetImage(Resources.Load<Texture2D>("Data/Backgrounds/" + ResourcesPath.BACKGROUND_PATH));

            var text = _localisationService.GetUpLastWord(actionTrigger);
            
            var wordsUI = await _windowService.OpenAndGetComponent<LastWordsUI>(WindowID.LastWords);
            wordsUI.OnBackButtonClicked = OpenMainMenu;
            wordsUI.SetText(text);

            _metricService.SendEvent(GetMetric(actionTrigger));
        }

        private void OpenMainMenu()
        {
            Initializer.StateMachine.SwitchState<MenuState>();
        }

        private static MetricEventType GetMetric(string actionTrigger)
        {
            return actionTrigger switch
            {
                "end1" => MetricEventType.End1,
                "end2" => MetricEventType.End2,
                _ => throw new ArgumentOutOfRangeException(nameof(actionTrigger), actionTrigger, null)
            };
        }
    }
}