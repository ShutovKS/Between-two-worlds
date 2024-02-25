#region

using System;
using System.Globalization;
using Data.Dynamic;
using Data.Localization.Dialogues;
using Infrastructure.Services.LocalisationDataLoad;
using Infrastructure.Services.SaveLoadData;
using Tools.Camera;
using UI.ImageCaptureForSave;
using UI.SaveLoad;
using UnityEngine;
using UnityEngine.Events;
using static Tools.Extensions.Resources;

#endregion

namespace Infrastructure.ScenesManagers.Core
{
    public class SaveLoadManager
    {
        public SaveLoadManager(ISaveLoadDataService saveLoadData, SaveLoadUI saveLoadUI, GameData data,
            Func<string> onGetDialogCurrent, ImageCaptureForSaveUI imageCaptureForSaveUI, 
            ILocalisationDataLoadService localisationDataLoad, UnityAction<string> onSetDialog, UnityAction onClearHistory)
        {
            _saveLoadData = saveLoadData;
            _saveLoadUI = saveLoadUI;
            _data = data;
            _onGetDialogCurrent = onGetDialogCurrent;
            _imageCaptureForSaveUI = imageCaptureForSaveUI;
            _localisationDataLoad = localisationDataLoad;
            _onSetDialog = onSetDialog;
            _onClearHistory = onClearHistory;
        }

        private readonly Func<string> _onGetDialogCurrent;
        private readonly UnityAction<string> _onSetDialog;
        private readonly UnityAction _onClearHistory;
        private readonly GameData _data;
        private readonly ISaveLoadDataService _saveLoadData;
        private readonly ILocalisationDataLoadService _localisationDataLoad;
        private readonly ImageCaptureForSaveUI _imageCaptureForSaveUI;
        private readonly SaveLoadUI _saveLoadUI;

        public void OpenDataSave()
        {
            _saveLoadUI.SetActivePanel(true);
            _saveLoadUI.ButtonsUI.OnButtonClicked = CloseDataScreen;

            var index = 0;
            foreach (var dataUI in _saveLoadUI.SaveDataUIs)
            {
                var indexLocal = index;

                var data = _data.dialogues[index++];

                SetConfigForDataUI(dataUI, data, OnButtonClicked);
                continue;

                void OnButtonClicked()
                {
                    var id = _onGetDialogCurrent.Invoke();
                    var titleText = DateTime.Now.ToString(CultureInfo.InvariantCulture);
                    var phraseId = _localisationDataLoad.GetPhraseId(id);

                    var backgroundUI = _imageCaptureForSaveUI.BackgroundUI;
                    var dialogueUI = _imageCaptureForSaveUI.DialogueUI;

                    switch (phraseId)
                    {
                        case Phrase phrase:
                        {
                            backgroundUI.SetBackgroundImage(GetTexture2D("Backgrounds/" + phrase.BackgroundPath));
                            dialogueUI.Answers.SetActiveAnswerOptions(false);
                            dialogueUI.DialogueText.SetAuthorName(phrase.Name);
                            dialogueUI.DialogueText.SetText(phrase.Text);
                            if (phrase.CharacterAvatarPath == null)
                            {
                                dialogueUI.Person.SetActionAvatar(false);
                            }
                            else
                            {
                                dialogueUI.Person.SetActionAvatar(true);
                                dialogueUI.Person.SetAvatar(
                                    GetTexture2D("CharacterAvatars/" + phrase.CharacterAvatarPath));
                            }

                            break;
                        }
                        case Responses response:
                        {
                            dialogueUI.Answers.SetActiveAnswerOptions(true);

                            var tuples = new (string, UnityAction)[response.ResponseList.Length];
                            for (var i = 0; i < response.ResponseList.Length; i++)
                            {
                                tuples[i] = (response.ResponseList[i].AnswerText, null);
                            }

                            dialogueUI.Answers.SetAnswerOptions(tuples);
                            break;
                        }
                    }

                    _imageCaptureForSaveUI.SetActivePanel(true);

                    var texture2D = CameraTextureCapture.CaptureCameraView(
                        backgroundUI.GetComponent<Canvas>(),
                        dialogueUI.GetComponent<Canvas>());

                    _imageCaptureForSaveUI.SetActivePanel(false);

                    _data.dialogues[indexLocal] = new DialoguesData
                    {
                        idLastDialogue = id,
                        titleText = titleText,
                        Background = texture2D,
                        isDataExist = true
                    };

                    _saveLoadData.Save(_data);

                    SetConfigForDataUI(dataUI, _data.dialogues[indexLocal], OnButtonClicked);
                }
            }
        }

        public void OpenDataLoad()
        {
            _saveLoadUI.SetActivePanel(true);
            _saveLoadUI.ButtonsUI.OnButtonClicked = CloseDataScreen;

            var index = 0;
            foreach (var dataUI in _saveLoadUI.SaveDataUIs)
            {
                var dataDialogue = _data.dialogues[index++];

                if (dataDialogue.isDataExist)
                {
                    SetConfigForDataUI(dataUI, dataDialogue, OnButtonClicked);
                }

                continue;

                void OnButtonClicked()
                {
                    _onClearHistory?.Invoke();
                    _onSetDialog.Invoke(dataDialogue.idLastDialogue);
                    _saveLoadUI.SetActivePanel(false);
                }
            }
        }

        private void SetConfigForDataUI(WindowSaveLoadUI loadUI, DialoguesData dialoguesData, Action onButtonClicked)
        {
            loadUI.SetImage(dialoguesData.Background);
            loadUI.SetTitle(dialoguesData.titleText);
            loadUI.OnButtonClicked = onButtonClicked;
        }

        private void CloseDataScreen()
        {
            foreach (var dataUI in _saveLoadUI.SaveDataUIs)
            {
                dataUI.OnButtonClicked = null;
            }

            _saveLoadUI.SetActivePanel(false);
        }
    }
}