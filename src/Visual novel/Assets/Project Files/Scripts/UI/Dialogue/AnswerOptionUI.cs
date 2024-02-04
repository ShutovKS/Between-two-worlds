#region

using TMPro;
using Unit.Tools.Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#endregion

namespace UI.Dialogue
{
	public class AnswerOptionUI : MonoBehaviour
	{
		[SerializeField] private Button _answerButton;
		[SerializeField] private TextMeshProUGUI _answerText;

		public void SetAnswerOption(string text, UnityAction action)
		{
			_answerText.text = text;
			_answerButton.RegisterNewCallback(action);
		}
	}
}