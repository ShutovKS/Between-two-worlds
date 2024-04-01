using System.Threading.Tasks;
using UnityEngine;

namespace Infrastructure.Services.ScreenshotsOfSaves
{
    public interface IScreenshotsOfSavesService
    {
        Task<Texture2D> GetImage(string idDialog);
    }
}