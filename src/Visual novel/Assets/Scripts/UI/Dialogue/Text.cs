using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Dialogue
{
    public class Text : MonoBehaviour
    {
        [field: SerializeField] public TextMeshProUGUI AuthorNameText { get; private set; }
        [field: SerializeField] public TextMeshProUGUI TextText { get; private set; }
        [field: SerializeField] public Button FurtherButton { get; private set; }
    }
}