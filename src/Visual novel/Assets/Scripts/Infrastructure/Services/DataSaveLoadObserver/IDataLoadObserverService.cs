using Data.Dynamic;

namespace Infrastructure.Services.DataSaveLoadObserver
{
    public interface IDataLoadObserverService
    {
        void GetData(DynamicData data);
    }
}