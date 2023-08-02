using Infrastructure.Services;
using Infrastructure.Services.AssetsAddressables;
using Infrastructure.Services.DataSaveLoadObserver;
using Infrastructure.Services.LocalisationDataLoad;
using Infrastructure.Services.PersistentData;
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
            var saveLoadDataObserver = new SaveLoadDataObserverService();
            var persistentData = new PersistentDataService();
            var saveLoadData = new SaveLoadDataService();
            var localisationDataLoad = new LocalisationDataLoadService();
            var uiFactory = new UIFactoryService(assetsAddressablesProvider);

            ServicesContainer.SetServices(
                assetsAddressablesProvider,
                saveLoadDataObserver,
                persistentData,
                saveLoadData,
                uiFactory,
                localisationDataLoad);
        }

        private void GameLoading()
        {
            SceneManager.LoadScene("1.Loading");
        }
    }
}