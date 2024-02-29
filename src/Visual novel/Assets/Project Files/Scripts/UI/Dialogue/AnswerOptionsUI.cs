#region

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#endregion

namespace UI.Dialogue
{
    public class AnswerOptionsUI : MonoBehaviour
    {
        [SerializeField] private GameObject _answerOptionUIPrefab;
        [SerializeField] private Transform _answersParentTransform;
        [SerializeField] private GameObject _answerOptionsGameObject;
        
        private readonly List<GameObject> _answerOptionUIGameObjects = new();

        public void SetAnswerOptions(params (string text, UnityAction action)[] answers)
        {
            foreach (var go in _answerOptionUIGameObjects)
            {
                Destroy(go);
            }

            var count = answers.Length;
            foreach (var (text, action) in answers)
            {
                var answerOption = Instantiate(_answerOptionUIPrefab, _answersParentTransform);
                answerOption.GetComponent<AnswerOptionUI>().SetAnswerOption(text, action);
                _answerOptionUIGameObjects.Add(answerOption);

                var rectTransform = answerOption.GetComponent<RectTransform>();
                var anchoredPosition = rectTransform.anchoredPosition;
                rectTransform.anchoredPosition = new Vector2(
                    anchoredPosition.x,
                    anchoredPosition.y + rectTransform.sizeDelta.y * count * 1.5f);

                count--;

                answerOption.SetActive(true);
            }
        }

        public void SetActiveAnswerOptions(bool value)
        {
            _answerOptionsGameObject.SetActive(value);
        }
    }
}