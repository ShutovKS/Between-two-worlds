using Data.Dynamic;
using Infrastructure.Services;
using Infrastructure.Services.LocalisationDataLoad;
using Infrastructure.Services.LocalizationUI;
using Infrastructure.Services.SaveLoadData;
using Infrastructure.Services.UIFactory;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Infrastructure.ScenesManagers.Meta
{
    public class Meta : MonoBehaviour
    {
        private ILocalisationDataLoadService _localisationDataLoad;
        private ISaveLoadDataService _saveLoadData;
        private IUIFactoryInfoService _uiFactoryInfo;
        private ILocalizerUI _localizerUI;

        private Settings _settings;
        private MainMenu _menu;

        private void Awake()
        {
            InitializedServices();

            _menu = new MainMenu(_uiFactoryInfo.MainMenuUI, LoadGame, StartGame, OpenSetting, Exit);
            _settings = new Settings(_uiFactoryInfo.SettingsUI, _localisationDataLoad, ChangeLocalisation, OpenMenu);

            OpenMenu();
        }

        private void StartGame()
        {
            var data = new DynamicData();
            _saveLoadData.Save(data);

            _menu.ClosedMenu();
            SceneManager.LoadScene("3.Core");
        }

        private void LoadGame()
        {
            _menu.ClosedMenu();
            SceneManager.LoadScene("3.Core");
        }

        private void OpenMenu()
        {
            _settings.ClosedSetting();
            _menu.OpenMenu();
        }

        private void OpenSetting()
        {
            _menu.ClosedMenu();
            _settings.OpenSetting();
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
            _localisationDataLoad = ServicesContainer.GetService<ILocalisationDataLoadService>();
            _saveLoadData = ServicesContainer.GetService<ISaveLoadDataService>();
            _uiFactoryInfo = ServicesContainer.GetService<IUIFactoryInfoService>();
            _localizerUI = ServicesContainer.GetService<ILocalizerUI>();
        }
    }
}