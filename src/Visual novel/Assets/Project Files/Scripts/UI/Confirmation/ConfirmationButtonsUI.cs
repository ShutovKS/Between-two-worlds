#region

using TMPro;
using Unit.Tools.Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#endregion

namespace UI.Confirmation
{
	public class ConfirmationButtonsUI : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _yesButtonText;
		[SerializeField] private Button _yesButton;
		[SerializeField] private TextMeshProUGUI _noButtonText;
		[SerializeField] private Button _noButton;

		public void SetYesButtonText(string text)
		{
			_yesButtonText.text = text;
		}

		public void SetNoButtonText(string text)
		{
			_noButtonText.text = text;
		}

		public void RegisterYesButtonCallback(UnityAction callback)
		{
			_yesButton.RegisterNewCallback(callback);
		}

		public void RegisterNoButtonCallback(UnityAction callback)
		{
			_noButton.RegisterNewCallback(callback);
		}
	}
}