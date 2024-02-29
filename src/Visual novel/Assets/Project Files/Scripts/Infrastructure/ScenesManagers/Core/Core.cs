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
using static Data.Constant.PlayerPrefsPath;

#endregion

namespace Infrastructure.ScenesManagers.Core
{
    public class Core : MonoBehaviour
    {
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

        private string _idDialogue;

        private void Awake()
        {
            InitializedServices();
            LoadData();
            InitializedManagers();

            _dialogueManager.StartDialogue(_idDialogue);
        }

        private void InitializedManagers()
        {
            _actionTriggerManager = new ActionTriggerManager(_uiFactoryInfo, _localisationDataLoad)
            {
                OnExitInMainMenu = ExitInMenu
            };

            _historyManager = new HistoryManager(_uiFactoryInfo.DialogueUI.History);

            _dialogueManager = new DialogueManager(
                _uiFactoryInfo.DialogueUI,
                _uiFactoryInfo.BackgroundUI,
                _coroutineRunnerService,
                _soundsService)
            {
                OnGetPart = _localisationDataLoad.GetPhraseId,
                OnNewDialog = _historyManager.AddedDialogInHistory,
                HandleActionTrigger = _actionTriggerManager.HandleActionTrigger
            };
            _dialogueManager.OnNewDialog += (id, _, _) => SetNewCurrentDialogue(id);

            _saveLoadManager = new SaveLoadManager(
                _saveLoadData,
                _uiFactoryInfo.SaveLoadUI,
                _dataCurrent,
                _uiFactoryInfo.ImageCaptureForSaveUI,
                _localisationDataLoad)
            {
                OnGetDialogCurrent = () => _dialogueManager.CurrentDialogue.ID,
                OnSetDialog = _dialogueManager.SetDialog,
                OnClearHistory = _historyManager.ClearHistory
            };

            _uiFactoryInfo.DialogueUI.Buttons.OnBackButtonClicked = ConfirmExitInMenu;
            _uiFactoryInfo.DialogueUI.Buttons.OnSaveButtonClicked = _saveLoadManager.OpenDataSave;
            _uiFactoryInfo.DialogueUI.Buttons.OnLoadButtonClicked = _saveLoadManager.OpenDataLoad;
            _uiFactoryInfo.DialogueUI.Buttons.OnHistoryButtonClicked = _historyManager.OpenDialogHistory;
            _uiFactoryInfo.DialogueUI.Buttons.OnSpeedUpButtonClicked = _dialogueManager.ChangeTypingDialogSpeedUp;
            _uiFactoryInfo.DialogueUI.Buttons.OnAutoButtonClicked = _dialogueManager.AutoDialogSwitchMode;
            _uiFactoryInfo.DialogueUI.Buttons.OnFurtherButtonClicked = _dialogueManager.DialogFurther;
        }

        private void InitializedServices()
        {
            _localisationDataLoad = ServicesContainer.GetService<ILocalisationDataLoadService>();
            _coroutineRunnerService = ServicesContainer.GetService<ICoroutineRunnerService>();
            _uiFactoryInfo = ServicesContainer.GetService<IUIFactoryInfoService>();
            _saveLoadData = ServicesContainer.GetService<ISaveLoadDataService>();
            _soundsService = ServicesContainer.GetService<ISoundsService>();
        }

        private void LoadData()
        {
            _dataCurrent = _saveLoadData.GetData();
            _idDialogue = _dataCurrent.currentDialogue;
        }

        private void ConfirmExitInMenu()
        {
            _dialogueManager.StopAutoDialogSwitchMode();
            _uiFactoryInfo.ConfirmationUI.SetActivePanel(true);
            _uiFactoryInfo.ConfirmationUI.Buttons.OnYesButtonClicked = ExitInMenu;
            _uiFactoryInfo.ConfirmationUI.Buttons.OnNoButtonClicked = () =>
            {
                _uiFactoryInfo.ConfirmationUI.SetActivePanel(false);
            };
        }

        private void SetNewCurrentDialogue(string id)
        {
            _dataCurrent.currentDialogue = id;
            _saveLoadData.Save(_dataCurrent);
        }

        private void ExitInMenu()
        {
            SceneManager.LoadScene("2.Meta");

            _uiFactoryInfo.ConfirmationUI.SetActivePanel(false);
            _uiFactoryInfo.DialogueUI.SetActivePanel(false);
        }
    }
}