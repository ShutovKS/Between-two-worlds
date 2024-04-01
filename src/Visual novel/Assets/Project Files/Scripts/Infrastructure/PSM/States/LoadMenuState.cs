using System;
using System.Threading.Tasks;
using Infrastructure.PSM.Core;
using Infrastructure.Services.Metric;
using Infrastructure.Services.Progress;
using Infrastructure.Services.WindowsService;
using UI.Background;
using UI.SaveLoad;
using UnityEngine;

namespace Infrastructure.PSM.States
{
    public class LoadMenuState : IState<Bootstrap>, IEnterableWithOneArg<IState<Bootstrap>>, IExitable
    {
        public LoadMenuState(Bootstrap initializer, IProgressService progressService, IWindowService windowService,
            IMetricService metricService)
        {
            _progressService = progressService;
            _windowService = windowService;
            _metricService = metricService;
            Initializer = initializer;
        }

        public Bootstrap Initializer { get; }
        private readonly IProgressService _progressService;
        private readonly IWindowService _windowService;
        private readonly IMetricService _metricService;
        private IState<Bootstrap> _state;

        public async void OnEnter(IState<Bootstrap> state)
        {
            _state = state;

            await OpenUI();
        }

        public void OnExit()
        {
            _windowService.Close(WindowID.SaveLoad);
            _windowService.Close(WindowID.Background);
        }

        private async Task OpenUI()
        {
            var backgroundUI = await _windowService.OpenAndGetComponent<BackgroundUI>(WindowID.Background);
            backgroundUI.SetColor(Color.black);

            var saveLoadUI = await _windowService.OpenAndGetComponent<SaveLoadUI>(WindowID.SaveLoad);
            var gameData = await _progressService.GetProgress();

            saveLoadUI.ButtonsUI.OnButtonClicked = Back;

            for (var i = 0; i < saveLoadUI.SaveDataUIs.Length && i < gameData.dialogues.Length; i++)
            {
                var data = gameData.dialogues[i];
                if (!data.isDataExist)
                {
                    continue;
                }

                var ui = saveLoadUI.SaveDataUIs[i];

                ui.SetImage(data.Background);
                ui.SetTitle(data.titleText);
                ui.OnButtonClicked = async () => await SetNewCurrentIdDialogue(data.idLastDialogue);
            }
        }

        private async Task SetNewCurrentIdDialogue(string newId)
        {
            var gameData = await _progressService.GetProgress();
            gameData.currentDialogue = newId;
            _progressService.SetProgress(gameData);

            _metricService.SendEvent(MetricEventType.Load);

            Initializer.StateMachine.SwitchState<GameplayState>();
        }

        private void Back()
        {
            switch (_state)
            {
                case MenuState:
                    Initializer.StateMachine.SwitchState<MenuState>();
                    break;
                case GameplayState:
                    Initializer.StateMachine.SwitchState<GameplayState>();
                    break;
                default: throw new Exception($"Unprocessed state for transition from boot menu.");
            }
        }
    }
}