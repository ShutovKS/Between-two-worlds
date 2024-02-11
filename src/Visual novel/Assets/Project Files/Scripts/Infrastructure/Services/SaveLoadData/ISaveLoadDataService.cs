#region

using Data.Dynamic;

#endregion

namespace Infrastructure.Services.SaveLoadData
{
    public interface ISaveLoadDataService
    {
        GameData LoadOrCreateNew();
        void Save(GameData gameData);
        bool Exists();
        void Remove();
    }
}