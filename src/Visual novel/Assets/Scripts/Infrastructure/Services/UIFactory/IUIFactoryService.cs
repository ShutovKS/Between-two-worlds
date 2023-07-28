using System.Threading.Tasks;
using UnityEngine;

namespace Infrastructure.Services.UIFactory
{
    public interface IUIFactoryService : IUIFactoryInfoService
    {
        Task<GameObject> CreatedMainMenuScreen();
        Task<GameObject> CreatedSettingsScreen();
        Task<GameObject> CreatedDialogueScreen();
        Task<GameObject> CreatedBackgroundScreen();
        
        void DestroyMainMenuScreen();
        void DestroyDialogueScreen();
        void DestroySettingsScreen();
        void DestroyBackgroundScreen();
    }
}