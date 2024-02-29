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
        public Action OnStartGameButtonClicked;
        public Action OnExitButtonClicked;

        [SerializeField] private Button _loadGameButton;
        [SerializeField] private Button _startGameButton;
        [SerializeField] private Button _exitButton;

        [SerializeField] private TextMeshProUGUI _loadGameButtonText;
        [SerializeField] private TextMeshProUGUI _startGameButtonText;
        [SerializeField] private TextMeshProUGUI _exitButtonText;

        private void Awake()
        {
            _loadGameButton.onClick.AddListener(() => OnLoadGameButtonClicked?.Invoke());
            _startGameButton.onClick.AddListener(() => OnStartGameButtonClicked?.Invoke());
            _exitButton.onClick.AddListener(() => OnExitButtonClicked?.Invoke());


#if UNITY_WEBGL
            _exitButton.gameObject.SetActive(false);
#endif
        }

        public void SetLoadGameButton(string text)
        {
            _loadGameButtonText.text = text;
        }

        public void SetStartGameButton(string text)
        {
            _startGameButtonText.text = text;
        }

        public void SetExitButton(string text)
        {
            _exitButtonText.text = text;
        }
    }
}