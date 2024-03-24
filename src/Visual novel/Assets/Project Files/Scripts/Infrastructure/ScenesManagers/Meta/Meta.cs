#region

using System;
using Data.Constant;
using Infrastructure.Services;
using Infrastructure.Services.LocalisationDataLoad;
using Infrastructure.Services.LocalizationUI;
using Infrastructure.Services.Metric;
using Infrastructure.Services.Progress;
using Infrastructure.Services.Sounds;
using Infrastructure.Services.UIFactory;
using UnityEngine;
using UnityEngine.SceneManagement;

#endregion

namespace Infrastructure.ScenesManagers.Meta
{
    public class Meta : MonoBehaviour
    {
        private MainMenu _menu;

        private ILocalisationDataLoadService _localisationDataLoad;
        private ILocalizerUIService _localizerUI;
        private IProgressService _progress;
        private IUIFactoryInfoService _uiFactoryInfo;
        private ISoundsService _sounds;
        private IMetricService _metric;

        private AsyncOperation _loadSceneAsync;

        private void Start()
        {
            InitializedServices();

            var gameData = _progress.GetProgress();
            
            _uiFactoryInfo.BackgroundUI.SetBackgroundImage(
                Resources.Load<Texture2D>("Data/Backgrounds/" + ResourcesPath.BACKGROUND_PATH));

            var mainMenuUI = _uiFactoryInfo.MainMenuUI;
            mainMenuUI.Buttons.OnExitButtonClicked = Exit;
            mainMenuUI.Buttons.OnLoadGameButtonClicked = LoadDataGame;
            mainMenuUI.Buttons.OnStartNewGameButtonClicked = StartNewGame;
            mainMenuUI.Buttons.OnContinueGameButtonClicked = ContinueGame;
            mainMenuUI.Buttons.SetContinueGameButtonInteractable(!string.IsNullOrEmpty(gameData.currentDialogue));
            mainMenuUI.SettingsButtons.OnLanguageButtonClicked = OpenLanguageMenu; 
            
            _menu = new MainMenu(mainMenuUI);

            _sounds.SetClip(ResourcesPath.SOUND_MAIN_MENU, true);

            _loadSceneAsync = SceneManager.LoadSceneAsync("3.Core");
            _loadSceneAsync.allowSceneActivation = false;

            OpenMenu();
        }

        private void StartNewGame()
        {
            var gameData = _progress.GetProgress();
            gameData.currentDialogue = PlayerPrefsPath.DIALOG_START_ID;
            gameData.LastSaveTime = DateTime.Now;
            _progress.SetProgress(gameData);

            ContinueGame();
        }

        private void ContinueGame()
        {
            _menu.ClosedMenu();

            _loadSceneAsync.allowSceneActivation = true;
        }

        private void LoadDataGame()
        {
            _uiFactoryInfo.SaveLoadUI.SetActivePanel(true);
            _uiFactoryInfo.SaveLoadUI.ButtonsUI.OnButtonClicked = () => _uiFactoryInfo.SaveLoadUI.SetActivePanel(false);

            var number = 0;
            var gameData = _progress.GetProgress();

            foreach (var ui in _uiFactoryInfo.SaveLoadUI.SaveDataUIs)
            {
                var data = gameData.dialogues[number++];

                if (!data.isDataExist)
                {
                    continue;
                }

                ui.SetImage(data.Background);
                ui.SetTitle(data.titleText);
                ui.OnButtonClicked = () =>
                {
                    _metric.SendEvent(MetricEventType.Load);
                    
                    gameData.currentDialogue = data.idLastDialogue;

                    _uiFactoryInfo.SaveLoadUI.SetActivePanel(false);

                    _menu.ClosedMenu();

                    _loadSceneAsync.allowSceneActivation = true;
                };
            }
        }

        private void OpenMenu()
        {
            _menu.OpenMenu();
        }

        private void Exit()
        {
            Application.Quit();
        }

        private void OpenLanguageMenu()
        {
            _uiFactoryInfo.MainMenuUI.SetActivePanel(false);
            _uiFactoryInfo.ChooseLanguageUI.SetActivePanel(true);
            _uiFactoryInfo.ChooseLanguageUI.ScrollViewLanguages.OnSelectLanguage += ChangeLocalisation;
        }
        
        private void ChangeLocalisation(string language)
        {
            _uiFactoryInfo.ChooseLanguageUI.ScrollViewLanguages.OnSelectLanguage += ChangeLocalisation;
            
            _localisationDataLoad.Load(language);

            var localisation = _localisationDataLoad.GetUILocalisation();

            _localizerUI.Localize(localisation);

            _uiFactoryInfo.ChooseLanguageUI.SetActivePanel(false);
            _uiFactoryInfo.MainMenuUI.SetActivePanel(true);
        }

        private void InitializedServices()
        {
            _uiFactoryInfo = ServicesContainer.GetService<IUIFactoryInfoService>();
            _localisationDataLoad = ServicesContainer.GetService<ILocalisationDataLoadService>();
            _progress = ServicesContainer.GetService<IProgressService>();
            _uiFactoryInfo = ServicesContainer.GetService<IUIFactoryInfoService>();
            _localizerUI = ServicesContainer.GetService<ILocalizerUIService>();
            _sounds = ServicesContainer.GetService<ISoundsService>();
            _metric = ServicesContainer.GetService<IMetricService>();
        }
    }
}