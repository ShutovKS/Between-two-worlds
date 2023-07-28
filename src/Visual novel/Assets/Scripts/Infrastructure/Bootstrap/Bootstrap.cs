using Infrastructure.Services;
using Infrastructure.Services.AssetsAddressables;
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
            var uiFactory = new UIFactoryService(assetsAddressablesProvider);
            var staticDataLoad = new StaticDataLoadService();

            ServicesContainer.SetUp(assetsAddressablesProvider, uiFactory, staticDataLoad);
        }
    }
}