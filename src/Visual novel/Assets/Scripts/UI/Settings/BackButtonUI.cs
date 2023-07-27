using TMPro;
using Units.Tools;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI.Settings
{
    public class BackButtonUI : MonoBehaviour
    {
        [SerializeField] private Button _backButton;
        [SerializeField] private TextMeshProUGUI _backButtonText;

        public void SetBackButtonText(string text) => _backButtonText.text = text;
        public void RegisterBackButtonClickCallback(UnityAction onBack) => _backButton.RegisterNewCallback(onBack);
    }
}