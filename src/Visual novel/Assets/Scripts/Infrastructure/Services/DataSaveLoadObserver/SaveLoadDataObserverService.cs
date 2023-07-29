using System.Collections.Generic;
using UnityEngine;

namespace Infrastructure.Services.DataSaveLoadObserver
{
    public class SaveLoadDataObserverService : ISaveLoadDataObserverService
    {
        public List<IDataLoadObserverService> LoadObservers { get; } = new();
        public List<IDataSaveObserverService> SaveObservers { get; } = new();

        public void RegisterObserver(object obj)
        {
            if (obj is IDataLoadObserverService loadObserver)
            {
                LoadObservers.Add(loadObserver);
            }

            if (obj is IDataSaveObserverService saveObserver)
            {
                SaveObservers.Add(saveObserver);
            }
        }

        public void RemoveObserver(object obj)
        {
            if (obj is IDataLoadObserverService loadObserver)
            {
                LoadObservers.Remove(loadObserver);
            }

            if (obj is IDataSaveObserverService saveObserver)
            {
                SaveObservers.Remove(saveObserver);
            }
        }

        public void RemoveAll()
        {
            LoadObservers.Clear();
            SaveObservers.Clear();
        }
    }
}