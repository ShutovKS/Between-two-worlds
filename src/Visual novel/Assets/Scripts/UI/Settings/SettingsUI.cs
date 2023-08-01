using Data.Localization.UILocalisation;
using UnityEngine;

namespace UI.Settings
{
    public class SettingsUI : MonoBehaviour
    {
        [SerializeField] private GameObject _settingsScreenGameObject;

        [field: SerializeField] public BackButtonUI BackButton { get; private set; }
        [field: SerializeField] public LanguageSettingUI LanguageSetting { get; private set; }

        public void SetActivePanel(bool value) => _settingsScreenGameObject.SetActive(value);

        public void Localisator(UILocalisation localisation)
        {
            BackButton.SetBackButtonText(localisation.Back);
            
            LanguageSetting.SetLanguageName(localisation.Language);
        }
    }
}