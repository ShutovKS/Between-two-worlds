using TMPro;
using UnityEngine;

namespace UI.Dialogue
{
    public class DialogueTextUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _authorNameText;
        [SerializeField] private TextMeshProUGUI _textText;

        public void SetAuthorName(string authorName) => _authorNameText.text = authorName;
        public void SetText(string text) => _textText.text = text;
    }
}