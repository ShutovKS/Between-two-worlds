using System;
using Infrastructure.Services.LocalisationDataLoad;
using UI.LastWords;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Infrastructure.ScenesManagers.Core
{
    public class ActionTriggerManager
    {
        public ActionTriggerManager(LastWordsUI lastWordsUI, ILocalisationDataLoadService localisationDataLoadService, UnityAction onExitInMainMenu)
        {
            _lastWordsUI = lastWordsUI;
            _localisationDataLoadService = localisationDataLoadService;
            _onExitInMainMenu = onExitInMainMenu;
        }

        private readonly LastWordsUI _lastWordsUI;
        private readonly ILocalisationDataLoadService _localisationDataLoadService;
        private UnityAction _onExitInMainMenu;

        public void HandleActionTrigger(string actionTrigger)
        {
            switch (actionTrigger)
            {
                case "end1":
                    ActionEnd1();
                    break;
                case "end2":
                    ActionEnd2();
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(actionTrigger), actionTrigger, null);
            }
        }

        private void ActionEnd1()
        {
            _lastWordsUI.SetActivePanel(true);
            _lastWordsUI.SetText("End1");
            _lastWordsUI.RegisterBackButtonCallback(() =>
            {
                _lastWordsUI.SetActivePanel(false);
                _onExitInMainMenu();
            });
        }

        private void ActionEnd2()
        {
            _lastWordsUI.SetActivePanel(true);
            _lastWordsUI.SetText("End2");
            _lastWordsUI.RegisterBackButtonCallback(() =>
            {
                _lastWordsUI.SetActivePanel(false);
                _onExitInMainMenu();
            });
        }
    }
}
