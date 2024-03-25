#region

using System;
using Data.Static.UILocalisation;
using Infrastructure.Services.LocalizationUI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#endregion

namespace UI.LastWords
{
    public class LastWordsUI : BaseScreen, ILocalizableUI
    {
        public Action OnBackButtonClicked;
        
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Button _backButton;
        [SerializeField] private Canvas canvas;
        
        private void Awake()
        {
            _backButton.onClick.AddListener(() => OnBackButtonClicked?.Invoke());
        }

        public void SetText(string text)
        {
            _text.text = text;
        }
        
        public void Localize(UILocalisation localisation)
        {
            _backButton.GetComponentInChildren<TextMeshProUGUI>().text = localisation.BackButton;
        }

        public void SetActivePanel(bool isActive)
        {
            canvas.enabled = isActive;
        }
    }
}