using System.Collections.Generic;
using UnityEngine;

namespace Infrastructure.Services.DataSaveLoadObserver
{
    public interface ISaveLoadDataObserverService
    {
        List<IDataLoadObserverService> LoadObservers { get; }
        List<IDataSaveObserverService> SaveObservers { get; }
        void RegisterObserver(object obj);
        void RemoveObserver(object obj);
        void RemoveAll();
    }
}