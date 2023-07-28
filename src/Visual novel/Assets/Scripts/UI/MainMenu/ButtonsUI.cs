﻿using TMPro;
using Units.Tools;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI.MainMenu
{
    public class ButtonsUI : MonoBehaviour
    {
        [SerializeField] private Button _loadGameButton;
        [SerializeField] private Button _startGameButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _exitButton;

        [SerializeField] private TextMeshProUGUI _loadGameButtonText;
        [SerializeField] private TextMeshProUGUI _startGameButtonText;
        [SerializeField] private TextMeshProUGUI _settingsButtonText;
        [SerializeField] private TextMeshProUGUI _exitButtonText;

        public void RegisterLoadGameButtonCallback(UnityAction action) => _loadGameButton.RegisterNewCallback(action);
        public void RegisterStartGameButtonCallback(UnityAction action) => _startGameButton.RegisterNewCallback(action);
        public void RegisterSettingsButtonCallback(UnityAction action) => _settingsButton.RegisterNewCallback(action);
        public void RegisterExitButtonCallback(UnityAction action) => _exitButton.RegisterNewCallback(action);

        public void SetLoadGameButton(string text) => _loadGameButtonText.text = text;
        public void SetStartGameButton(string text) => _startGameButtonText.text = text;
        public void SetSettingsButton(string text) => _settingsButtonText.text = text;
        public void SetExitButton(string text) => _exitButtonText.text = text;
    }
}