#region

using TMPro;
using Unit.Tools.Extensions;
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

        public void RegisterBackButtonCallback(UnityAction action)
        {
            _backButton.RegisterNewCallback(action);
        }

        public void RegisterSaveButtonCallback(UnityAction action)
        {
            _saveButton.RegisterNewCallback(action);
        }

        public void RegisterLoadButtonCallback(UnityAction action)
        {
            _loadButton.RegisterNewCallback(action);
        }

        public void RegisterHistoryButtonCallback(UnityAction action)
        {
            _historyButton.RegisterNewCallback(action);
        }

        public void RegisterSpeedUpButtonCallback(UnityAction action)
        {
            _speedUpButton.RegisterNewCallback(action);
        }

        public void RegisterAutoButtonCallback(UnityAction action)
        {
            _autoButton.RegisterNewCallback(action);
        }

        public void RegisterFurtherButtonCallback(UnityAction action)
        {
            _furtherButton.RegisterNewCallback(action);
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