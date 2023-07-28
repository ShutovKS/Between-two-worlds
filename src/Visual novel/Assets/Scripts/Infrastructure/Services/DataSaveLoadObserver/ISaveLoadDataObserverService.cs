using System.Collections.Generic;
using UnityEngine;

namespace Infrastructure.Services.DataLoadObserver
{
    public interface ISaveLoadDataObserverService
    {
        List<IDataLoadObserverService> LoadObservers { get; }
        List<IDataSaveObserverService> SaveObservers { get; }
        void RegisterObserver(GameObject gameObject);
        void RemoveObserver(GameObject gameObject);
        void RemoveAll();
    }
}