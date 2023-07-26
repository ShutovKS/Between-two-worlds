using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.MainMenu
{
    public class Buttons : MonoBehaviour
    {
        [field: SerializeField] public Button LoadGameButton { get; private set; }
        [field: SerializeField] public Button StartGameButton { get; private set; }
        [field: SerializeField] public Button SettingsButton { get; private set; }
        [field: SerializeField] public Button ExitButton { get; private set; }

        [field: SerializeField] public TextMeshProUGUI LoadGameButtonText { get; private set; }
        [field: SerializeField] public TextMeshProUGUI StartGameButtonText { get; private set; }
        [field: SerializeField] public TextMeshProUGUI SettingsButtonText { get; private set; }
        [field: SerializeField] public TextMeshProUGUI ExitButtonText { get; private set; }
    }
}