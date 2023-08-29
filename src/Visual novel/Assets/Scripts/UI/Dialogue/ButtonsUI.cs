using TMPro;
using Units.Tools;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI.Dialogue
{
    public class ButtonsUI : MonoBehaviour
    {
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _saveButton;
        [SerializeField] private Button _loadButton;
        [SerializeField] private Button _settingsButton;
        [Space] 
        [SerializeField] private Button _historyButton;
        [SerializeField] private Button _skipButton;
        [SerializeField] private Button _autoButton;
        [SerializeField] private Button _furtherButton;
        [Space]
        [SerializeField] private TextMeshProUGUI _historyButtonText;
        [SerializeField] private TextMeshProUGUI _skipButtonText;
        [SerializeField] private TextMeshProUGUI _autoButtonText;
        [SerializeField] private TextMeshProUGUI _furtherButtonText;

        public void RegisterBackButtonCallback(UnityAction action) => _backButton.RegisterNewCallback(action);
        public void RegisterSaveButtonCallback(UnityAction action) => _saveButton.RegisterNewCallback(action);
        public void RegisterLoadButtonCallback(UnityAction action) => _loadButton.RegisterNewCallback(action);
        public void RegisterSettingsButtonCallback(UnityAction action) => _settingsButton.RegisterNewCallback(action);
        
        public void RegisterHistoryButtonCallback(UnityAction action) => _historyButton.RegisterNewCallback(action);
        public void RegisterSkipButtonCallback(UnityAction action) => _skipButton.RegisterNewCallback(action);
        public void RegisterAutoButtonCallback(UnityAction action) => _autoButton.RegisterNewCallback(action);
        public void RegisterFurtherButtonCallback(UnityAction action) => _furtherButton.RegisterNewCallback(action);
        
        public void SetHistoryButtonText(string text) => _historyButtonText.SetText(text);
        public void SetSkipButtonText(string text) => _skipButtonText.SetText(text);
        public void SetAutoButtonText(string text) => _autoButtonText.SetText(text);
        public void SetFurtherButtonText(string text) => _furtherButtonText.SetText(text);
    }
}