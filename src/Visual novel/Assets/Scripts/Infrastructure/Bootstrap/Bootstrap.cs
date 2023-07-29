using Infrastructure.Services;
using Infrastructure.Services.AssetsAddressables;
using Infrastructure.Services.DataLoadObserver;
using Infrastructure.Services.PersistentData;
using Infrastructure.Services.SaveLoadData;
using Infrastructure.Services.StaticDataLoad;
using Infrastructure.Services.UIFactory;
using UnityEngine;

namespace Infrastructure.Bootstrap
{
    public class Bootstrap : MonoBehaviour
    {
        private void Awake()
        {
            ServicesInitialize();
        }

        private void ServicesInitialize()
        {
            var assetsAddressablesProvider = new AssetsAddressablesProviderService();
            var saveLoadDataObserver = new SaveLoadDataObserverService();
            var persistentDataService = new PersistentDataService();
            var saveLoadData = new SaveLoadDataService();
            var staticDataLoad = new StaticDataLoadService();
            var uiFactory = new UIFactoryService(assetsAddressablesProvider);

            ServicesContainer.SetUp(
                assetsAddressablesProvider,
                saveLoadDataObserver,
                persistentDataService,
                saveLoadData,
                uiFactory,
                staticDataLoad);
        }
    }
}