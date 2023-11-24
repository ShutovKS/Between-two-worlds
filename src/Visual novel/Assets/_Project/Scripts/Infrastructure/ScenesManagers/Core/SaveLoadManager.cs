#region

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

#endregion

namespace Infrastructure.ScenesManagers.Core
{
	public class SaveLoadManager
	{
		public SaveLoadManager(ISaveLoadDataService saveLoadData, SaveLoadUI saveLoadUI, DynamicData data,
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
		private readonly DynamicData _data;
		private readonly DialogueUI _dialogueUI;
		private readonly Func<string> _onGetDialogCurrent;
		private readonly UnityAction<string> _onSetDialog;
		private readonly UnityAction _onClearHistory;
		private readonly ISaveLoadDataService _saveLoadData;
		private readonly SaveLoadUI _saveLoadUI;

		public void DataSave()
		{
			_saveLoadUI.SetActivePanel(true);
			_saveLoadUI.ButtonsUI.RegisterBackButtonCallback(
				() => { _saveLoadUI.SetActivePanel(false); });

			var number = 0;
			foreach (var ui in _saveLoadUI.SaveDataUIs)
			{
				var n = number;
				var data = _data.dialogues[number++];
				if (data.isDataExist)
				{
					ui.SetImage(data.background);
					ui.SetTitle(data.titleText);
				}

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
							background = texture2D,
							isDataExist = true
						};

						_saveLoadData.Save(_data);

						var saveDataUI = _saveLoadUI.SaveDataUIs[n];
						saveDataUI.SetTitle(titleText);
						saveDataUI.SetImage(texture2D);
						saveDataUI.RegisterButtonCallback(null);
					});
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
				var data = _data.dialogues[number++];

				if (data.isDataExist)
				{
					ui.SetImage(data.background);
					ui.SetTitle(data.titleText);
					ui.RegisterButtonCallback(
						() =>
						{
							_onClearHistory?.Invoke();
							_onSetDialog.Invoke(data.idLastDialogue);
							_saveLoadUI.SetActivePanel(false);
						});
				}
				else
				{
					ui.RegisterButtonCallback(null);
				}
			}
		}
	}
}