using Data.Dynamic;
using Infrastructure.Services;
using Infrastructure.Services.LocalisationDataLoad;
using Infrastructure.Services.SaveLoadData;
using Infrastructure.Services.UIFactory;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Infrastructure.ScenesManagers.Core
{
    public class Core : MonoBehaviour
    {
        private ILocalisationDataLoadService _localisationDataLoad;
        private ISaveLoadDataService _saveLoadData;
        private IUIFactoryInfoService _uiFactoryInfo;

        private DynamicData _dataCurrent;
        private ButtonManager _buttonManager;
        private DialogueManager _dialogueManager;
        private HistoryManager _historyManager;
        private SaveLoadManager _saveLoadManager;
        private SettingsManager _settingsManager;

        private void Awake()
        {
            InitializedServices();
            LoadData();
            InitializedManagers();
            _dialogueManager.StartDialogue();
        }

        private void InitializedManagers()
        {
            _dialogueManager = new DialogueManager(
                _localisationDataLoad.GetPart,
                _uiFactoryInfo.DialogueUI,
                _uiFactoryInfo.BackgroundUI);

            _saveLoadManager = new SaveLoadManager(
                _saveLoadData,
                _uiFactoryInfo.SaveLoadUI,
                _dataCurrent,
                () => _dialogueManager.CurrentDialogue.ID,
                _uiFactoryInfo.BackgroundUI,
                _uiFactoryInfo.DialogueUI,
                _dialogueManager.SetDialog);

            _historyManager = new HistoryManager();

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