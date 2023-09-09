#region

using System.Threading.Tasks;
using UnityEngine;

#endregion

namespace Infrastructure.Services.AssetsAddressables
{
	public interface IAssetsAddressablesProviderService
	{
		Task<T> GetAsset<T>(string address) where T : Object;
	}
}