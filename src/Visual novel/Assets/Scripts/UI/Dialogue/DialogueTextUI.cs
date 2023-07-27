using TMPro;
using Units.Tools;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI.Dialogue
{
    public class DialogueTextUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _authorNameText;
        [SerializeField] private TextMeshProUGUI _textText;
        [SerializeField] private Button _furtherButton;
        [SerializeField] private TextMeshProUGUI _furtherText;

        public void SetAuthorName(string authorName) => _authorNameText.text = authorName;
        public void SetText(string text) => _textText.text = text;
        public void RegisterFurtherButtonCallback(UnityAction action) => _furtherButton.RegisterNewCallback(action);
        public void SetBackButtonText(string text) => _furtherText.text = text;
    }
}