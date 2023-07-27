﻿using Infrastructure.Services;
using Infrastructure.Services.AssetsAddressables;
using Infrastructure.Services.JsonLoad;
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
            var assetsAddressablesProvider = new AssetsAddressablesProvider();
            var uiFactory = new UIFactory(assetsAddressablesProvider);
            var staticDataLoad = new StaticDataLoad();

            ServicesContainer.SetUp(assetsAddressablesProvider, uiFactory, staticDataLoad);
        }
    }
}