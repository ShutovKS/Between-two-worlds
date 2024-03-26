using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.MainMenu
{
    public class SettingsButtonsUI : MonoBehaviour
    {
        public Action OnLanguageButtonClicked;

        [SerializeField] private Button languageButton;

        private void Awake()
        {
            languageButton.onClick.AddListener(() => OnLanguageButtonClicked?.Invoke());
        }
    }
}