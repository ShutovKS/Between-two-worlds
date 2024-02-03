#region

using TMPro;
using Units.Tools;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#endregion

namespace UI.SaveLoad
{
	public class ButtonsUI : MonoBehaviour
	{
		[SerializeField] private Button _backButton;
		[SerializeField] private TextMeshProUGUI _backButtonText;

		public void RegisterBackButtonCallback(UnityAction callback)
		{
			_backButton.RegisterNewCallback(callback);
		}

		public void SetBackButtonText(string text)
		{
			_backButtonText.text = text;
		}
	}
}