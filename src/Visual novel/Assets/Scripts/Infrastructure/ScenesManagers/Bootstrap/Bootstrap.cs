using Infrastructure.Services;
using Infrastructure.Services.AssetsAddressables;
using Infrastructure.Services.LocalisationDataLoad;
using Infrastructure.Services.LocalizationUI;
using Infrastructure.Services.SaveLoadData;
using Infrastructure.Services.UIFactory;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Infrastructure.ScenesManagers.Bootstrap
{
    public class Bootstrap : MonoBehaviour
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
            var localizerUI = new LocalizerUI();
            var uiFactory = new UIFactoryService(assetsAddressablesProvider);

            ServicesContainer.SetServices(
                assetsAddressablesProvider,
                saveLoadData,
                uiFactory,
                localisationDataLoad,
                localizerUI);
        }

        private void GameLoading()
        {
            SceneManager.LoadScene("1.Loading");
        }
    }
}