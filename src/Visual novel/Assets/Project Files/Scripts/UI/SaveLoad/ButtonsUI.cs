#region

using System;
using TMPro;
using Unit.Tools.Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#endregion

namespace UI.SaveLoad
{
    public class ButtonsUI : MonoBehaviour
    {
        public Action OnButtonClicked;
        
        [SerializeField] private Button _backButton;
        [SerializeField] private TextMeshProUGUI _backButtonText;

        private void Awake()
        {
            _backButton.RegisterNewCallback(() => OnButtonClicked?.Invoke());
        }

        public void SetBackButtonText(string text)
        {
            _backButtonText.text = text;
        }
    }
}