#region

using System;
using System.IO;
using Data.Dynamic;
using UnityEngine;

#endregion

namespace Infrastructure.Services.SaveLoadData
{
    public class SaveLoadDataLocalService : ISaveLoadDataService
    {
        public SaveLoadDataLocalService()
        {
            var folderPath = Path.Combine(Application.persistentDataPath, FOLDER_NAME);

            _filePath = Path.Combine(folderPath, FILE_NAME);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }

        private const string FILE_NAME = "GameData.txt";
        private const string FOLDER_NAME = "DataFolder";
        private readonly string _filePath;

        public GameData LoadOrCreateNew()
        {
            if (Exists())
            {
                var json = File.ReadAllText(_filePath);

                var gameData = JsonUtility.FromJson<CustomData>(json);

                gameData.Deserialize();

                return gameData;
            }

            return new CustomData();
        }

        public void Save(GameData gameData)
        {
            gameData.Serialize();

            var dataString = JsonUtility.ToJson(gameData, false);

            File.WriteAllText(_filePath, dataString);
        }

        public bool Exists()
        {
            return File.Exists(_filePath);
        }

        public void Remove()
        {
            if (File.Exists(_filePath))
            {
                File.Delete(_filePath);
            }
        }
    }

    [Serializable]
    public class CustomData : GameData
    {
    }
}