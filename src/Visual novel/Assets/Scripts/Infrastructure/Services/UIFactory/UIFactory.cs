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

        public void DestroyMainMenuScreen()
        {
            Destroy(MainMenuScreen);
        }

        public void DestroyDialogueScreen()
        {
            Destroy(DialogueScreen);
        }
    }
}