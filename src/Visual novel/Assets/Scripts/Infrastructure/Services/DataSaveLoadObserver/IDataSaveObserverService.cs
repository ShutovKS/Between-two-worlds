using Data.Dynamic;

namespace Infrastructure.Services.DataSaveLoadObserver
{
    public interface IDataSaveObserverService
    {
        void SetData(DynamicData data);
    }
}