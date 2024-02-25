#region

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
            GameData gameData;
            
            if (Exists() == false)
            {
                gameData = CreatingNewData();
            }
            else
            {
                var json = File.ReadAllText(_filePath);

                gameData = JsonUtility.FromJson<GameData>(json);
            }

            return gameData;
        }

        public void Save(GameData gameData)
        {
            var dataString = JsonUtility.ToJson(gameData, false);

            File.WriteAllText(_filePath, dataString);
        }

        public bool Exists()
        {
            return File.Exists(_filePath);
        }

        public void Remove()
        {
            CreatingNewData();
        }
        
        private GameData CreatingNewData()
        {
            var gameData = new GameData();
            
            Save(gameData);

            return gameData;
        }
    }
}