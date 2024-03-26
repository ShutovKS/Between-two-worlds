using System.Threading.Tasks;
using UnityEngine;

namespace Infrastructure.Services.WindowsService
{
    public interface IWindowService
    {
        public Task Open(WindowID windowID);
        public Task<T> OpenAndGetComponent<T>(WindowID windowID) where T : Component;
        public void Close(WindowID windowID);
    }
}