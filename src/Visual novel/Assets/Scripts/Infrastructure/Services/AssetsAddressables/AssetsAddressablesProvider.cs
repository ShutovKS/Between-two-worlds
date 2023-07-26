﻿using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Infrastructure.Services.AssetsAddressables
{
    public class AssetsAddressablesProvider : IAssetsAddressablesProvider
    {
        private readonly Dictionary<string, Object> _cachedAssets = new();

        public async Task<T> GetAsset<T>(string address) where T : Object
        {
            if (_cachedAssets.TryGetValue(address, out var cachedAsset))
            {
                return cachedAsset as T;
            }

            var handle = Addressables.LoadAssetAsync<T>(address);
            await handle.Task;

            _cachedAssets[address] = handle.Result;

            return handle.Result;
        }
    }
}