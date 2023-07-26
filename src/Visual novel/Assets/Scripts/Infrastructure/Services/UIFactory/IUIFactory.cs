using System.Threading.Tasks;
using UnityEngine;

namespace Infrastructure.Services.UIFactory
{
    public interface IUIFactory : IUIFactoryInfo
    {
        Task<GameObject> CreatedMainMenuScreen();
        void DestroyMainMenuScreen();
        
        Task<GameObject> CreatedDialogueScreen();
        void DestroyDialogueScreen();
    }
}