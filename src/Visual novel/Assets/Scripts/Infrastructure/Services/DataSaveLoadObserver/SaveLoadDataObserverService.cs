using System.Collections.Generic;
using UnityEngine;

namespace Infrastructure.Services.DataLoadObserver
{
    public class SaveLoadDataObserverService : ISaveLoadDataObserverService
    {
        public List<IDataLoadObserverService> LoadObservers { get; } = new();
        public List<IDataSaveObserverService> SaveObservers { get; } = new();

        public void RegisterObserver(GameObject gameObject)
        {
            foreach (var loadObserver in gameObject.GetComponentsInChildren<IDataLoadObserverService>())
            {
                if (loadObserver is IDataSaveObserverService saveObserver)
                {
                    SaveObservers.Add(saveObserver);
                }

                LoadObservers.Add(loadObserver);
            }
        }

        public void RemoveObserver(GameObject gameObject)
        {
            foreach (var loadObserver in gameObject.GetComponentsInChildren<IDataLoadObserverService>())
            {
                if (loadObserver is IDataSaveObserverService saveObserver)
                {
                    SaveObservers.Remove(saveObserver);
                }

                LoadObservers.Remove(loadObserver);
            }
        }

        public void RemoveAll()
        {
            LoadObservers.Clear();
            SaveObservers.Clear();
        }
    }
}