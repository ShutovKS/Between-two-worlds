using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace UI.Settings
{
    public class LanguageSettingUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TMP_Dropdown _dropdown;

        public void SetLanguageName(string text) => _nameText.text = text;
        public void AddLanguagesToDropdown(params string[] languages) => _dropdown.AddOptions(languages.ToList());

        public void RegisterLanguageChangeCallback(UnityAction<int> onChangeNumber)
        {
            _dropdown.onValueChanged.RemoveAllListeners();
            _dropdown.onValueChanged.AddListener(onChangeNumber);
        }
    }
}