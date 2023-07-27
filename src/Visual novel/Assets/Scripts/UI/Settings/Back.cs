using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Settings
{
    public class Back : MonoBehaviour
    {
        [field: SerializeField] public Button BackButton { get; private set; }
        [field: SerializeField] public TextMeshProUGUI BackButtonText { get; private set; }
    }
}