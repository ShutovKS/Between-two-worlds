using System;
using System.Threading.Tasks;
using Data.Localization.Dialogues;
using Data.Static;
using Infrastructure.Services.LocalisationDataLoad;
using UI.Background;
using UI.Dialogue;
using UnityEngine;
using UnityEngine.Events;

namespace Infrastructure.ScenesManagers.Core
{
    public class DialogueManager
    {
        private const float SECONDS_DELAY_DEFAULT = 0.05f;
        private const float SECONDS_DELAY_FAST = 0.005f;

        private bool _isDialogCompleted;
        private bool _isAutoMode;
        private bool _isSpeedUpMode;

        public IPhrase CurrentDialogue { get; private set; }
        private UnityAction _onDialogueCompleted;
        private Task _displayTypingTask;
        private float _secondsDelay;

        private readonly Func<string, IPhrase> _onGetPart;
        private readonly BackgroundUI _backgroundUI;
        private readonly DialogueUI _dialogueUI;

        public DialogueManager(Func<string, IPhrase> onGetPart, DialogueUI dialogueUI,
            BackgroundUI backgroundUI)
        {
            _onGetPart = onGetPart;
            _dialogueUI = dialogueUI;
            _backgroundUI = backgroundUI;
        }

        public void StartDialogue()
        {
            _secondsDelay = SECONDS_DELAY_DEFAULT;
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
                SetDialog(phrase.IDNextDialog);
            }
            else
            {
                _isDialogCompleted = true;
                _displayTypingTask.Dispose();
                _dialogueUI.DialogueText.SetText(phrase.Text);
                _onDialogueCompleted?.Invoke();
            }
        }

        public void ChangeTypingDialogSpeedUp()
        {
            if (_isSpeedUpMode)
            {
                _isSpeedUpMode = false;
                _secondsDelay = SECONDS_DELAY_DEFAULT;
            }
            else
            {
                _isSpeedUpMode = true;
                _secondsDelay = SECONDS_DELAY_FAST;
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
                SetDialog(phrase.IDNextDialog);
            }
        }

        private void SetPhraseTyping(Phrase phrase)
        {
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

            _displayTypingTask = DisplayTyping(phrase.Text);
        }

        private async Task DisplayTyping(string text)
        {
            var currentText = string.Empty;
            foreach (var letter in text)
            {
                currentText += letter;
                _dialogueUI.DialogueText.SetText(currentText);
                await Task.Delay(TimeSpan.FromSeconds(_secondsDelay));
            }

            await Task.Delay(TimeSpan.FromSeconds(_secondsDelay * 5));

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
                    () => SetDialog(response.ResponseList[index].IDNextDialog));
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