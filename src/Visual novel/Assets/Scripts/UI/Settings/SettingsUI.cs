using Data.Localization.UILocalisation;
using Infrastructure.Services.LocalizationUI;
using UnityEngine;

namespace UI.Settings
{
    public class SettingsUI : MonoBehaviour, ILocalizableUI
    {
        [SerializeField] private GameObject _settingsScreenGameObject;

        [field: SerializeField] public BackButtonUI BackButton { get; private set; }
        [field: SerializeField] public LanguageSettingUI LanguageSetting { get; private set; }

        public void SetActivePanel(bool value) => _settingsScreenGameObject.SetActive(value);

        public void Localize(UILocalisation localisation)
        {
            // BackButton.SetBackButtonText(localisation.BackButton);
            
            LanguageSetting.SetLanguageName(localisation.Language);
        }
    }
}