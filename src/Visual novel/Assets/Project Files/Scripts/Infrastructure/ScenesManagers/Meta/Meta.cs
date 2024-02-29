#region

using Data.Constant;
using Infrastructure.Services;
using Infrastructure.Services.LocalisationDataLoad;
using Infrastructure.Services.LocalizationUI;
using Infrastructure.Services.SaveLoadData;
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
        private ISaveLoadDataService _saveLoadData;
        private IUIFactoryInfoService _uiFactoryInfo;
        private ISoundsService _sounds;

        private AsyncOperation _loadSceneAsync;

        private void Start()
        {
            InitializedServices();

            _uiFactoryInfo.BackgroundUI.SetBackgroundImage(
                Resources.Load<Texture2D>("Data/Backgrounds/" + ResourcesPath.BACKGROUND_PATH));

            _menu = new MainMenu(_uiFactoryInfo.MainMenuUI);
            _uiFactoryInfo.MainMenuUI.Buttons.OnExitButtonClicked = Exit;
            _uiFactoryInfo.MainMenuUI.Buttons.OnLoadGameButtonClicked = LoadDataGame;
            _uiFactoryInfo.MainMenuUI.Buttons.OnStartNewGameButtonClicked = StartNewGame;
            _uiFactoryInfo.MainMenuUI.Buttons.OnContinueGameButtonClicked = ContinueGame;

            var gameData = _saveLoadData.GetData();
            _uiFactoryInfo.MainMenuUI.Buttons.SetContinueGameButtonInteractable(
                !string.IsNullOrEmpty(gameData.currentDialogue));

            _sounds.SetClip(ResourcesPath.SOUND_MAIN_MENU, true);

            _loadSceneAsync = SceneManager.LoadSceneAsync("3.Core");
            _loadSceneAsync.allowSceneActivation = false;

            OpenMenu();
        }

        private void StartNewGame()
        {
            var gameData = _saveLoadData.GetData();
            gameData.currentDialogue = PlayerPrefsPath.DIALOG_START_ID;
            _saveLoadData.Save(gameData);

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
            var gameData = _saveLoadData.GetData();

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

        private void ChangeLocalisation(string language)
        {
            _localisationDataLoad.Load(language);

            var localisation = _localisationDataLoad.GetUILocalisation();

            _localizerUI.Localize(localisation);
        }

        private void InitializedServices()
        {
            _uiFactoryInfo = ServicesContainer.GetService<IUIFactoryInfoService>();
            _localisationDataLoad = ServicesContainer.GetService<ILocalisationDataLoadService>();
            _saveLoadData = ServicesContainer.GetService<ISaveLoadDataService>();
            _uiFactoryInfo = ServicesContainer.GetService<IUIFactoryInfoService>();
            _localizerUI = ServicesContainer.GetService<ILocalizerUIService>();
            _sounds = ServicesContainer.GetService<ISoundsService>();
        }
    }
}