using TMPro;
using UnityEngine;

namespace UI.Settings
{
    public class Language : MonoBehaviour
    {
        [field: SerializeField] public TextMeshProUGUI Name;
        [field: SerializeField] public TMP_Dropdown Dropdown;
    }
}