using Data.Dynamic;

namespace Infrastructure.Services.PersistentData
{
    public class PersistentDataService : IPersistentDataService
    {
        public DynamicData DynamicData { get; private set; }

        public void SetDynamicData(DynamicData data)
        {
            DynamicData = data;
        }
    }
}