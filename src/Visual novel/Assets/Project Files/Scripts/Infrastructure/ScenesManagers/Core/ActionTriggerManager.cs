#region

using System;
using Data.Constant;
using Infrastructure.Services.LocalisationDataLoad;
using Infrastructure.Services.Metric;
using Infrastructure.Services.UIFactory;
using UnityEngine;
using UnityEngine.Events;

#endregion

namespace Infrastructure.ScenesManagers.Core
{
    public class ActionTriggerManager
    {
        public ActionTriggerManager(IUIFactoryInfoService uiFactoryInfo,
            ILocalisationDataLoadService localisationDataLoad, IMetricService metric)
        {
            _uiFactoryInfo = uiFactoryInfo;
            _localisationDataLoad = localisationDataLoad;
            _metric = metric;
        }

        public UnityAction OnExitInMainMenu;

        private readonly IUIFactoryInfoService _uiFactoryInfo;
        private readonly ILocalisationDataLoadService _localisationDataLoad;
        private readonly IMetricService _metric;

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
            var text = _localisationDataLoad.GetUpLastWord("end1");

            SetUpLastWordsUI(text);
            
            _metric.SendEvent(MetricEventType.End1);
        }

        private void ActionEnd2()
        {
            var text = _localisationDataLoad.GetUpLastWord("end2");
            
            SetUpLastWordsUI(text);
            
            _metric.SendEvent(MetricEventType.End1);
        }

        private void SetUpLastWordsUI(string text)
        {
            _uiFactoryInfo.BackgroundUI.SetBackgroundImage(
                Resources.Load<Texture2D>("Data/Backgrounds/" + ResourcesPath.BACKGROUND_PATH));
            _uiFactoryInfo.DialogueUI.SetActivePanel(false);
            _uiFactoryInfo.LastWordsUI.SetActivePanel(true);
            _uiFactoryInfo.LastWordsUI.SetText(text);
            _uiFactoryInfo.LastWordsUI.OnBackButtonClicked = () =>
            {
                _uiFactoryInfo.LastWordsUI.SetActivePanel(false);
                OnExitInMainMenu?.Invoke();
            };
        }
    }
}