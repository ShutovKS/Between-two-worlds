#region

using System.Threading.Tasks;
using Features.Services.WindowsService;
using UnityEngine;

#endregion

namespace Infrastructure.Services.UIFactory
{
    public interface IUIFactoryService
    {
        Task<GameObject> CreateScreen(string assetAddress, WindowID windowId);
        Task<T> GetScreenComponent<T>(WindowID windowId) where T : Component;
        void DestroyScreen(WindowID windowId);
    }
}