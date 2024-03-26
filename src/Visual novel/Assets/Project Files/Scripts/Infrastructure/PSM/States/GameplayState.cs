using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Data.Dynamic;
using Data.Static.Dialogues;
using Infrastructure.PSM.Core;
using Infrastructure.Services.CoroutineRunner;
using Infrastructure.Services.DialogueStories;
using Infrastructure.Services.Localisation;
using Infrastructure.Services.Progress;
using Infrastructure.Services.Sounds;
using Infrastructure.Services.WindowsService;
using UI.Background;
using UI.Confirmation;
using UI.Dialogue;
using UnityEngine;
using UnityEngine.Events;
using static Tools.Extensions.Resources;

namespace Infrastructure.PSM.States
{
    public class GameplayState : IState<Bootstrap>, IEnterable, IExitable
    {
        public GameplayState(Bootstrap initializer, IWindowService windowService, IProgressService progressService,
            ICoroutineRunnerService coroutineRunnerService, ISoundService soundService,
            ILocalisationService localisationService, IDialogueHistoryService dialogueHistoryService)
        {
            _windowService = windowService;
            _progressService = progressService;
            _coroutineRunnerService = coroutineRunnerService;
            _soundService = soundService;
            _localisationService = localisationService;
            _dialogueHistoryService = dialogueHistoryService;
            Initializer = initializer;
        }

        public Bootstrap Initializer { get; }

        private const float SECONDS_DELAY_DEFAULT = 0.05f;
        private const float SECONDS_DELAY_FAST = 0.005f;

        private readonly IWindowService _windowService;
        private readonly IProgressService _progressService;
        private readonly ICoroutineRunnerService _coroutineRunnerService;
        private readonly ISoundService _soundService;
        private readonly ILocalisationService _localisationService;
        private readonly IDialogueHistoryService _dialogueHistoryService;

        private DialogueUI _dialogueUI;
        private BackgroundUI _backgroundUI;
        private GameData _gameData;
        private IPhrase _currentDialogue;
        private Coroutine _displayTypingCoroutine;

        private UnityAction _onDialogueCompleted;

        private string _currentSoundEffect;
        private float _typingDelay = SECONDS_DELAY_DEFAULT;
        private bool _isDialogCompleted;
        private bool _isSpeedUpMode;
        private bool _isAutoMode;

        public async void OnEnter()
        {
            _gameData = _progressService.GetProgress();

            await OpenUI();

            TryLoadDialogueHistory();

            StartDialogue(_gameData.currentDialogue);
        }

        public void OnExit()
        {
            ResetData();

            _windowService.Close(WindowID.Dialogue);
            _windowService.Close(WindowID.Background);
        }

        private async Task OpenUI()
        {
            _backgroundUI = await _windowService.OpenAndGetComponent<BackgroundUI>(WindowID.Background);

            _dialogueUI = await _windowService.OpenAndGetComponent<DialogueUI>(WindowID.Dialogue);
            _dialogueUI.Buttons.OnBackButtonClicked = ConfirmExitInMenu;
            _dialogueUI.Buttons.OnSaveButtonClicked = OpenSaveMenu;
            _dialogueUI.Buttons.OnLoadButtonClicked = OpenLoadMenu;
            _dialogueUI.Buttons.OnHistoryButtonClicked = OpenDialogHistory;
            _dialogueUI.Buttons.OnSpeedUpButtonClicked = ChangeTypingDialogSpeedUp;
            _dialogueUI.Buttons.OnAutoButtonClicked = AutoDialogSwitchMode;
            _dialogueUI.Buttons.OnFurtherButtonClicked = DialogFurther;
        }

        private void ResetData()
        {
            _coroutineRunnerService.StopCoroutine(_displayTypingCoroutine);
            _currentSoundEffect = string.Empty;
            _typingDelay = SECONDS_DELAY_DEFAULT;
            _isDialogCompleted = false;
            _isSpeedUpMode = false;
            _isAutoMode = false;
        }

        private void StartDialogue(string id)
        {
            _typingDelay = SECONDS_DELAY_DEFAULT;
            _dialogueUI.SetActivePanel(true);

            SetDialog(id);
        }

