using TMPro;
using Units.Tools;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI.Dialogue
{
    public class AnswerOptionsUI : MonoBehaviour
    {
        [SerializeField] private Button _answerButton1;
        [SerializeField] private Button _answerButton2;
        [SerializeField] private Button _answerButton3;

        [SerializeField] private TextMeshProUGUI _answerText1;
        [SerializeField] private TextMeshProUGUI _answerText2;
        [SerializeField] private TextMeshProUGUI _answerText3;
        
        [SerializeField] private GameObject _answerOptionsGameObject;

        public void SetAnswerOptions(
            (string text, UnityAction action) answer1,
            (string text, UnityAction action) answer2,
            (string text, UnityAction action) answer3)
        {
            _answerText1.text = answer1.text;
            _answerButton1.RegisterNewCallback(answer1.action);

            _answerText2.text = answer2.text;
            _answerButton2.RegisterNewCallback(answer2.action);

            _answerText3.text = answer3.text;
            _answerButton3.RegisterNewCallback(answer3.action);
        }

        public void SetActiveAnswerOptions(bool value) => _answerOptionsGameObject.SetActive(value);
    }
}