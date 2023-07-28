using Data.Dynamic;

namespace Infrastructure.Services.DataLoadObserver
{
    public interface IDataSaveObserverService
    {
        void SetData(DynamicData data);
    }
}