        private void DialogFurther()
        {
            if (_currentDialogue is not Phrase phrase)
            {
                throw new Exception($"Current dialogue is not {_currentDialogue.ID}");
            }

            if (_isDialogCompleted)
            {
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

        private void ChangeTypingDialogSpeedUp()
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

        private void AutoDialogSwitchMode()
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

        private void StopAutoDialogSwitchMode()
        {
            if (_isAutoMode)
            {
                AutoDialogSwitchMode();
            }
        }

        private void SetDialog(string id)
        {
            _currentDialogue = _localisationService.GetPhraseId(id);
            _isDialogCompleted = false;

            switch (_currentDialogue)
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
            if (_isDialogCompleted && _currentDialogue is Phrase phrase)
            {
                SetDialog(phrase.IDNextDialog);
            }
        }

        private void SetPhraseTyping(Phrase phrase)
        {
            _dialogueUI.Answers.SetActiveAnswerOptions(false);
            _backgroundUI.SetImage(GetTexture2D("Backgrounds/" + phrase.BackgroundPath));
            _dialogueUI.DialogueText.SetAuthorName(phrase.Name);
            _dialogueUI.DialogueText.SetText(string.Empty);

            if (phrase.CharacterAvatarPath == null)
            {
                _dialogueUI.Person.SetActionAvatar(false);
            }
            else
            {
                _dialogueUI.Person.SetActionAvatar(true);
                _dialogueUI.Person.SetAvatar(GetTexture2D("CharacterAvatars/" + phrase.CharacterAvatarPath));
            }

            if (_currentSoundEffect != phrase.SoundEffect)
            {
                _currentSoundEffect = phrase.SoundEffect;
                _soundService.SetClip(phrase.SoundEffect, true);
            }
            
            if (!string.IsNullOrEmpty(phrase.ActionTrigger))
            {
                HandleActionTrigger(phrase.ActionTrigger);
            }

            SetNewCurrentDialogue(phrase.ID);
            AddDialogueInHistory(phrase.ID, phrase.Name, phrase.Text);
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
                tuples[i] = (response.ResponseList[i].AnswerText, () =>
                {
                    SetNewCurrentDialogue(response.ID);
                    AddDialogueInHistory(response.ID, null, response.ResponseList[index].AnswerText);
                    SetDialog(response.ResponseList[index].IDNextDialog);
                });
            }

            _dialogueUI.Answers.SetAnswerOptions(tuples);
        }

        private void OpenDialogHistory()
        {
            _dialogueUI.History.SetActivePanel(true);
            _dialogueUI.History.OnBackButtonClicked = () => _dialogueUI.History.SetActivePanel(false);
        }

        private async void ConfirmExitInMenu()
        {
            StopAutoDialogSwitchMode();
            var confirmationUI = await _windowService.OpenAndGetComponent<ConfirmationUI>(WindowID.Confirmation);
            confirmationUI.Buttons.OnYesButtonClicked = () =>
            {
                _windowService.Close(WindowID.Confirmation);
                OpenMenu();
            };
            confirmationUI.Buttons.OnNoButtonClicked = () => { _windowService.Close(WindowID.Confirmation); };
        }

        private void SetNewCurrentDialogue(string id)
        {
            _gameData.LastSaveTime = DateTime.Now;
            _gameData.currentDialogue = id;
            _progressService.SetProgress(_gameData);
        }

        private void HandleActionTrigger(string actionTrigger)
        {
            if (actionTrigger.Contains("end"))
            {
                Debug.Log("end");
                _dialogueUI.Buttons.OnFurtherButtonClicked = () => OpenLastWords(actionTrigger);
            }
        }

        private void TryLoadDialogueHistory()
        {
            if (_dialogueHistoryService.GetLatestDialogue().HasValue &&
                _dialogueHistoryService.GetLatestDialogue()?.id == _gameData.currentDialogue)
            {
                var dialogInHistories = _dialogueHistoryService.GetHistory();
                foreach (var dialog in dialogInHistories)
                {
                    _dialogueUI.History.CreateHistoryPhrase(dialog.id, dialog.name, dialog.text);
                }
            }
            else
            {
                _dialogueHistoryService.Clear();
            }
        }

        private void AddDialogueInHistory(string id, string name, string text)
        {
            _dialogueUI.History.CreateHistoryPhrase(id, name, text);
            _dialogueHistoryService.AddDialogue(id, name, text);
        }

        private void OpenLoadMenu()
        {
            Initializer.StateMachine.SwitchState<LoadMenuState, GameplayState>(this);
        }

        private void OpenSaveMenu()
        {
            Initializer.StateMachine.SwitchState<SaveMenuState, string>(_gameData.currentDialogue);
        }

        private void OpenMenu()
        {
            Initializer.StateMachine.SwitchState<MenuState>();
        }

        private void OpenLastWords(string endingNumber)
        {
            Initializer.StateMachine.SwitchState<LastWordsState, string>(endingNumber);
        }
    }
}