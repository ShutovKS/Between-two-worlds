using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace UI.Settings
{
    public class SettingsUI : MonoBehaviour
    {
        [SerializeField] private GameObject _settingsScreenGO;

        [SerializeField] private Back _back;
        [SerializeField] private Language _language;

        public void SetUpBack(string textButton, UnityAction onBack)
        {
            _back.BackButtonText.text = textButton;
            _back.BackButton.onClick.AddListener(onBack);
        }

        public void SetUpLanguage(string nameCategories, string[] languages, UnityAction<string> onLanguageChanged)
        {
            _language.Name.text = nameCategories;
            _language.Dropdown.AddOptions(languages.ToList());
            _language.Dropdown.onValueChanged.RemoveAllListeners();
            _language.Dropdown.onValueChanged.AddListener(number => onLanguageChanged?.Invoke(languages[number]));
        }
        
        public void SetActivePanel(bool value) => _settingsScreenGO.SetActive(value);
    }
}