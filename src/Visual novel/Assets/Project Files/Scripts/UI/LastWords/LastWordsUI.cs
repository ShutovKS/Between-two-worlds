#region

using Data.Localization.UILocalisation;
using Infrastructure.Services.LocalizationUI;
using TMPro;
using Tools.Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#endregion

namespace UI.LastWords
{
    public class LastWordsUI : MonoBehaviour, ILocalizableUI
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Button _backButton;
        [SerializeField] private Canvas canvas;

        public void SetText(string text)
        {
            _text.text = text;
        }

        public void RegisterBackButtonCallback(UnityAction callback)
        {
            _backButton.RegisterNewCallback(callback);
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