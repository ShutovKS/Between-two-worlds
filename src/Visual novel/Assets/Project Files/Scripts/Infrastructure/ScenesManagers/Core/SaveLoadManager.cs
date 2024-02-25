#region

using System;
using System.Globalization;
using Data.Dynamic;
using Infrastructure.Services.SaveLoadData;
using UI.Background;
using UI.Dialogue;
using UI.SaveLoad;
using Unit.Tools.Camera;
using UnityEngine;
using UnityEngine.Events;

#endregion

namespace Infrastructure.ScenesManagers.Core
{
    public class SaveLoadManager
    {
        public SaveLoadManager(ISaveLoadDataService saveLoadData, SaveLoadUI saveLoadUI, GameData data,
            Func<string> onGetDialogCurrent, BackgroundUI backgroundUI, DialogueUI dialogueUI,
            UnityAction<string> onSetDialog, UnityAction onClearHistory)
        {
            _saveLoadData = saveLoadData;
            _saveLoadUI = saveLoadUI;
            _data = data;
            _onGetDialogCurrent = onGetDialogCurrent;
            _backgroundUI = backgroundUI;
            _dialogueUI = dialogueUI;
            _onSetDialog = onSetDialog;
            _onClearHistory = onClearHistory;
        }

        private readonly BackgroundUI _backgroundUI;
        private readonly GameData _data;
        private readonly DialogueUI _dialogueUI;
        private readonly Func<string> _onGetDialogCurrent;
        private readonly UnityAction<string> _onSetDialog;
        private readonly UnityAction _onClearHistory;
        private readonly ISaveLoadDataService _saveLoadData;
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
                    var texture2D = CameraTextureCapture.CaptureCameraView(
                        _backgroundUI.GetComponent<Canvas>(),
                        _dialogueUI.GetComponent<Canvas>());

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