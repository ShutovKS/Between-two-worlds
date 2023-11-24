#region

using Data.Localization.UILocalisation;
using Infrastructure.Services.LocalizationUI;
using UnityEngine;

#endregion

namespace UI.MainMenu
{
	public class MainMenuUI : MonoBehaviour, ILocalizableUI
	{
		[SerializeField] private GameObject mainMenuScreenGameObject;

		[field: SerializeField] public ButtonsUI Buttons { get; private set; }
		[field: SerializeField] public GameNameUI GameName { get; private set; }

		public void Localize(UILocalisation localisation)
		{
			Buttons.SetLoadGameButton(localisation.LoadGameButton);
			Buttons.SetStartGameButton(localisation.StartGameButton);
			Buttons.SetExitButton(localisation.ExitButton);

			GameName.SetGameName(localisation.GameName);
		}

		public void SetActivePanel(bool value)
		{
			mainMenuScreenGameObject.SetActive(value);
		}
	}
}