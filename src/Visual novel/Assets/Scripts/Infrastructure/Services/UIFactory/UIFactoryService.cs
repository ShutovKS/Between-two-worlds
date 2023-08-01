using System;
using System.Threading.Tasks;
using Infrastructure.Services.AssetsAddressables;
using UI.Background;
using UI.ChooseLanguage;
using UI.Dialogue;
using UI.MainMenu;
using UI.Settings;
using UnityEngine;
using static Data.AssetsAddressablesContainer.AssetsAddressablesContainer;
using static UnityEngine.Object;

namespace Infrastructure.Services.UIFactory
{
    public class UIFactoryService : IUIFactoryService
    {
        public UIFactoryService(IAssetsAddressablesProviderService assetsAddressablesProviderService)
        {
            _assetsAddressablesProviderService = assetsAddressablesProviderService;
        }

        private readonly IAssetsAddressablesProviderService _assetsAddressablesProviderService;

        public DialogueUI DialogueUI { get; private set; }
        public MainMenuUI MainMenuUI { get; private set; }
        public SettingsUI SettingsUI { get; private set; }
        public BackgroundUI BackgroundUI { get; private set; }
        public ChooseLanguageUI ChooseLanguageUI { get; private set; }

        private GameObject _dialogueScreen;
        private GameObject _mainMenuScreen;
        private GameObject _settingsScreen;
        private GameObject _backgroundScreen;
        private GameObject _chooseLanguageScreen;

        public async Task<GameObject> CreatedMainMenuScreen()
        {
            var prefab = await _assetsAddressablesProviderService.GetAsset<GameObject>(MAIN_MENU_SCREEN);
            _mainMenuScreen = Instantiate(prefab);

            DontDestroyOnLoad(_mainMenuScreen);
            
            MainMenuUI = _mainMenuScreen.TryGetComponent(out MainMenuUI ui)
                ? ui
                : throw new Exception($"No {ui.GetType()} in gameObject");

            return _mainMenuScreen;
        }

        public async Task<GameObject> CreatedDialogueScreen()
        {
            var prefab = await _assetsAddressablesProviderService.GetAsset<GameObject>(DIALOGUE_SCREEN);
            _dialogueScreen = Instantiate(prefab);

            DontDestroyOnLoad(_dialogueScreen);
            
            DialogueUI = _dialogueScreen.TryGetComponent(out DialogueUI ui)
                ? ui
                : throw new Exception($"No {ui.GetType()} in gameObject");

            return _dialogueScreen;
        }

        public async Task<GameObject> CreatedBackgroundScreen()
        {
            var prefab = await _assetsAddressablesProviderService.GetAsset<GameObject>(BACKGROUND_SCREEN);
            _backgroundScreen = Instantiate(prefab);

            DontDestroyOnLoad(_backgroundScreen);

            BackgroundUI = _backgroundScreen.TryGetComponent(out BackgroundUI ui)
                ? ui
                : throw new Exception($"No {ui.GetType()} in gameObject");

            return _backgroundScreen;
        }

        public async Task<GameObject> CreatedChooseLanguageScreen()
        {
            var prefab = await _assetsAddressablesProviderService.GetAsset<GameObject>(CHOOSE_LANGUAGE_SCREEN);
            _chooseLanguageScreen = Instantiate(prefab);

            DontDestroyOnLoad(_chooseLanguageScreen);

            ChooseLanguageUI = _chooseLanguageScreen.TryGetComponent(out ChooseLanguageUI ui)
                ? ui
                : throw new Exception($"No {ui.GetType()} in gameObject");

            return _chooseLanguageScreen;
        }

        public async Task<GameObject> CreatedSettingsScreen()
        {
            var prefab = await _assetsAddressablesProviderService.GetAsset<GameObject>(SETTINGS_SCREEN);
            _settingsScreen = Instantiate(prefab);

            DontDestroyOnLoad(_settingsScreen);

            SettingsUI = _settingsScreen.TryGetComponent(out SettingsUI ui)
                ? ui
                : throw new Exception($"No {ui.GetType()} in gameObject");

            return _settingsScreen;
        }

        public void DestroyMainMenuScreen() => Destroy(_dialogueScreen);
        public void DestroyDialogueScreen() => Destroy(_mainMenuScreen);
        public void DestroySettingsScreen() => Destroy(_settingsScreen);
        public void DestroyBackgroundScreen() => Destroy(_backgroundScreen);

        public void DestroyChooseLanguageScreen()
        {
            throw new NotImplementedException();
        }
    }
}