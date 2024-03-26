using System;
using System.Globalization;
using System.Threading.Tasks;
using Infrastructure.PSM.Core;
using Infrastructure.Services.Progress;
using Infrastructure.Services.ScreenshotsOfSaves;
using Infrastructure.Services.WindowsService;
using UI.SaveLoad;

namespace Infrastructure.PSM.States
{
    public class SaveMenuState : IState<Bootstrap>, IEnterableWithOneArg<string>, IExitable
    {
        public SaveMenuState(Bootstrap initializer, IProgressService progressService, IWindowService windowService,
            IScreenshotsOfSavesService screenshotsOfSavesService)
        {
            _progressService = progressService;
            _windowService = windowService;
            _screenshotsOfSavesService = screenshotsOfSavesService;
            Initializer = initializer;
        }

        public Bootstrap Initializer { get; }

        private readonly IProgressService _progressService;
        private readonly IWindowService _windowService;
        private readonly IScreenshotsOfSavesService _screenshotsOfSavesService;

        private string _newDialogueId;

        public async void OnEnter(string newDialogueId)
        {
            _newDialogueId = newDialogueId;

            await OpenUI();
        }

        public void OnExit()
        {
            _windowService.Close(WindowID.SaveLoad);
        }

        private async Task OpenUI()
        {
            var saveLoadUI = await _windowService.OpenAndGetComponent<SaveLoadUI>(WindowID.SaveLoad);
            var gameData = _progressService.GetProgress();

            saveLoadUI.ButtonsUI.OnButtonClicked = Back;

            for (var i = 0; i < saveLoadUI.SaveDataUIs.Length && i < gameData.dialogues.Length; i++)
            {
                var index = i;
                var data = gameData.dialogues[i];
                if (!data.isDataExist)
                {
                    continue;
                }

                var ui = saveLoadUI.SaveDataUIs[i];

                ui.SetImage(data.Background);
                ui.SetTitle(data.titleText);
                ui.OnButtonClicked = () => { SetNewIdDialogue(index); };
            }
        }

        private async void SetNewIdDialogue(int saveId)
        {
            var gameData = _progressService.GetProgress();
            gameData.dialogues[saveId].idLastDialogue = _newDialogueId;
            gameData.dialogues[saveId].Background = await _screenshotsOfSavesService.GetImage(_newDialogueId);
            gameData.dialogues[saveId].titleText = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            _progressService.SetProgress(gameData);

            Back();
        }

        private void Back()
        {
            Initializer.StateMachine.SwitchState<GameplayState>();
        }
    }
}