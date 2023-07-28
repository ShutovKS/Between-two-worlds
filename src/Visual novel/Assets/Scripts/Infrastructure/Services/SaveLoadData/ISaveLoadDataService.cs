using Data.Dynamic;

namespace Infrastructure.Services.SaveLoadData
{
    public interface ISaveLoadDataService
    {
        DynamicData Load();
        void Save(DynamicData data);
        bool Exists();
        void Remove();
    }
}