using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Dialogue
{
    public class Answers : MonoBehaviour
    {
        [field: SerializeField] public Button AnswerButton1 { get; private set; }
        [field: SerializeField] public Button AnswerButton2 { get; private set; }
        
        [field: SerializeField] public TextMeshProUGUI AnswerText1 { get; private set; }
        [field: SerializeField] public TextMeshProUGUI AnswerText2 { get; private set; }
    }
}