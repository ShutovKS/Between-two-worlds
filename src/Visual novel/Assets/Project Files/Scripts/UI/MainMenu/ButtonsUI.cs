#region

using System;
using TMPro;
using Tools.Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#endregion

namespace UI.MainMenu
{
    public class ButtonsUI : MonoBehaviour
    {
        public Action OnLoadGameButtonClicked;
        public Action OnStartNewGameButtonClicked;
        public Action OnContinueGameButtonClicked;
        public Action OnExitButtonClicked;

        [SerializeField] private Button _loadGameButton;
        [SerializeField] private Button _startNewGameButton;
        [SerializeField] private Button _continueGameButton;
        [SerializeField] private Button _exitButton;

        [SerializeField] private TextMeshProUGUI _loadGameButtonText;
        [SerializeField] private TextMeshProUGUI _startNewGameButtonText;
        [SerializeField] private TextMeshProUGUI _continueGameButtonText;
        [SerializeField] private TextMeshProUGUI _exitButtonText;

        private void Awake()
        {
            _loadGameButton.onClick.AddListener(() => OnLoadGameButtonClicked?.Invoke());
            _startNewGameButton.onClick.AddListener(() => OnStartNewGameButtonClicked?.Invoke());
            _continueGameButton.onClick.AddListener(() => OnContinueGameButtonClicked?.Invoke());
            _exitButton.onClick.AddListener(() => OnExitButtonClicked?.Invoke());

#if UNITY_WEBGL
            _exitButton.gameObject.SetActive(false);
#endif
        }

        public void SetLoadGameButton(string text)
        {
            _loadGameButtonText.text = text;
        }

        public void SetStartNewGameButton(string text)
        {
            _startNewGameButtonText.text = text;
        }

        public void SetContinueGameButton(string text)
        {
            _continueGameButtonText.text = text;
        }

        public void SetExitButton(string text)
        {
            _exitButtonText.text = text;
        }
    }
}