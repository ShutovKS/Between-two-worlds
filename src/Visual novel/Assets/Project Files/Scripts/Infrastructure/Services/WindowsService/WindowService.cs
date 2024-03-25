using System.Threading.Tasks;
using Infrastructure.Services.UIFactory;
using UnityEngine;

namespace Features.Services.WindowsService
{
    public class WindowService : IWindowService
    {
        public WindowService(IUIFactoryService uiFactory)
        {
            _uiFactory = uiFactory;
        }

        private readonly IUIFactoryService _uiFactory;

        public async Task Open(WindowID windowID)
        {
            await OpenWindow(windowID);
        }

        public async Task<T> OpenAndGetComponent<T>(WindowID windowID) where T : Component
        {
            await OpenWindow(windowID);

            var component = await _uiFactory.GetScreenComponent<T>(windowID);

            return component;
        }

        private async Task OpenWindow(WindowID windowID)
        {
            var windowsPath = GetWindowsPath(windowID);

            if (windowsPath != null) 
            {
                await _uiFactory.CreateScreen(windowsPath, windowID);
            }
        }
        
        public void Close(WindowID windowID)
        {
            if (windowID == WindowID.Unknown)
            {
                Debug.Log("Unknown window id + " + windowID);
                return;
            }

            _uiFactory.DestroyScreen(windowID);
        }

        private static string GetWindowsPath(WindowID windowID)
        {
            return windowID switch
            {
                // WindowID.Loading => AssetsAddressableConstants.LOADING_SCREEN,
                _ => null
            };
        }
    }
}