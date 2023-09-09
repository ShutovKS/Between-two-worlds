#region

using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

#endregion

namespace UI.Settings
{
	public class LanguageSettingUI : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _nameText;
		[SerializeField] private TMP_Dropdown _dropdown;

		public void SetLanguageName(string text)
		{
			_nameText.text = text;
		}

		public void SetLanguagesToDropdown(params string[] languages)
		{
			_dropdown.ClearOptions();
			_dropdown.AddOptions(languages.ToList());
		}

		public void SetCurrentLanguageToDropdown(int value)
		{
			_dropdown.value = value;
		}

		public void RegisterLanguageChangeCallback(UnityAction<int> onChangeNumber)
		{
			_dropdown.onValueChanged.RemoveAllListeners();
			_dropdown.onValueChanged.AddListener(onChangeNumber);
		}
	}
}