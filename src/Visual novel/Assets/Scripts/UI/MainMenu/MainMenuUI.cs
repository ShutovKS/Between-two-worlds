using UnityEngine;

namespace UI.MainMenu
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private GameObject mainMenuScreenGameObject;

        [field: SerializeField] public ButtonsUI Buttons { get; private set; }
        [field: SerializeField] public GameNameUI GameName { get; private set; }

        public void SetActivePanel(bool value) => mainMenuScreenGameObject.SetActive(value);
    }
}