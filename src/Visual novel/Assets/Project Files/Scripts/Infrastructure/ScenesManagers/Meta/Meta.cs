#region

using Data.Dynamic;
using Data.Static;
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
        private ILocalisationDataLoadService _localisationDataLoad;
        private IUIFactoryInfoService _uiFactoryInfoService;
        private ILocalizerUI _localizerUI;
        private MainMenu _menu;
        private ISaveLoadDataService _saveLoadData;
        private IUIFactoryInfoService _uiFactoryInfo;
        private ISoundsService _soundsService;

        private void Start()
        {
            InitializedServices();

            _uiFactoryInfoService.BackgroundUI.SetBackgroundImage(Resources.Load<Texture2D>("Data/Backgrounds/" + Constant.BACKGROUND_PATH));
            _menu = new MainMenu(_uiFactoryInfo.MainMenuUI, LoadGame, StartGame, Exit);
            _soundsService.SetClip(Constant.SOUND_MAIN_MENU, true);

            OpenMenu();
        }

        private void StartGame()
        {
            PlayerPrefs.SetString(Constant.KEY_ID_DIALOGUE_FOR_PLAYER_PREFS, Constant.DIALOG_START_ID);
            _menu.ClosedMenu();
            SceneManager.LoadScene("3.Core");
        }

        private void LoadGame()
        {
            _uiFactoryInfo.SaveLoadUI.SetActivePanel(true);
            _uiFactoryInfo.SaveLoadUI.ButtonsUI.RegisterBackButtonCallback(
                () => { _uiFactoryInfo.SaveLoadUI.SetActivePanel(false); });

            var number = 0;
            var datas = _saveLoadData.Load();

            foreach (var ui in _uiFactoryInfo.SaveLoadUI.SaveDataUIs)
            {
                var data = datas.dialogues[number++];

                if (!data.isDataExist) continue;

                ui.SetImage(data.background);
                ui.SetTitle(data.titleText);
                ui.RegisterButtonCallback(
                    () =>
                    {
                        PlayerPrefs.SetString(Constant.KEY_ID_DIALOGUE_FOR_PLAYER_PREFS, data.idLastDialogue);
                        _uiFactoryInfo.SaveLoadUI.SetActivePanel(false);
                        _menu.ClosedMenu();
                        SceneManager.LoadScene("3.Core");
                    });
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
            _uiFactoryInfoService = ServicesContainer.GetService<IUIFactoryInfoService>();
            _localisationDataLoad = ServicesContainer.GetService<ILocalisationDataLoadService>();
            _saveLoadData = ServicesContainer.GetService<ISaveLoadDataService>();
            _uiFactoryInfo = ServicesContainer.GetService<IUIFactoryInfoService>();
            _localizerUI = ServicesContainer.GetService<ILocalizerUI>();
            _soundsService = ServicesContainer.GetService<ISoundsService>();
        }
    }
}
