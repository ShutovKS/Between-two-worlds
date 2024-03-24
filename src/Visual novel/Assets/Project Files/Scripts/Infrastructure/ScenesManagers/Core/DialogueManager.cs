#region

using System;
using System.Collections;
using Data.Localization.Dialogues;
using Infrastructure.Services.CoroutineRunner;
using Infrastructure.Services.Sounds;
using UI.Background;
using UI.Dialogue;
using UnityEngine;
using UnityEngine.Events;

#endregion

namespace Infrastructure.ScenesManagers.Core
{
    public class DialogueManager
    {
        public DialogueManager(DialogueUI dialogueUI, BackgroundUI backgroundUI,
            ICoroutineRunnerService coroutineRunnerService, ISoundsService soundsService)
        {
            _dialogueUI = dialogueUI;
            _backgroundUI = backgroundUI;
            _coroutineRunnerService = coroutineRunnerService;
            _soundsService = soundsService;
        }

        private const float SECONDS_DELAY_DEFAULT = 0.05f;
        private const float SECONDS_DELAY_FAST = 0.005f;

        public IPhrase CurrentDialogue { get; private set; }

        private readonly ICoroutineRunnerService _coroutineRunnerService;
        private readonly ISoundsService _soundsService;
        private readonly BackgroundUI _backgroundUI;
        private readonly DialogueUI _dialogueUI;

        public UnityAction<string, string, string> OnNewDialog;
        public UnityAction<string> HandleActionTrigger;
        public Func<string, IPhrase> OnGetPart;

        private UnityAction _onDialogueCompleted;

        private Coroutine _displayTypingCoroutine;

        private string _currentSoundEffect;

        private float _typingDelay;

        private bool _isDialogCompleted;
        private bool _isSpeedUpMode;
        private bool _isAutoMode;


        public void StartDialogue(string id)
        {
            _typingDelay = SECONDS_DELAY_DEFAULT;
            _dialogueUI.SetActivePanel(true);

            SetDialog(id);
        }

        public void DialogFurther()
        {
            if (CurrentDialogue is not Phrase phrase)
            {
                throw new Exception($"Current dialogue is not {CurrentDialogue.ID}");
            }

            if (_isDialogCompleted)
            {
                if (!string.IsNullOrEmpty(phrase.ActionTrigger))
                {
                    HandleActionTrigger?.Invoke(phrase.ActionTrigger);
                }

                SetDialog(phrase.IDNextDialog);
            }
            else
            {
                _isDialogCompleted = true;
                _coroutineRunnerService.StopCoroutine(_displayTypingCoroutine);
                _dialogueUI.DialogueText.SetText(phrase.Text);
                _onDialogueCompleted?.Invoke();
            }
        }

        public void ChangeTypingDialogSpeedUp()
        {
            if (_isSpeedUpMode)
            {
                _isSpeedUpMode = false;
                _typingDelay = SECONDS_DELAY_DEFAULT;
            }
            else
            {
                _isSpeedUpMode = true;
                _typingDelay = SECONDS_DELAY_FAST;
            }
        }

        public void AutoDialogSwitchMode()
        {
            if (_isAutoMode)
            {
                _isAutoMode = false;
                _onDialogueCompleted -= AutoDialogSwitchIfComplete;
            }
            else
            {
                _isAutoMode = true;
                _onDialogueCompleted += AutoDialogSwitchIfComplete;
                if (_isDialogCompleted)
                {
                    AutoDialogSwitchIfComplete();
                }
            }
        }

        public void StopAutoDialogSwitchMode()
        {
            if (_isAutoMode)
            {
                AutoDialogSwitchMode();
            }
        }

        public void SetDialog(string id)
        {
            CurrentDialogue = OnGetPart?.Invoke(id);
            _isDialogCompleted = false;

            switch (CurrentDialogue)
            {
                case Phrase phrase:
                    SetPhraseTyping(phrase);
                    break;
                case Responses response:
                    SetResponseChoice(response);
                    break;
            }
        }

        private void AutoDialogSwitchIfComplete()
        {
            if (_isDialogCompleted && CurrentDialogue is Phrase phrase)
            {
                if (!string.IsNullOrEmpty(phrase.ActionTrigger))
                {
                    HandleActionTrigger?.Invoke(phrase.ActionTrigger);
                }

                SetDialog(phrase.IDNextDialog);
            }
        }

        private void SetPhraseTyping(Phrase phrase)
        {
            OnNewDialog.Invoke(phrase.ID, phrase.Name, phrase.Text);
            _dialogueUI.Answers.SetActiveAnswerOptions(false);
            _backgroundUI.SetBackgroundImage(
                Tools.Extensions.Resources.GetTexture2D("Backgrounds/" + phrase.BackgroundPath));
            _dialogueUI.DialogueText.SetAuthorName(phrase.Name);
            _dialogueUI.DialogueText.SetText(string.Empty);

            if (phrase.CharacterAvatarPath == null)
            {
                _dialogueUI.Person.SetActionAvatar(false);
            }
            else
            {
                _dialogueUI.Person.SetActionAvatar(true);
                _dialogueUI.Person.SetAvatar(
                    Tools.Extensions.Resources.GetTexture2D("CharacterAvatars/" + phrase.CharacterAvatarPath));
            }

            if (_currentSoundEffect != phrase.SoundEffect)
            {
                _currentSoundEffect = phrase.SoundEffect;
                _soundsService.SetClip(phrase.SoundEffect, true);
            }

            _displayTypingCoroutine = _coroutineRunnerService.StartCoroutine(DisplayTyping(phrase.Text));
        }

        private IEnumerator DisplayTyping(string text)
        {
            var currentText = string.Empty;
            foreach (var letter in text)
            {
                currentText += letter;
                _dialogueUI.DialogueText.SetText(currentText);
                yield return new WaitForSeconds(_typingDelay);
            }

            yield return new WaitForSeconds(_typingDelay * 5);

            _isDialogCompleted = true;
            _onDialogueCompleted?.Invoke();
        }

        private void SetResponseChoice(Responses response)
        {
            _dialogueUI.Answers.SetActiveAnswerOptions(true);

            var tuples = new (string, UnityAction)[response.ResponseList.Length];
            for (var i = 0; i < response.ResponseList.Length; i++)
            {
                var index = i;
                tuples[i] = (response.ResponseList[i].AnswerText,
                    () =>
                    {
                        OnNewDialog.Invoke(response.ID, null, response.ResponseList[index].AnswerText);
                        SetDialog(response.ResponseList[index].IDNextDialog);
                    });
            }

            _dialogueUI.Answers.SetAnswerOptions(tuples);
        }
    }
}