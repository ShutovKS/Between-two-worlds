using System;
using System.Collections;
using Data.Dynamic;
using Data.Localization.Dialogues;
using Infrastructure.Services;
using Infrastructure.Services.LocalisationDataLoad;
using Infrastructure.Services.SaveLoadData;
using Infrastructure.Services.UIFactory;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Infrastructure.ScenesManagers.Core
{
    public class Core : MonoBehaviour
    {
        private const float SECONDS_DELAY_DEFAULT = 0.05f;
        private const float SECONDS_DELAY_FAST = 0.005f;

        private ILocalisationDataLoadService _localisationDataLoad;
        private ISaveLoadDataService _saveLoadData;
        private IUIFactoryInfoService _uiFactoryInfo;

        private DynamicData _data;
        private Dialogues _dialogues;

        private bool _isDialogCompleted;
        private bool _isAutoMode;
        private bool _isSpeedUpMode;

        private IPhrase _currentDialogue;
        private UnityAction _onDialogueCompleted;
        private Coroutine _displayTypingCoroutine;
        private float _secondsDelay;

        private void Awake()
        {
            InitializedServices();
            LoadData();
            RegisterButtons();
            StartDialogue();
        }

        private void InitializedServices()
        {
            _localisationDataLoad = ServicesContainer.GetService<ILocalisationDataLoadService>();
            _saveLoadData = ServicesContainer.GetService<ISaveLoadDataService>();
            _uiFactoryInfo = ServicesContainer.GetService<IUIFactoryInfoService>();
        }

        private void LoadData()
        {
            _data = _saveLoadData.Load();
        }

        private void RegisterButtons()
        {
            _uiFactoryInfo.DialogueUI.Buttons.RegisterBackButtonCallback(OnClickBack);
            _uiFactoryInfo.DialogueUI.Buttons.RegisterSaveButtonCallback(OnClickSave);
            _uiFactoryInfo.DialogueUI.Buttons.RegisterLoadButtonCallback(OnClickLoad);
            _uiFactoryInfo.DialogueUI.Buttons.RegisterSettingsButtonCallback(OnClickSettings);

            _uiFactoryInfo.DialogueUI.Buttons.RegisterHistoryButtonCallback(OnClickHistory);
            _uiFactoryInfo.DialogueUI.Buttons.RegisterSpeedUpButtonCallback(OnClickSpeedUp);
            _uiFactoryInfo.DialogueUI.Buttons.RegisterAutoButtonCallback(OnClickAuto);
            _uiFactoryInfo.DialogueUI.Buttons.RegisterFurtherButtonCallback(OnClickFurther);
        }

        private void StartDialogue()
        {
            _secondsDelay = SECONDS_DELAY_DEFAULT;
            _uiFactoryInfo.DialogueUI.SetActivePanel(true);
            SetDialog(_data.dialogues.idLastDialogue);
        }

        private void SetDialog(string id)
        {
            _currentDialogue = _localisationDataLoad.GetPart(id);
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

        private void SetPhraseTyping(Phrase phrase)
        {
            _uiFactoryInfo.DialogueUI.Answers.SetActiveAnswerOptions(false);
            _uiFactoryInfo.BackgroundUI.SetBackgroundImage(GetTexture2D("Backgrounds/" + phrase.BackgroundPath));
            _uiFactoryInfo.DialogueUI.DialogueText.SetAuthorName(phrase.Name);
            _uiFactoryInfo.DialogueUI.DialogueText.SetText(string.Empty);

            if (phrase.CharacterAvatarPath == null)
            {
                _uiFactoryInfo.DialogueUI.Person.SetActionAvatar(false);
            }
            else
            {
                _uiFactoryInfo.DialogueUI.Person.SetActionAvatar(true);
                _uiFactoryInfo.DialogueUI.Person.SetAvatar(
                    GetTexture2D("CharacterAvatars/" + phrase.CharacterAvatarPath));
            }

            _displayTypingCoroutine = StartCoroutine(DisplayTyping(phrase.Text));
        }

        private IEnumerator DisplayTyping(string text)
        {
            var currentText = string.Empty;
            foreach (var letter in text)
            {
                currentText += letter;
                _uiFactoryInfo.DialogueUI.DialogueText.SetText(currentText);
                yield return new WaitForSeconds(_secondsDelay);
            }

            yield return new WaitForSeconds(_secondsDelay * 5);

            _isDialogCompleted = true;
            _onDialogueCompleted?.Invoke();
        }

        private void SetResponseChoice(Responses response)
        {
            _uiFactoryInfo.DialogueUI.Answers.SetActiveAnswerOptions(true);

            var tuples = new (string, UnityAction)[response.ResponseList.Length];
            for (var i = 0; i < response.ResponseList.Length; i++)
            {
                var index = i;
                tuples[i] = (response.ResponseList[i].AnswerText,
                    () => SetDialog(response.ResponseList[index].IDNextDialog));
            }

            _uiFactoryInfo.DialogueUI.Answers.SetAnswerOptions(tuples);
        }

        #region

        private void OnClickBack()
        {
            _uiFactoryInfo.ConfirmationUI.SetActivePanel(true);
            _uiFactoryInfo.ConfirmationUI.Buttons.RegisterYesButtonCallback(
                () =>
                {
                    SceneManager.LoadScene("2.Meta");
                    _uiFactoryInfo.ConfirmationUI.SetActivePanel(false);
                    Exit();
                });

            _uiFactoryInfo.ConfirmationUI.Buttons.RegisterNoButtonCallback(
                () => { _uiFactoryInfo.ConfirmationUI.SetActivePanel(false); });
        }

        private void OnClickSave()
        {
            _data.dialogues.idLastDialogue = _currentDialogue.ID;
            _saveLoadData.Save(_data);
        }

        private void OnClickLoad()
        {
        }

        private void OnClickSettings()
        {
        }

        private void OnClickHistory()
        {
        }

        #endregion


        private void OnClickSpeedUp()
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

        private void OnClickAuto()
        {
            if (_isAutoMode)
            {
                _isAutoMode = false;
                _onDialogueCompleted -= AutoDialogSwitch;
            }
            else
            {
                _isAutoMode = true;
                _onDialogueCompleted += AutoDialogSwitch;
                if (_isDialogCompleted)
                {
                    AutoDialogSwitch();
                }
            }
        }

        private void AutoDialogSwitch()
        {
            if (_isDialogCompleted && _currentDialogue is Phrase phrase)
            {
                SetDialog(phrase.IDNextDialog);
            }
        }

        private void OnClickFurther()
        {
            if (_currentDialogue is not Phrase phrase)
                throw new Exception($"Current dialogue is not {_currentDialogue.ID}");

            if (_isDialogCompleted)
            {
                SetDialog(phrase.IDNextDialog);
            }
            else
            {
                _isDialogCompleted = true;
                StopCoroutine(_displayTypingCoroutine);
                _uiFactoryInfo.DialogueUI.DialogueText.SetText(phrase.Text);
                _onDialogueCompleted?.Invoke();
            }
        }

        private void Exit()
        {
            _uiFactoryInfo.DialogueUI.SetActivePanel(false);
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