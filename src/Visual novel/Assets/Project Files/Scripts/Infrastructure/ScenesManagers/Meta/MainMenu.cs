#region

using UI.MainMenu;
using UnityEngine.Events;

#endregion

namespace Infrastructure.ScenesManagers.Meta
{
    public class MainMenu
    {
        public MainMenu(MainMenuUI mainMenuUI)
        {
            _mainMenuUI = mainMenuUI;
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