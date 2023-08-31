using TMPro;
using UnityEngine;

namespace UI.Confirmation
{
    public class ConfirmationTextUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _titleText;

        public void SetTitleText(string text) => _titleText.text = text;
    }
}