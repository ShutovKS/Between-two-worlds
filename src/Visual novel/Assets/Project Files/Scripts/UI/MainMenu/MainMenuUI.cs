#region

using Data.Localization.UILocalisation;
using Features.UI.Scripts.Base;
using Infrastructure.Services.LocalizationUI;
using UnityEngine;

#endregion

namespace UI.MainMenu
{
    public class MainMenuUI : BaseScreen, ILocalizableUI
    {
        [SerializeField] private Canvas canvas;

        [field: SerializeField] public ButtonsUI Buttons { get; private set; }
        [field: SerializeField] public SettingsButtonsUI SettingsButtons { get; private set; }
        [field: SerializeField] public GameNameUI GameName { get; private set; }

        public void Localize(UILocalisation localisation)
        {
            Buttons.SetLoadGameButton(localisation.LoadGameButton);
            Buttons.SetStartNewGameButton(localisation.StartNewGameButton);
            Buttons.SetContinueGameButton(localisation.ContinueGameButton);
            Buttons.SetExitButton(localisation.ExitButton);

            GameName.SetGameName(localisation.GameName);
        }

        public void SetActivePanel(bool value)
        {
            canvas.enabled = value;
        }
    }
}