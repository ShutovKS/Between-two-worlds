using System;
using System.IO;
using Data.Dynamic;
using UnityEngine;

namespace Infrastructure.Services.SaveLoadData
{
    public class SaveLoadDataService : ISaveLoadDataService
    {
        public SaveLoadDataService()
        {
            var folderPath = Path.Combine(Application.persistentDataPath, FOLDER_NAME);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            _filePath = Path.Combine(folderPath, FILE_NAME);
            if (!File.Exists(_filePath))
            {
                File.CreateText(_filePath);
            }
        }

        private const string FILE_NAME = "DynamicData.txt";
        private const string FOLDER_NAME = "MyDataFolder";
        private readonly string _filePath;

        public DynamicData Load()
        {
            if (!Exists()) throw new Exception("no data for load");

            var dataString = File.ReadAllText(_filePath);
            var dataDynamic = JsonUtility.FromJson<DynamicData>(dataString);
            dataDynamic.Deserialize();
            return dataDynamic;
        }

        public void Save(DynamicData data)
        {
            data.Serialize();
            var dataString = JsonUtility.ToJson(data);
            File.WriteAllText(_filePath, dataString);
        }

        public bool Exists()
        {
            return File.ReadAllText(_filePath) != "";
        }

        public void Remove()
        {
            File.CreateText(_filePath);
        }
    }
}