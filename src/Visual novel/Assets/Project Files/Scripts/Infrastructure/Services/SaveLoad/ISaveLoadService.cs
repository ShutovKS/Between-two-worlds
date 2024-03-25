#region

using Data.Dynamic;

#endregion

namespace Infrastructure.Services.SaveLoad
{
    public interface ISaveLoadService
    {
        GameData Load(out LoadState loadState);
        void Save(GameData gameData);
        
    }

    public enum LoadState
    {
        Successfully,
        NoSavedProgress,
    }
}