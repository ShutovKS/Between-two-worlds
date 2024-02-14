#region

using UI.MainMenu;
using UnityEngine.Events;

#endregion

namespace Infrastructure.ScenesManagers.Meta
{
    public class MainMenu
    {
        public MainMenu(MainMenuUI mainMenuUI, UnityAction loadGame, UnityAction startGame, UnityAction exit)
        {
            _mainMenuUI = mainMenuUI;

            _mainMenuUI.Buttons.RegisterLoadGameButtonCallback(loadGame);
            _mainMenuUI.Buttons.RegisterStartGameButtonCallback(startGame);
            _mainMenuUI.Buttons.RegisterExitButtonCallback(exit);
        }

        private readonly MainMenuUI _mainMenuUI;

        public void OpenMenu()
        {
            _mainMenuUI.SetActivePanel(true);
        }

        public void ClosedMenu()
        {
            _mainMenuUI.SetActivePanel(false);
        }
    }
}