using Data.Dynamic;

namespace Infrastructure.Services.DataLoadObserver
{
    public interface IDataLoadObserverService
    {
        void GetData(DynamicData data);
    }
}