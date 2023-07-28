using System.Threading.Tasks;
using UnityEngine;

namespace Infrastructure.Services.AssetsAddressables
{
    public interface IAssetsAddressablesProviderService
    {
        Task<T> GetAsset<T>(string address) where T : Object;
    }
}