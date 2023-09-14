﻿#region

using System;
using Data.Dynamic;
using Infrastructure.Services;
using Infrastructure.Services.CoroutineRunner;
using Infrastructure.Services.LocalisationDataLoad;
using Infrastructure.Services.SaveLoadData;
using Infrastructure.Services.UIFactory;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

#endregion

namespace Infrastructure.ScenesManagers.Core
{
	public class Core : MonoBehaviour
	{
		private ButtonManager _buttonManager;
		private ICoroutineRunner _coroutineRunner;

		private DynamicData _dataCurrent;
		private DialogueManager _dialogueManager;
		private HistoryManager _historyManager;
		private ILocalisationDataLoadService _localisationDataLoad;
		private ISaveLoadDataService _saveLoadData;
		private SaveLoadManager _saveLoadManager;
		private SettingsManager _settingsManager;
		private IUIFactoryInfoService _uiFactoryInfo;

		private void Awake()
		{
			InitializedServices();
			LoadData();
			InitializedManagers();
			_dialogueManager.StartDialogue();
		}

		private void InitializedManagers()
		{
			_historyManager = new HistoryManager(_uiFactoryInfo.DialogueUI.History);

			_dialogueManager = new DialogueManager(
				_localisationDataLoad.GetPhraseId,
				_uiFactoryInfo.DialogueUI,
				_uiFactoryInfo.BackgroundUI,
				_coroutineRunner,
				_historyManager.AddedDialogInHistory);

			_saveLoadManager = new SaveLoadManager(
				_saveLoadData,
				_uiFactoryInfo.SaveLoadUI,
				_dataCurrent,
				() => _dialogueManager.CurrentDialogue.ID,
				_uiFactoryInfo.BackgroundUI,
				_uiFactoryInfo.DialogueUI,
				_dialogueManager.SetDialog,
				_historyManager.ClearHistory);

			_settingsManager = new SettingsManager();

			_buttonManager = new ButtonManager(_uiFactoryInfo.DialogueUI.Buttons);
			_buttonManager.RegisterOnClickBack(ExitInMenu);
			_buttonManager.RegisterOnClickSave(_saveLoadManager.DataSave);
			_buttonManager.RegisterOnClickLoad(_saveLoadManager.DataLoad);
			_buttonManager.RegisterOnClickSettings(_settingsManager.OpenSettingsPanel);
			_buttonManager.RegisterOnClickHistory(_historyManager.OpenDialogHistory);
			_buttonManager.RegisterOnClickSpeedUp(_dialogueManager.ChangeTypingDialogSpeedUp);
			_buttonManager.RegisterOnClickAuto(_dialogueManager.AutoDialogSwitchMode);
			_buttonManager.RegisterOnClickFurther(_dialogueManager.DialogFurther);
		}

		private void InitializedServices()
		{
			_localisationDataLoad = ServicesContainer.GetService<ILocalisationDataLoadService>();
			_saveLoadData = ServicesContainer.GetService<ISaveLoadDataService>();
			_uiFactoryInfo = ServicesContainer.GetService<IUIFactoryInfoService>();
			_coroutineRunner = ServicesContainer.GetService<ICoroutineRunner>();
		}

		private void LoadData()
		{
			_dataCurrent = _saveLoadData.Load();
		}

		private void ExitInMenu()
		{
			_uiFactoryInfo.ConfirmationUI.SetActivePanel(true);
			_uiFactoryInfo.ConfirmationUI.Buttons.RegisterYesButtonCallback(
				() =>
				{
					SceneManager.LoadScene("2.Meta");
					_uiFactoryInfo.ConfirmationUI.SetActivePanel(false);
					_uiFactoryInfo.DialogueUI.SetActivePanel(false);
				});

			_uiFactoryInfo.ConfirmationUI.Buttons.RegisterNoButtonCallback(
				() => { _uiFactoryInfo.ConfirmationUI.SetActivePanel(false); });
		}
	}
}