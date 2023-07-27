using System.Threading.Tasks;
using UnityEngine;

namespace Infrastructure.Services.UIFactory
{
    public interface IUIFactory : IUIFactoryInfo
    {
        Task<GameObject> CreatedMainMenuScreen();
        Task<GameObject> CreatedSettingsScreen();
        Task<GameObject> CreatedDialogueScreen();
        
        void DestroyMainMenuScreen();
        void DestroyDialogueScreen();
        void DestroySettingsScreen();
    }
}