#region

using System.Threading.Tasks;
using Data.Dynamic;

#endregion

namespace Infrastructure.Services.SaveLoad
{
    public interface ISaveLoadService
    {
        Task<(GameData, LoadState)> Load();
        void Save(GameData gameData);
    }

    public enum LoadState
    {
        Successfully,
        NoSavedProgress,
    }
}