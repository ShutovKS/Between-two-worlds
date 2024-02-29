#region

using System;
using TMPro;
using Tools.Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#endregion

namespace UI.Confirmation
{
    public class ConfirmationButtonsUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _yesButtonText;
        [SerializeField] private Button _yesButton;
        [SerializeField] private TextMeshProUGUI _noButtonText;
        [SerializeField] private Button _noButton;
        
        public Action OnYesButtonClicked;
        public Action OnNoButtonClicked;

        public void Awake()
        {
            _yesButton.RegisterNewCallback(() => OnYesButtonClicked?.Invoke());
            _noButton.RegisterNewCallback(() => OnNoButtonClicked?.Invoke());
        }

        public void SetYesButtonText(string text)
        {
            _yesButtonText.text = text;
        }

        public void SetNoButtonText(string text)
        {
            _noButtonText.text = text;
        }
    }
}