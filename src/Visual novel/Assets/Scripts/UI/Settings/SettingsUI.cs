#region

using Data.Localization.UILocalisation;
using Infrastructure.Services.LocalizationUI;
using UnityEngine;

#endregion

namespace UI.Settings
{
	public class SettingsUI : MonoBehaviour, ILocalizableUI
	{
		[SerializeField] private GameObject _settingsScreenGameObject;

		[field: SerializeField] public BackButtonUI BackButton { get; private set; }
		[field: SerializeField] public LanguageSettingUI LanguageSetting { get; private set; }

		public void Localize(UILocalisation localisation)
		{
			// BackButton.SetBackButtonText(localisation.BackButton);

			LanguageSetting.SetLanguageName(localisation.Language);
		}

		public void SetActivePanel(bool value)
		{
			_settingsScreenGameObject.SetActive(value);
		}
	}
}