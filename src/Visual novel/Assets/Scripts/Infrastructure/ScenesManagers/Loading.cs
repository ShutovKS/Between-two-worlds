﻿using System;
using System.Collections;
using System.Threading.Tasks;
using Data.Dynamic;
using Infrastructure.Services;
using Infrastructure.Services.LocalisationDataLoad;
using Infrastructure.Services.PersistentData;
using Infrastructure.Services.SaveLoadData;
using Infrastructure.Services.UIFactory;
using Units.Tools;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Infrastructure.ScenesManagers
{
    public class Loading : MonoBehaviour
    {
        private ILocalisationDataLoadService _localisationDataLoad;
        private IPersistentDataService _persistentData;
        private ISaveLoadDataService _saveLoadData;
        private IUIFactoryInfoService _uiFactoryInfo;
        private IUIFactoryService _uiFactory;

        private string _language = "";

        private async void Start()
        {
            InitializedServices();
            await CreatedUI();
            await LoadData();
            Localisation();
            StartGame();
        }

        private void InitializedServices()
        {
            _localisationDataLoad = ServicesContainer.GetService<ILocalisationDataLoadService>();
            _persistentData = ServicesContainer.GetService<IPersistentDataService>();
            _saveLoadData = ServicesContainer.GetService<ISaveLoadDataService>();
            _uiFactoryInfo = ServicesContainer.GetService<IUIFactoryInfoService>();
            _uiFactory = ServicesContainer.GetService<IUIFactoryService>();
        }

        private async Task CreatedUI()
        {
            await _uiFactory.CreatedBackgroundScreen();
            _uiFactoryInfo.BackgroundUI.SetBackgroundColor(Color.black);

            await _uiFactory.CreatedChooseLanguageScreen();

            await _uiFactory.CreatedDialogueScreen();
            _uiFactoryInfo.DialogueUI.SetActivePanel(false);

            await _uiFactory.CreatedSettingsScreen();
            _uiFactoryInfo.SettingsUI.SetActivePanel(false);

            await _uiFactory.CreatedMainMenuScreen();
            _uiFactoryInfo.MainMenuUI.SetActivePanel(false);
        }

        private async Task LoadData()
        {
            if (_saveLoadData.Exists())
            {
                var dynamicData = _saveLoadData.Load();
                _persistentData.SetDynamicData(dynamicData);
            }
            else
            {
                var dynamicData = new DynamicData();
                _saveLoadData.Save(dynamicData);
                _persistentData.SetDynamicData(dynamicData);
            }

            var localizationsInfo = _localisationDataLoad.GetLocalizationsInfo();
            var addLanguageInScrollView = new Action<string, UnityAction>(
                _uiFactoryInfo.ChooseLanguageUI.ScrollViewLanguages.AddLanguageInScrollView);

            foreach (var localizationInfo in localizationsInfo)
            {
                addLanguageInScrollView.Invoke(
                    localizationInfo.Language,
                    () => _language = localizationInfo.Language);
            }

            await Task.Run(
                () =>
                {
                    while (_language == "")
                    {
                    }
                });

            _localisationDataLoad.Load(_language);
        }

        private void Localisation()
        {
            var uiLocalisation = _localisationDataLoad.GetUILocalisation();
            _uiFactoryInfo.MainMenuUI.Localisator(uiLocalisation);
            _uiFactoryInfo.SettingsUI.Localisator(uiLocalisation);
        }

        private void StartGame()
        {
            SceneManager.LoadScene("2.Meta");
            _uiFactoryInfo.ChooseLanguageUI.SetActivePanel(false);
        }
    }
}