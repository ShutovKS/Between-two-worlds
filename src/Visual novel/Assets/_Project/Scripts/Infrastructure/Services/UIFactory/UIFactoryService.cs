﻿#region

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Infrastructure.Services.AssetsAddressables;
using UI.Background;
using UI.ChooseLanguage;
using UI.Confirmation;
using UI.Dialogue;
using UI.LastWords;
using UI.MainMenu;
using UI.SaveLoad;
using UnityEngine;
using static Data.AssetsAddressablesContainer.AssetsAddressablesContainer;
using static UnityEngine.Object;

#endregion

namespace Infrastructure.Services.UIFactory
{
    public class UIFactoryService : IUIFactoryService
    {
        public UIFactoryService(IAssetsAddressablesProviderService assetsAddressablesProviderService)
        {
            _assetsAddressablesProviderService = assetsAddressablesProviderService;
        }

        private readonly IAssetsAddressablesProviderService _assetsAddressablesProviderService;

        private readonly Dictionary<Type, GameObject> _screens = new();

        public DialogueUI DialogueUI { get; private set; }
        public MainMenuUI MainMenuUI { get; private set; }
        public BackgroundUI BackgroundUI { get; private set; }
        public ChooseLanguageUI ChooseLanguageUI { get; private set; }
        public ConfirmationUI ConfirmationUI { get; private set; }
        public SaveLoadUI SaveLoadUI { get; private set; }
        public LastWordsUI LastWordsUI { get; private set; }

        public async Task CreatedMainMenuScreen()
        {
            var prefab = await _assetsAddressablesProviderService.GetAsset<GameObject>(MAIN_MENU_SCREEN);
            var mainMenuScreen = Instantiate(prefab);

            DontDestroyOnLoad(mainMenuScreen);

            MainMenuUI = mainMenuScreen.TryGetComponent(out MainMenuUI ui)
                ? ui
                : throw new Exception($"No {ui.GetType()} in gameObject");

            _screens.Add(typeof(MainMenuUI), mainMenuScreen);
        }

        public async Task CreatedDialogueScreen()
        {
            var prefab = await _assetsAddressablesProviderService.GetAsset<GameObject>(DIALOGUE_SCREEN);
            var dialogueScreen = Instantiate(prefab);

            DontDestroyOnLoad(dialogueScreen);

            DialogueUI = dialogueScreen.TryGetComponent(out DialogueUI ui)
                ? ui
                : throw new Exception($"No {ui.GetType()} in gameObject");

            _screens.Add(typeof(DialogueUI), dialogueScreen);
        }

        public async Task CreatedBackgroundScreen()
        {
            var prefab = await _assetsAddressablesProviderService.GetAsset<GameObject>(BACKGROUND_SCREEN);
            var backgroundScreen = Instantiate(prefab);

            DontDestroyOnLoad(backgroundScreen);

            BackgroundUI = backgroundScreen.TryGetComponent(out BackgroundUI ui)
                ? ui
                : throw new Exception($"No {ui.GetType()} in gameObject");

            _screens.Add(typeof(BackgroundUI), backgroundScreen);
        }

        public async Task CreatedChooseLanguageScreen()
        {
            var prefab = await _assetsAddressablesProviderService.GetAsset<GameObject>(CHOOSE_LANGUAGE_SCREEN);
            var chooseLanguageScreen = Instantiate(prefab);

            DontDestroyOnLoad(chooseLanguageScreen);

            ChooseLanguageUI = chooseLanguageScreen.TryGetComponent(out ChooseLanguageUI ui)
                ? ui
                : throw new Exception($"No {ui.GetType()} in gameObject");

            _screens.Add(typeof(ChooseLanguageUI), chooseLanguageScreen);
        }

        public async Task CreatedConfirmationScreen()
        {
            var prefab = await _assetsAddressablesProviderService.GetAsset<GameObject>(CONFIRMATION_SCREEN);
            var confirmationScreen = Instantiate(prefab);

            DontDestroyOnLoad(confirmationScreen);

            ConfirmationUI = confirmationScreen.TryGetComponent(out ConfirmationUI ui)
                ? ui
                : throw new Exception($"No {ui.GetType()} in gameObject");

            _screens.Add(typeof(ConfirmationUI), confirmationScreen);
        }

        public async Task CreatedSaveLoadScreen()
        {
            var prefab = await _assetsAddressablesProviderService.GetAsset<GameObject>(SAVE_LOAD_SCREEN);
            var saveLoadScreen = Instantiate(prefab);

            DontDestroyOnLoad(saveLoadScreen);

            SaveLoadUI = saveLoadScreen.TryGetComponent(out SaveLoadUI ui)
                ? ui
                : throw new Exception($"No {ui.GetType()} in gameObject");

            _screens.Add(typeof(SaveLoadUI), saveLoadScreen);
        }
        public async Task CreatedLastWordsScreen()
        {
            var prefab = await _assetsAddressablesProviderService.GetAsset<GameObject>(LAST_WORDS_SCREEN);
            var lastWordsScreen = Instantiate(prefab);

            DontDestroyOnLoad(lastWordsScreen);

            LastWordsUI = lastWordsScreen.TryGetComponent(out LastWordsUI ui)
                ? ui
                : throw new Exception($"No {ui.GetType()} in gameObject");
        }

        public void DestroyMainMenuScreen()
        {
            Destroy(_screens[typeof(MainMenuUI)]);
        }

        public void DestroyDialogueScreen()
        {
            Destroy(_screens[typeof(DialogueUI)]);
        }

        public void DestroyBackgroundScreen()
        {
            Destroy(_screens[typeof(BackgroundUI)]);
        }

        public void DestroyChooseLanguageScreen()
        {
            Destroy(_screens[typeof(ChooseLanguageUI)]);
        }

        public void DestroyConfirmationScreen()
        {
            Destroy(_screens[typeof(ConfirmationUI)]);
        }

        public void DestroySaveLoadScreen()
        {
            Destroy(_screens[typeof(SaveLoadUI)]);
        }
        public void DestroyLastWordsScreen()
        {
            Destroy(_screens[typeof(LastWordsUI)]);
        }
    }
}
