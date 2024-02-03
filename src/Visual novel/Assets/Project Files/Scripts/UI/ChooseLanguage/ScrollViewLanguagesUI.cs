#region

using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#endregion

namespace UI.ChooseLanguage
{
	public class ScrollViewLanguagesUI : MonoBehaviour
	{
		[SerializeField] private Transform _contentTransform;
		[SerializeField] private GameObject _languagePrefab;

		public void AddLanguageInScrollView(string languageName, UnityAction onClick)
		{
			var instance = Instantiate(_languagePrefab, _contentTransform);
			instance.SetActive(true);
			instance.GetComponent<Button>().onClick.AddListener(onClick);
			instance.GetComponentInChildren<TextMeshProUGUI>().text = languageName;

			var instanceRectTransform = instance.GetComponent<RectTransform>();
			var contentRectTransform = _contentTransform.GetComponent<RectTransform>();

			var scrollSizeDelta = contentRectTransform.sizeDelta;
			scrollSizeDelta.y += instanceRectTransform.sizeDelta.y;
			contentRectTransform.sizeDelta = scrollSizeDelta;

			var anchoredPosition = instanceRectTransform.anchoredPosition;
			anchoredPosition.y = -scrollSizeDelta.y + instanceRectTransform.sizeDelta.y * 0.5f;
			instanceRectTransform.anchoredPosition = anchoredPosition;
		}

		public void RemoveAllLanguagesInScrollView()
		{
			foreach (Object variable in _contentTransform)
			{
				Destroy(variable);
			}
		}
	}
}