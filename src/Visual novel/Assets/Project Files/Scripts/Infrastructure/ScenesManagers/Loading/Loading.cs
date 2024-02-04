#region

using System;
using System.Threading.Tasks;
using Infrastructure.Services;
using Infrastructure.Services.LocalisationDataLoad;
using Infrastructure.Services.LocalizationUI;
using Infrastructure.Services.UIFactory;
using UnityEngine;
using UnityEngine.SceneManagement;

#endregion

namespace Infrastructure.ScenesManagers.Loading
{
    public class Loading : MonoBehaviour
    {
        private string _language = "";
        private ILocalisationDataLoadService _localisationDataLoad;
        private ILocalizerUI _localizerUI;
        private IUIFactoryService _uiFactory;
        private IUIFactoryInfoService _uiFactoryInfo;

        private async void Start()
        {
            InitializedServices();
            await CreatedUI();
            RegisterLocalizableUI();
            LoadData(() =>
            {
                LocalisationUI();
                StartGame();
            });
        }

        private void InitializedServices()
        {
            _localisationDataLoad = ServicesContainer.GetService<ILocalisationDataLoadService>();
            _uiFactoryInfo = ServicesContainer.GetService<IUIFactoryInfoService>();
            _uiFactory = ServicesContainer.GetService<IUIFactoryService>();
            _localizerUI = ServicesContainer.GetService<ILocalizerUI>();
        }

        private async Task CreatedUI()
        {
            await _uiFactory.CreatedBackgroundScreen();
            _uiFactoryInfo.BackgroundUI.SetBackgroundColor(Color.black);

            await _uiFactory.CreatedChooseLanguageScreen();
            _uiFactoryInfo.ChooseLanguageUI.SetActivePanel(true);

            await _uiFactory.CreatedDialogueScreen();
            _uiFactoryInfo.DialogueUI.SetActivePanel(false);

            await _uiFactory.CreatedMainMenuScreen();
            _uiFactoryInfo.MainMenuUI.SetActivePanel(false);

            await _uiFactory.CreatedConfirmationScreen();
            _uiFactoryInfo.ConfirmationUI.SetActivePanel(false);

            await _uiFactory.CreatedSaveLoadScreen();
            _uiFactoryInfo.SaveLoadUI.SetActivePanel(false);

            await _uiFactory.CreatedLastWordsScreen();
            _uiFactoryInfo.LastWordsUI.SetActivePanel(false);
        }

        private void RegisterLocalizableUI()
        {
            _localizerUI.Register(_uiFactoryInfo.DialogueUI);
            _localizerUI.Register(_uiFactoryInfo.MainMenuUI);
            _localizerUI.Register(_uiFactoryInfo.ConfirmationUI);
            _localizerUI.Register(_uiFactoryInfo.SaveLoadUI);
        }

        private void LoadData(Action onCompleted)
        {
            var localizationsInfo = _localisationDataLoad.GetLocalizationsInfo();

            foreach (var localizationInfo in localizationsInfo)
            {
                _uiFactoryInfo.ChooseLanguageUI.ScrollViewLanguages.AddLanguageInScrollView(
                    localizationInfo.Language,
                    localizationInfo.FlagImage,
                    () =>
                    {
                        _language = localizationInfo.Language;
                        _localisationDataLoad.Load(_language);
                        onCompleted?.Invoke();
                    });
            }
        }

        private void LocalisationUI()
        {
            var uiLocalisation = _localisationDataLoad.GetUILocalisation();
            _localizerUI.Localize(uiLocalisation);
        }

        private void StartGame()
        {
            SceneManager.LoadScene("2.Meta");
            _uiFactoryInfo.ChooseLanguageUI.SetActivePanel(false);
        }
    }
}