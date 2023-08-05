using System.Collections.Generic;
using System.Linq;
using Infrastructure.Services.LocalisationDataLoad;
using UI.Settings;
using UnityEngine.Events;

namespace Infrastructure.ScenesManagers.Meta
{
    public class Settings
    {
        public Settings(SettingsUI settingsUI, ILocalisationDataLoadService localisationDataLoad,
            UnityAction<string> onChangeLocalisation, UnityAction onBack)
        {
            _settingsUI = settingsUI;
            _onChangeLocalisation = onChangeLocalisation;
            _onBack = onBack;

            _languages = localisationDataLoad.GetLocalizationsInfo().Select(l => l.Language).ToList();
            _currentLanguage = localisationDataLoad.CurrentLanguage;

            InitialiseSetting();
        }

        private readonly SettingsUI _settingsUI;
        private readonly List<string> _languages;
        private readonly UnityAction _onBack;
        private readonly UnityAction<string> _onChangeLocalisation;

        private string _currentLanguage;

        public void OpenSetting()
        {
            _settingsUI.SetActivePanel(true);
        }

        public void ClosedSetting()
        {
            _settingsUI.SetActivePanel(false);
        }


        private void InitialiseSetting()
        {
            InitialiseSettingLanguage();
        }

        #region InitialiseSettings

        private void InitialiseSettingLanguage()
        {
            _settingsUI.LanguageSetting.SetLanguagesToDropdown(_languages.ToArray());
            _settingsUI.LanguageSetting.RegisterLanguageChangeCallback(
                index =>
                {
                    var language = _languages[index];
                    _onChangeLocalisation(language);
                    _currentLanguage = language;
                });

            _settingsUI.LanguageSetting.SetCurrentLanguageToDropdown(
                _languages.IndexOf(_currentLanguage));

            _settingsUI.BackButton.RegisterBackButtonClickCallback(_onBack);
        }

        #endregion
    }
}