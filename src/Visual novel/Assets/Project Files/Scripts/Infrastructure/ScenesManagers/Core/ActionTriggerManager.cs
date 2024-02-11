#region

using System;
using Data.Constant;
using Infrastructure.Services.LocalisationDataLoad;
using Infrastructure.Services.UIFactory;
using UnityEngine;
using UnityEngine.Events;

#endregion

namespace Infrastructure.ScenesManagers.Core
{
    public class ActionTriggerManager
    {
        public ActionTriggerManager(IUIFactoryInfoService uiFactoryInfoService,
            ILocalisationDataLoadService localisationDataLoadService, UnityAction onExitInMainMenu)
        {
            _uiFactoryInfoService = uiFactoryInfoService;
            _localisationDataLoadService = localisationDataLoadService;
            _onExitInMainMenu = onExitInMainMenu;
        }

        private readonly IUIFactoryInfoService _uiFactoryInfoService;
        private readonly ILocalisationDataLoadService _localisationDataLoadService;
        private readonly UnityAction _onExitInMainMenu;

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
                default:
                    throw new ArgumentOutOfRangeException(nameof(actionTrigger), actionTrigger, null);
            }
        }

        private void ActionEnd1()
        {
            var text = _localisationDataLoadService.GetUpLastWord("end1");
#if YG_SERVICES
            YandexMetrica.Send("end1");
#endif
            SetUpLastWordsUI(text);
        }

        private void ActionEnd2()
        {
            var text = _localisationDataLoadService.GetUpLastWord("end2");
#if YG_SERVICES
            YandexMetrica.Send("end2");
#endif
            SetUpLastWordsUI(text);
        }

        private void SetUpLastWordsUI(string text)
        {
            _uiFactoryInfoService.BackgroundUI.SetBackgroundImage(
                Resources.Load<Texture2D>("Data/Backgrounds/" + ResourcesPath.BACKGROUND_PATH));
            _uiFactoryInfoService.DialogueUI.SetActivePanel(false);
            _uiFactoryInfoService.LastWordsUI.SetActivePanel(true);
            _uiFactoryInfoService.LastWordsUI.SetText(text);
            _uiFactoryInfoService.LastWordsUI.RegisterBackButtonCallback(() =>
            {
                _uiFactoryInfoService.LastWordsUI.SetActivePanel(false);
                _onExitInMainMenu?.Invoke();
            });
        }
    }
}