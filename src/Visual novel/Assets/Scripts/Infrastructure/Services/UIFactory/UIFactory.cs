using System.Threading.Tasks;
using Infrastructure.Services.AssetsAddressables;
using UnityEngine;
using static Data.AssetsAddressablesContainer.AssetsAddressablesContainer;
using static UnityEngine.Object;

namespace Infrastructure.Services.UIFactory
{
    public class UIFactory : IUIFactory
    {
        public UIFactory(IAssetsAddressablesProvider assetsAddressablesProvider)
        {
            _assetsAddressablesProvider = assetsAddressablesProvider;
        }

        private readonly IAssetsAddressablesProvider _assetsAddressablesProvider;

        public GameObject DialogueScreen { get; private set; }
        public GameObject MainMenuScreen { get; private set; }
        public GameObject SettingsScreen { get; private set; }
        public GameObject BackgroundScreen { get; private set; }

        public async Task<GameObject> CreatedMainMenuScreen()
        {
            var prefab = await _assetsAddressablesProvider.GetAsset<GameObject>(MAIN_MENU_SCREEN);
            MainMenuScreen = Instantiate(prefab);
            return MainMenuScreen;
        }

        public async Task<GameObject> CreatedDialogueScreen()
        {
            var prefab = await _assetsAddressablesProvider.GetAsset<GameObject>(DIALOGUE_SCREEN);
            DialogueScreen = Instantiate(prefab);
            return DialogueScreen;
        }

        public async Task<GameObject> CreatedBackgroundScreen()
        {
            var prefab = await _assetsAddressablesProvider.GetAsset<GameObject>(BACKGROUND_SCREEN);
            BackgroundScreen = Instantiate(prefab);
            return BackgroundScreen;
        }

        public async Task<GameObject> CreatedSettingsScreen()
        {
            var prefab = await _assetsAddressablesProvider.GetAsset<GameObject>(SETTINGS_SCREEN);
            SettingsScreen = Instantiate(prefab);
            return SettingsScreen;
        }

        public void DestroyMainMenuScreen() => Destroy(MainMenuScreen);
        public void DestroyDialogueScreen() => Destroy(DialogueScreen);
        public void DestroySettingsScreen() => Destroy(SettingsScreen);
        public void DestroyBackgroundScreen() => Destroy(BackgroundScreen);
    }
}