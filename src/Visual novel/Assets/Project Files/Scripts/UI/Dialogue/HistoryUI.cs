#region

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#endregion

namespace UI.Dialogue
{
    public class HistoryUI : MonoBehaviour
    {
        public Action OnBackButtonClicked;

        [SerializeField] private GameObject _historyPhrasePrefab;
        [SerializeField] private Transform _contentTransform;
        [SerializeField] private GameObject _historyGameObject;
        [SerializeField] private Button _backButton;

        private readonly Dictionary<string, GameObject> _historyPhrases = new();

        public void Awake()
        {
            _backButton.onClick.AddListener(() => OnBackButtonClicked?.Invoke());
        }

        public void SetActivePanel(bool value)
        {
            _historyGameObject.SetActive(value);
        }
        
        public void CreateHistoryPhrase(string id, string name, string text)
        {
            var historyPhraseInstantiate = Instantiate(_historyPhrasePrefab, _contentTransform);
            historyPhraseInstantiate.SetActive(true);
            _historyPhrases.Add(id, historyPhraseInstantiate);

            if (historyPhraseInstantiate.TryGetComponent(out HistoryPhraseUI historyPhraseUI))
            {
                historyPhraseUI.NameText.text = name;
                historyPhraseUI.TextText.text = text;
            }
            else
            {
                throw new Exception("No HistoryPhraseUI in instance historyPhrasePrefab");
            }

            var contentPanelRT = _contentTransform.GetComponent<RectTransform>();
            var panel = historyPhraseInstantiate.GetComponent<RectTransform>();

            var scrollSizeDelta = contentPanelRT.sizeDelta;
            scrollSizeDelta.y += contentPanelRT.childCount == 1
                ? panel.sizeDelta.y
                : panel.sizeDelta.y * 1.5f;

            contentPanelRT.sizeDelta = scrollSizeDelta;

            var panelAnchoredPosition = panel.anchoredPosition;
            panelAnchoredPosition.y = -scrollSizeDelta.y + panel.sizeDelta.y * 0.5f;
            panel.anchoredPosition = panelAnchoredPosition;
        }

        public void ClearHistory()
        {
            foreach (var historyPhrase in _historyPhrases)
            {
                Destroy(historyPhrase.Value);
            }

            _historyPhrases.Clear();
        }

        public void DestroyHistoryPhrase(string id)
        {
            if (_historyPhrases.TryGetValue(id, out var go))
            {
                Destroy(go);
                _historyPhrases.Remove(id);
            }
            else
            {
                throw new Exception($"No id {id} in dictionary history phrases");
            }
        }

        public IEnumerable<string> GetHistoryPhrasesId()
        {
            return _historyPhrases.Keys;
        }
    }
}