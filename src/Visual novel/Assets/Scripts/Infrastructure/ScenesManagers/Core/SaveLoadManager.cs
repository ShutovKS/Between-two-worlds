using System;
using System.Globalization;
using Data.Dynamic;
using Infrastructure.Services.SaveLoadData;
using UI.Background;
using UI.Dialogue;
using UI.SaveLoad;
using Units.Tools;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace Infrastructure.ScenesManagers.Core
{
    public class SaveLoadManager
    {
        private ISaveLoadDataService _saveLoadData;
        private DynamicData _data;
        private readonly Func<string> _onGetDialogCurrent;
        private readonly BackgroundUI _backgroundUI;
        private readonly DialogueUI _dialogueUI;
        private readonly UnityAction<string> _onSetDialog;
        private SaveLoadUI _saveLoadUI;

        public SaveLoadManager(ISaveLoadDataService saveLoadData, SaveLoadUI saveLoadUI, DynamicData data,
            Func<string> onGetDialogCurrent, BackgroundUI backgroundUI, DialogueUI dialogueUI,
            UnityAction<string> onSetDialog)
        {
            _saveLoadData = saveLoadData;
            _saveLoadUI = saveLoadUI;
            _data = data;
            _onGetDialogCurrent = onGetDialogCurrent;
            _backgroundUI = backgroundUI;
            _dialogueUI = dialogueUI;
            _onSetDialog = onSetDialog;
        }

        public void DataSave()
        {
            _saveLoadUI.SetActivePanel(true);
            _saveLoadUI.ButtonsUI.RegisterBackButtonCallback(
                () => { _saveLoadUI.SetActivePanel(false); });

            var number = 0;
            foreach (var ui in _saveLoadUI.SaveDataUIs)
            {
                var data = _data.dialogues[number];
                var n = number;
                ui.SetImage(data.background);
                ui.SetTitle(data.titleText);
                ui.RegisterButtonCallback(
                    () =>
                    {
                        var id = _onGetDialogCurrent.Invoke();
                        var titleText = DateTime.Now.ToString(CultureInfo.InvariantCulture);
                        var texture2D = CameraTextureCapture.CaptureCameraView(
                            _backgroundUI.GetComponent<Canvas>(),
                            _dialogueUI.GetComponent<Canvas>());

                        _data.dialogues[n] = new DialoguesData
                        {
                            idLastDialogue = id,
                            titleText = titleText,
                            background = texture2D
                        };

                        _saveLoadData.Save(_data);

                        var saveDataUI = _saveLoadUI.SaveDataUIs[n];
                        saveDataUI.SetTitle(titleText);
                        saveDataUI.SetImage(texture2D);
                        saveDataUI.RegisterButtonCallback(null);
                    });

                number++;
            }
        }

        public void DataLoad()
        {
            _saveLoadUI.SetActivePanel(true);
            _saveLoadUI.ButtonsUI.RegisterBackButtonCallback(
                () => { _saveLoadUI.SetActivePanel(false); });

            var number = 0;
            foreach (var ui in _saveLoadUI.SaveDataUIs)
            {
                var data = _data.dialogues[number];
                ui.SetImage(data.background);
                ui.SetTitle(data.titleText);
                ui.RegisterButtonCallback(
                    () =>
                    {
                        _onSetDialog.Invoke(data.idLastDialogue);
                        _saveLoadUI.SetActivePanel(false);
                    });

                number++;
            }
        }
    }
}