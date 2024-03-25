#region

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Infrastructure.Services.AssetsAddressables;
using Infrastructure.Services.Localisation;
using Infrastructure.Services.Sounds;
using Infrastructure.Services.WindowsService;
using UI;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

#endregion

namespace Infrastructure.Services.UIFactory
{
    public class UIFactoryService : IUIFactoryService
    {
        public UIFactoryService(DiContainer container, IAssetsAddressablesProviderService assetsAddressablesProvider,
            IWindowService windowService, ISoundService soundService, ILocalisationService localisationService)
        {
            _container = container;
            _assetsAddressablesProvider = assetsAddressablesProvider;
            _windowService = windowService;
            _soundService = soundService;
            _localisationService = localisationService;
        }

        private readonly DiContainer _container;
        private readonly IAssetsAddressablesProviderService _assetsAddressablesProvider;
        private readonly ILocalisationService _localisationService;
        private readonly IWindowService _windowService;
        private readonly ISoundService _soundService;

        private Dictionary<WindowID, GameObject> _screenTypeToInstanceMap = new();
        private Dictionary<Type, Component> _screenTypeToComponentMap = new();

        public async Task<GameObject> CreateScreen(string assetAddress, WindowID windowId)
        {
            var screenPrefab = await _assetsAddressablesProvider.GetAsset<GameObject>(assetAddress);
            var screenObject = _container.InstantiatePrefab(screenPrefab);

            if (!_screenTypeToInstanceMap.TryAdd(windowId, screenObject))
            {
                Debug.LogWarning($"A screen with WindowID {windowId} already exists. Replacing the existing screen object.");

                Object.Destroy(_screenTypeToInstanceMap[windowId]);

                _screenTypeToInstanceMap[windowId] = screenObject;
            }

            screenObject.GetComponent<BaseScreen>()?.Construct(_windowService, _soundService);
            screenObject.GetComponent<ILocalizableUI>()?.Localize(_localisationService.GetUILocalisation());

            TryInitializeScreen(screenObject, windowId);

            return screenObject;
        }

        public Task<T> GetScreenComponent<T>(WindowID windowId) where T : Component
        {
            if (_screenTypeToInstanceMap.TryGetValue(windowId, out var screenObject))
            {
                var screenComponent = screenObject.GetComponent<T>();

                if (screenComponent == null)
                {
                    Debug.LogError($"Screen component of type {typeof(T)} not found");
                    return Task.FromResult<T>(null);
                }

                _screenTypeToComponentMap[typeof(T)] = screenComponent;
                return Task.FromResult(screenComponent);
            }

            Debug.LogError($"Screen with WindowID {windowId} not found");
            return Task.FromResult<T>(null);
        }

        public void DestroyScreen(WindowID windowId)
        {
            if (_screenTypeToInstanceMap.Remove(windowId, out var screenObject))
            {
                Object.Destroy(screenObject);
            }
            else
            {
                Debug.LogError($"Screen with WindowID {windowId} not found");
            }
        }

        private void TryInitializeScreen(GameObject screen, WindowID windowId)
        {
            switch (windowId)
            {
                case WindowID.Unknown:
                    // Example: screen.Initialize();
                    break;
            }
        }
    }
}