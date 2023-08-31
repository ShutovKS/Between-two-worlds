using System.Threading.Tasks;
using UnityEngine;

namespace Infrastructure.Services.UIFactory
{
    public interface IUIFactoryService : IUIFactoryInfoService
    {
        Task CreatedMainMenuScreen();
        Task CreatedSettingsScreen();
        Task CreatedDialogueScreen();
        Task CreatedBackgroundScreen();
        Task CreatedChooseLanguageScreen();
        Task CreatedConfirmationScreen();

        void DestroyMainMenuScreen();
        void DestroyDialogueScreen();
        void DestroySettingsScreen();
        void DestroyBackgroundScreen();
        void DestroyChooseLanguageScreen();
        void DestroyConfirmationScreen();
    }
}