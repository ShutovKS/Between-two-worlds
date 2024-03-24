#region

using System;
using System.Globalization;
using Data.Dynamic;
using Data.Localization.Dialogues;
using Infrastructure.Services.LocalisationDataLoad;
using Infrastructure.Services.Metric;
using Infrastructure.Services.Progress;
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
        public SaveLoadManager(IProgressService progress, SaveLoadUI saveLoadUI, GameData data,
            ImageCaptureForSaveUI imageCaptureForSaveUI, ILocalisationDataLoadService localisationDataLoad, 
            IMetricService metricService)
        {
            _progress = progress;
            _saveLoadUI = saveLoadUI;
            _data = data;
            _imageCaptureForSaveUI = imageCaptureForSaveUI;
            _localisationDataLoad = localisationDataLoad;
            _metric = metricService;
        }

        private readonly GameData _data;
        private readonly IProgressService _progress;
        private readonly ILocalisationDataLoadService _localisationDataLoad;
        private readonly IMetricService _metric;
        private readonly ImageCaptureForSaveUI _imageCaptureForSaveUI;
        private readonly SaveLoadUI _saveLoadUI;
        
        public Func<string> OnGetDialogCurrent;
        public UnityAction<string> OnSetDialog;
        public UnityAction OnClearHistory;


        public void OpenDataSave()
        {
            _saveLoadUI.SetActivePanel(true);
            _saveLoadUI.ButtonsUI.OnButtonClicked = CloseDataScreen;

            var index = 0;
            foreach (var dataUI in _saveLoadUI.SaveDataUIs)
            {
                var indexLocal = index;

                var data = _data.dialogues[index++];

                SetConfigForDataUI(dataUI, data, OnSaveButtonClicked);
                continue;

                void OnSaveButtonClicked()
                {
                    _metric.SendEvent(MetricEventType.Saved);
                    
                    var id = OnGetDialogCurrent.Invoke();
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

                    _data.LastSaveTime = DateTime.Now;
                    _progress.SetProgress(_data);

                    SetConfigForDataUI(dataUI, _data.dialogues[indexLocal], OnSaveButtonClicked);
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
                    SetConfigForDataUI(dataUI, dataDialogue, OnLoadButtonClicked);
                }

                continue;

                void OnLoadButtonClicked()
                {
                    _metric.SendEvent(MetricEventType.Load);
                    
                    OnClearHistory?.Invoke();
                    OnSetDialog.Invoke(dataDialogue.idLastDialogue);
                    _saveLoadUI.SetActivePanel(false);
                }
            }
        }

        private static void SetConfigForDataUI(WindowSaveLoadUI loadUI, DialoguesData dialoguesData, Action onButtonClicked)
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