using UnityEngine;
using UnityEngine.Events;

namespace UI.MainMenu
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private Background _background;
        [SerializeField] private Buttons _buttons;
        [SerializeField] private GameName _gameName;

        public void SetImageForBackground(Texture2D texture2D) => _background.BackgroundImage.sprite = Sprite.Create(
            texture2D,
            new Rect(0, 0, texture2D.width, texture2D.height),
            new Vector2(0.5f, 0.5f));

        public void SetActionForLoadGameButton(UnityAction action)
        {
            _buttons.LoadGameButton.onClick.RemoveAllListeners();
            _buttons.LoadGameButton.onClick.AddListener(action);
        }

        public void SetActionForStartGameButton(UnityAction action)
        {
            _buttons.StartGameButton.onClick.RemoveAllListeners();
            _buttons.StartGameButton.onClick.AddListener(action);
        }

        public void SetActionForSettingsButton(UnityAction action)
        {
            _buttons.SettingsButton.onClick.RemoveAllListeners();
            _buttons.SettingsButton.onClick.AddListener(action);
        }

        public void SetActionForExitButton(UnityAction action)
        {
            _buttons.ExitButton.onClick.RemoveAllListeners();
            _buttons.ExitButton.onClick.AddListener(action);
        }

        public void SetTextForLoadGameButton(string text) => _buttons.LoadGameButtonText.text = text;
        public void SetTextForStartGameButton(string text) => _buttons.StartGameButtonText.text = text;
        public void SetTextForSettingsButton(string text) => _buttons.SettingsButtonText.text = text;
        public void SetTextForExitButton(string text) => _buttons.ExitButtonText.text = text;

        public void SetTextForGameName(string text) => _gameName.GameNameText.text = text;

        public void RemoveBackgroundImage() => _background.BackgroundImage.sprite = null;
    }
}