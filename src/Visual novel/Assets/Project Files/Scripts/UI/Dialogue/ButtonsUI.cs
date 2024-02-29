#region

using System;
using TMPro;
using Tools.Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#endregion

namespace UI.Dialogue
{
    public class ButtonsUI : MonoBehaviour
    {
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _saveButton;
        [SerializeField] private Button _loadButton;

        [Space] [SerializeField] private Button _historyButton;

        [SerializeField] private Button _speedUpButton;
        [SerializeField] private Button _autoButton;
        [SerializeField] private Button _furtherButton;

        [Space] [SerializeField] private TextMeshProUGUI _historyButtonText;

        [SerializeField] private TextMeshProUGUI _skipButtonText;
        [SerializeField] private TextMeshProUGUI _autoButtonText;
        [SerializeField] private TextMeshProUGUI _furtherButtonText;
        
        public Action OnBackButtonClicked;
        public Action OnSaveButtonClicked;
        public Action OnLoadButtonClicked;
        public Action OnHistoryButtonClicked;
        public Action OnSpeedUpButtonClicked;
        public Action OnAutoButtonClicked;
        public Action OnFurtherButtonClicked;

        public void Awake()
        {
            _backButton.RegisterNewCallback(() => OnBackButtonClicked?.Invoke());
            _saveButton.RegisterNewCallback(() => OnSaveButtonClicked?.Invoke());
            _loadButton.RegisterNewCallback(() => OnLoadButtonClicked?.Invoke());
            _historyButton.RegisterNewCallback(() => OnHistoryButtonClicked?.Invoke());
            _speedUpButton.RegisterNewCallback(() => OnSpeedUpButtonClicked?.Invoke());
            _autoButton.RegisterNewCallback(() => OnAutoButtonClicked?.Invoke());
            _furtherButton.RegisterNewCallback(() => OnFurtherButtonClicked?.Invoke());
        }

        public void SetHistoryButtonText(string text)
        {
            _historyButtonText.SetText(text);
        }

        public void SetSkipButtonText(string text)
        {
            _skipButtonText.SetText(text);
        }

        public void SetAutoButtonText(string text)
        {
            _autoButtonText.SetText(text);
        }

        public void SetFurtherButtonText(string text)
        {
            _furtherButtonText.SetText(text);
        }
    }
}