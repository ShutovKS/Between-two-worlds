#region

using System;
using System.Collections;
using Data.Localization.Dialogues;
using Data.Static;
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
        public DialogueManager(Func<string, IPhrase> onGetPart, DialogueUI dialogueUI,
            BackgroundUI backgroundUI, ICoroutineRunner coroutineRunner, ISoundsService soundsService,
            UnityAction<string, string, string> onNewDialog, UnityAction<string> handleActionTrigger)
        {
            _onGetPart = onGetPart;
            _dialogueUI = dialogueUI;
            _backgroundUI = backgroundUI;
            _coroutineRunner = coroutineRunner;
            _soundsService = soundsService;
            _onNewDialog = onNewDialog;
            _handleActionTrigger = handleActionTrigger;
        }

        private const float SECONDS_DELAY_DEFAULT = 0.05f;
        private const float SECONDS_DELAY_FAST = 0.005f;

        public IPhrase CurrentDialogue { get; private set; }
        private readonly BackgroundUI _backgroundUI;
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly ISoundsService _soundsService;
        private readonly UnityAction<string, string, string> _onNewDialog;
        private readonly UnityAction<string> _handleActionTrigger;
        private readonly DialogueUI _dialogueUI;

        private readonly Func<string, IPhrase> _onGetPart;
        private Coroutine _displayTypingCoroutine;
        private bool _isAutoMode;

        private bool _isDialogCompleted;
        private bool _isSpeedUpMode;
        private UnityAction _onDialogueCompleted;
        private float _typingDelay;

        private string _currentSoundEffect;

        public void StartDialogue()
        {
            _typingDelay = SECONDS_DELAY_DEFAULT;
            _dialogueUI.SetActivePanel(true);
            var id = PlayerPrefs.GetString(Constant.KEY_ID_DIALOGUE_FOR_PLAYER_PREFS, Constant.DIALOG_START_ID);
            SetDialog(id);
        }

        public void DialogFurther()
        {
            if (CurrentDialogue is not Phrase phrase)
                throw new Exception($"Current dialogue is not {CurrentDialogue.ID}");

            if (_isDialogCompleted)
            {
                if (!string.IsNullOrEmpty(phrase.ActionTrigger)) _handleActionTrigger?.Invoke(phrase.ActionTrigger);
                SetDialog(phrase.IDNextDialog);
            }
            else
            {
                _isDialogCompleted = true;
                _coroutineRunner.StopCoroutine(_displayTypingCoroutine);
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
            CurrentDialogue = _onGetPart?.Invoke(id);
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
                if (!string.IsNullOrEmpty(phrase.ActionTrigger)) _handleActionTrigger?.Invoke(phrase.ActionTrigger);
                SetDialog(phrase.IDNextDialog);
            }
        }

        private void SetPhraseTyping(Phrase phrase)
        {
            _onNewDialog.Invoke(phrase.ID, phrase.Name, phrase.Text);
            _dialogueUI.Answers.SetActiveAnswerOptions(false);
            _backgroundUI.SetBackgroundImage(GetTexture2D("Backgrounds/" + phrase.BackgroundPath));
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
                    GetTexture2D("CharacterAvatars/" + phrase.CharacterAvatarPath));
            }

            if (_currentSoundEffect != phrase.SoundEffect)
            {
                _currentSoundEffect = phrase.SoundEffect;
                _soundsService.SetClip(phrase.SoundEffect, true);
            }

            _displayTypingCoroutine = _coroutineRunner.StartCoroutine(DisplayTyping(phrase.Text));
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
                        //TODO: add name to response
                        _onNewDialog.Invoke(response.ID, null, response.ResponseList[index].AnswerText);
                        SetDialog(response.ResponseList[index].IDNextDialog);
                    });
            }

            _dialogueUI.Answers.SetAnswerOptions(tuples);
        }

        #region Tools

        private static Texture2D GetTexture2D(string path)
        {
            var texture2D = Resources.Load<Texture2D>("Data/" + path);

            return texture2D;
        }

        #endregion

    }
}
