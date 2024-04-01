using System;
using System.Threading.Tasks;
using Data.Constant;
using Data.Dynamic;
using Infrastructure.PSM.Core;
using Infrastructure.Services.Metric;
using Infrastructure.Services.Progress;
using Infrastructure.Services.Sounds;
using Infrastructure.Services.WindowsService;
using UI.Background;
using UI.MainMenu;
using UnityEngine;

namespace Infrastructure.PSM.States
{
    public class MenuState : IState<Bootstrap>, IEnterable, IExitable
    {
        public MenuState(Bootstrap initializer, IWindowService windowService, IProgressService progressService,
            ISoundService soundService)
        {
            _windowService = windowService;
            _progressService = progressService;
            _soundService = soundService;
            Initializer = initializer;
        }

        public Bootstrap Initializer { get; }
        private readonly IWindowService _windowService;
        private readonly IProgressService _progressService;
        private readonly ISoundService _soundService;

        public async void OnEnter()
        {
            await Initialize();
        }

        public void OnExit()
        {
            _windowService.Close(WindowID.MainMenu);
            _windowService.Close(WindowID.Background);
        }

        private async Task Initialize()
        {
            var gameData = await _progressService.GetProgress();

            var backgroundUI = await _windowService.OpenAndGetComponent<BackgroundUI>(WindowID.Background);
            backgroundUI.SetImage(Resources.Load<Texture2D>("Data/Backgrounds/" + ResourcesPath.BACKGROUND_PATH));

            var menuUI = await _windowService.OpenAndGetComponent<MainMenuUI>(WindowID.MainMenu);
            menuUI.Buttons.OnExitButtonClicked = Exit;
            menuUI.Buttons.OnLoadGameButtonClicked = LoadDataGame;
            menuUI.Buttons.OnStartNewGameButtonClicked = StartNewGame;
            menuUI.Buttons.OnContinueGameButtonClicked = ContinueGame;
            menuUI.Buttons.SetContinueGameButtonInteractable(!string.IsNullOrEmpty(gameData.currentDialogue));
            menuUI.SettingsButtons.OnLanguageButtonClicked = OpenLanguageMenu;

            _soundService.SetClip(ResourcesPath.SOUND_MAIN_MENU, true);
        }

        private async void StartNewGame()
        {
            var gameData = await _progressService.GetProgress();
            gameData.currentDialogue = PlayerPrefsPath.DIALOG_START_ID;
            gameData.LastSaveTime = DateTime.Now;
            _progressService.SetProgress(gameData);

            ContinueGame();
        }

        private void ContinueGame()
        {
            Initializer.StateMachine.SwitchState<GameplayState>();
        }

        private void LoadDataGame()
        {
            Initializer.StateMachine.SwitchState<LoadMenuState, MenuState>(this);
        }

        private void OpenLanguageMenu()
        {
            Initializer.StateMachine.SwitchState<LanguageSelectionState>();
        }

        private void Exit()
        {
            Application.Quit();
        }
    }
}