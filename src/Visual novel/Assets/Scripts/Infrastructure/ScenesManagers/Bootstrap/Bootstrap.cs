﻿using Infrastructure.Services;
using Infrastructure.Services.AssetsAddressables;
using Infrastructure.Services.CoroutineRunner;
using Infrastructure.Services.LocalisationDataLoad;
using Infrastructure.Services.LocalizationUI;
using Infrastructure.Services.SaveLoadData;
using Infrastructure.Services.UIFactory;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Infrastructure.ScenesManagers.Bootstrap
{
    public class Bootstrap : MonoBehaviour, ICoroutineRunner
    {
        private void Awake()
        {
            ServicesInitialize();
            GameLoading();
        }

        private void ServicesInitialize()
        {
            var assetsAddressablesProvider = new AssetsAddressablesProviderService();
            var saveLoadData = new SaveLoadDataService();
            var localisationDataLoad = new LocalisationDataLoadService();
            var localizerUI = new LocalizerUIService();
            var uiFactory = new UIFactoryService(assetsAddressablesProvider);
            var coroutineRunner = new GameObject().AddComponent<CoroutineRunnerService>();

            ServicesContainer.SetServices(
                assetsAddressablesProvider,
                saveLoadData,
                uiFactory,
                localisationDataLoad,
                localizerUI,
                coroutineRunner);
        }

        private void GameLoading()
        {
            SceneManager.LoadScene("1.Loading");
        }
    }
}