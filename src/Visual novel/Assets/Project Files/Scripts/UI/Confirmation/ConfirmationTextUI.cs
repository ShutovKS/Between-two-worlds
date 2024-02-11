#region

using TMPro;
using UnityEngine;

#endregion

namespace UI.Confirmation
{
    public class ConfirmationTextUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _titleText;

        public void SetTitleText(string text)
        {
            _titleText.text = text;
        }
    }
}