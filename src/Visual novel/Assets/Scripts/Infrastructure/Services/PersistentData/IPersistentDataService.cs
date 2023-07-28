using Data.Dynamic;

namespace Infrastructure.Services.PersistentData
{
    public interface IPersistentDataService
    {
        DynamicData DynamicData { get; }
        void SetDynamicData(DynamicData data);
    }
}