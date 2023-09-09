#region

using Data.Localization.UILocalisation;
using Infrastructure.Services.LocalizationUI;
using UnityEngine;

#endregion

namespace UI.SaveLoad
{
	public class SaveLoadUI : MonoBehaviour, ILocalizableUI
	{
		[SerializeField] private GameObject _saveLoadScreenGameObject;
		[field: SerializeField] public ButtonsUI ButtonsUI { get; private set; }
		[field: SerializeField] public SaveDataUI[] SaveDataUIs { get; private set; }

		public void Localize(UILocalisation localisation)
		{
			// ButtonsUI.SetBackButtonText(localisation.BackButton);
		}

		public void SetActivePanel(bool isActive)
		{
			_saveLoadScreenGameObject.SetActive(isActive);
		}
	}
}