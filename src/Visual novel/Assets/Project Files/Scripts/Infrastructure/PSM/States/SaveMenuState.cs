using System;
using System.Globalization;
using System.Threading.Tasks;
using Infrastructure.PSM.Core;
using Infrastructure.Services.Metric;
using Infrastructure.Services.Progress;
using Infrastructure.Services.ScreenshotsOfSaves;
using Infrastructure.Services.WindowsService;
using UI.Background;
using UI.SaveLoad;
using UnityEngine;

namespace Infrastructure.PSM.States
{
    public class SaveMenuState : IState<Bootstrap>, IEnterableWithOneArg<string>, IExitable
    {
        public SaveMenuState(Bootstrap initializer, IProgressService progressService, IWindowService windowService,
            IScreenshotsOfSavesService screenshotsOfSavesService, IMetricService metricService)
        {
            _progressService = progressService;
            _windowService = windowService;
            _screenshotsOfSavesService = screenshotsOfSavesService;
            _metricService = metricService;
            Initializer = initializer;
        }

        public Bootstrap Initializer { get; }

        private readonly IProgressService _progressService;
        private readonly IWindowService _windowService;
        private readonly IScreenshotsOfSavesService _screenshotsOfSavesService;
        private readonly IMetricService _metricService;

        private string _newDialogueId;

        public async void OnEnter(string newDialogueId)
        {
            _newDialogueId = newDialogueId;

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
                var index = i;
                var data = gameData.dialogues[i];
                var ui = saveLoadUI.SaveDataUIs[i];

                ui.SetImage(data.Background);
                ui.SetTitle(data.titleText);
                ui.OnButtonClicked = () => { SetNewIdDialogue(index); };
            }
        }

        private async void SetNewIdDialogue(int saveId)
        {
            var gameData = await _progressService.GetProgress();
            var dialoguesData = gameData.dialogues[saveId];
            dialoguesData.idLastDialogue = _newDialogueId;
            dialoguesData.Background = await _screenshotsOfSavesService.GetImage(_newDialogueId);
            dialoguesData.titleText = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            dialoguesData.isDataExist = true;
            _progressService.SetProgress(gameData);
            
            _metricService.SendEvent(MetricEventType.Saved);

            Back();
        }

        private void Back()
        {
            Initializer.StateMachine.SwitchState<GameplayState>();
        }
    }
}