using System.Threading.Tasks;
using UnityEngine;

namespace Infrastructure.Services.AssetsAddressables
{
    public interface IAssetsAddressablesProvider
    {
        Task<T> GetAsset<T>(string address) where T : Object;
    }
}