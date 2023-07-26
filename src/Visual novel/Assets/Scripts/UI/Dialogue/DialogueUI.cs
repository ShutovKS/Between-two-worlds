﻿using UnityEngine;
using UnityEngine.Events;
using static Units.Tools.Tools;

namespace UI.Dialogue
{
    public class DialogueUI : MonoBehaviour
    {
        [SerializeField] private Answers _answers;
        [SerializeField] private Background _background;
        [SerializeField] private Person _person;
        [SerializeField] private Text _text;

        public void SetAnswers((string text, UnityAction action) answer1, (string text, UnityAction action) answer2)
        {
            _answers.AnswerText1.text = answer1.text;
            _answers.AnswerButton1.onClick.RemoveAllListeners();
            _answers.AnswerButton1.onClick.AddListener(answer1.action);

            _answers.AnswerText2.text = answer2.text;
            _answers.AnswerButton2.onClick.RemoveAllListeners();
            _answers.AnswerButton2.onClick.AddListener(answer2.action);
        }

        public void SetBackground(Texture2D texture2D) => _background.BackgroundImage.sprite = texture2D.ToSprite();

        public void SetActionAvatar(bool value) => _person.AvatarGO.SetActive(value);

        public void SetAvatar(Texture2D texture2D) => _person.AvatarImage.sprite = texture2D.ToSprite();

        public void SetText(string authorName, string text, UnityAction action)
        {
            _text.AuthorNameText.text = authorName;
            _text.TextText.text = text;
            _text.FurtherButton.onClick.RemoveAllListeners();
            _text.FurtherButton.onClick.AddListener(action);
        }
    }
}