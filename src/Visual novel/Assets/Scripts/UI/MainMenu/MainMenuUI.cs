using UnityEngine;
using UnityEngine.Events;
using static Units.Tools.Tools;

namespace UI.MainMenu
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private GameObject _mainMenuScreenGO;
        
        [SerializeField] private Background _background;
        [SerializeField] private Buttons _buttons;
        [SerializeField] private GameName _gameName;
        
        public void SetBackground(Texture2D texture2D) => _background.BackgroundImage.sprite = texture2D.ToSprite();

        public void SetLoadGameButton(UnityAction action)
        {
            _buttons.LoadGameButton.onClick.RemoveAllListeners();
            _buttons.LoadGameButton.onClick.AddListener(action);
        }

        public void SetStartGameButton(UnityAction action)
        {
            _buttons.StartGameButton.onClick.RemoveAllListeners();
            _buttons.StartGameButton.onClick.AddListener(action);
        }

        public void SetSettingsButton(UnityAction action)
        {
            _buttons.SettingsButton.onClick.RemoveAllListeners();
            _buttons.SettingsButton.onClick.AddListener(action);
        }

        public void SetExitButton(UnityAction action)
        {
            _buttons.ExitButton.onClick.RemoveAllListeners();
            _buttons.ExitButton.onClick.AddListener(action);
        }

        public void SetActivePanel(bool value) => _mainMenuScreenGO.SetActive(value);

        public void SetLoadGameButton(string text) => _buttons.LoadGameButtonText.text = text;
        public void SetStartGameButton(string text) => _buttons.StartGameButtonText.text = text;
        public void SetSettingsButton(string text) => _buttons.SettingsButtonText.text = text;
        public void SetExitButton(string text) => _buttons.ExitButtonText.text = text;

        public void SetGameName(string text) => _gameName.GameNameText.text = text;
    }
}