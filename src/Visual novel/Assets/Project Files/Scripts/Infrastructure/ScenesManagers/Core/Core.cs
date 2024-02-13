#region

using Data.Dynamic;
using Infrastructure.Services;
using Infrastructure.Services.CoroutineRunner;
using Infrastructure.Services.LocalisationDataLoad;
using Infrastructure.Services.SaveLoadData;
using Infrastructure.Services.Sounds;
using Infrastructure.Services.UIFactory;
using UnityEngine;
using UnityEngine.SceneManagement;

#endregion

namespace Infrastructure.ScenesManagers.Core
{
    public class Core : MonoBehaviour
    {
        private ButtonManager _buttonManager;
        private ICoroutineRunnerService _coroutineRunnerService;

        private GameData _dataCurrent;
        private DialogueManager _dialogueManager;
        private HistoryManager _historyManager;
        private ILocalisationDataLoadService _localisationDataLoad;
        private ISaveLoadDataService _saveLoadData;
        private SaveLoadManager _saveLoadManager;
        private ActionTriggerManager _actionTriggerManager;
        private IUIFactoryInfoService _uiFactoryInfo;
        private ISoundsService _soundsService;

        private void Awake()
        {
            InitializedServices();
            LoadData();
            InitializedManagers();
            _dialogueManager.StartDialogue();
        }

        private void InitializedManagers()
        {
            _actionTriggerManager = new ActionTriggerManager(_uiFactoryInfo, _localisationDataLoad, ExitInMenu);

            _historyManager = new HistoryManager(_uiFactoryInfo.DialogueUI.History);

            _dialogueManager = new DialogueManager(
                _localisationDataLoad.GetPhraseId,
                _uiFactoryInfo.DialogueUI,
                _uiFactoryInfo.BackgroundUI,
                _coroutineRunnerService,
                _soundsService,
                _historyManager.AddedDialogInHistory,
                _actionTriggerManager.HandleActionTrigger);

            _saveLoadManager = new SaveLoadManager(
                _saveLoadData,
                _uiFactoryInfo.SaveLoadUI,
                _dataCurrent,
                () => _dialogueManager.CurrentDialogue.ID,
                _uiFactoryInfo.BackgroundUI,
                _uiFactoryInfo.DialogueUI,
                _dialogueManager.SetDialog,
                _historyManager.ClearHistory);

            _buttonManager = new ButtonManager(_uiFactoryInfo.DialogueUI.Buttons);
            _buttonManager.RegisterOnClickBack(ConfirmExitInMenu);
            _buttonManager.RegisterOnClickSave(_saveLoadManager.OpenDataSave);
            _buttonManager.RegisterOnClickLoad(_saveLoadManager.OpenDataLoad);
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
            _coroutineRunnerService = ServicesContainer.GetService<ICoroutineRunnerService>();
            _soundsService = ServicesContainer.GetService<ISoundsService>();
        }

        private void LoadData()
        {
            _dataCurrent = _saveLoadData.LoadOrCreateNew();
        }

        private void ConfirmExitInMenu()
        {
            _dialogueManager.StopAutoDialogSwitchMode();
            _uiFactoryInfo.ConfirmationUI.SetActivePanel(true);
            _uiFactoryInfo.ConfirmationUI.Buttons.RegisterYesButtonCallback(ExitInMenu);
            _uiFactoryInfo.ConfirmationUI.Buttons.RegisterNoButtonCallback(() =>
            {
                _uiFactoryInfo.ConfirmationUI.SetActivePanel(false);
            });
        }

        private void ExitInMenu()
        {
            SceneManager.LoadScene("2.Meta");
            _uiFactoryInfo.ConfirmationUI.SetActivePanel(false);
            _uiFactoryInfo.DialogueUI.SetActivePanel(false);
        }
    }
}