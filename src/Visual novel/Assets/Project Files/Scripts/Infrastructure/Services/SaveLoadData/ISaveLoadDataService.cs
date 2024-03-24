#region

using Data.Dynamic;

#endregion

namespace Infrastructure.Services.SaveLoadData
{
    public interface ISaveLoadDataService
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