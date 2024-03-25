using System.Threading.Tasks;
using Data.Constant;
using Infrastructure.Services.UIFactory;
using UnityEngine;

namespace Infrastructure.Services.WindowsService
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
                WindowID.Background => AssetsAddressablesPath.BACKGROUND_SCREEN,
                WindowID.ChooseLanguage => AssetsAddressablesPath.CHOOSE_LANGUAGE_SCREEN,
                WindowID.MainMenu => AssetsAddressablesPath.MAIN_MENU_SCREEN,
                WindowID.Dialogue => AssetsAddressablesPath.DIALOGUE_SCREEN,
                WindowID.Confirmation => AssetsAddressablesPath.SAVE_LOAD_SCREEN,
                WindowID.LastWords => AssetsAddressablesPath.LAST_WORDS_SCREEN,
                WindowID.ImageCaptureForSave => AssetsAddressablesPath.IMAGE_CAPTURE_FOR_SAVE,
                _ => null
            };
        }
    }
}