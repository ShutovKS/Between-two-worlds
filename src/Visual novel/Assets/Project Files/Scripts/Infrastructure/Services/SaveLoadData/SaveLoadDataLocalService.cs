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
        private GameData _gameData;

        public GameData GetData()
        {
            return _gameData ??= LoadOrCreateNew();
        }

        public GameData LoadOrCreateNew()
        {
            if (File.Exists(_filePath))
            {
                var json = File.ReadAllText(_filePath);

                _gameData = JsonUtility.FromJson<GameData>(json);
            }
            else
            {
                _gameData = CreatingNewData();
            }

            return _gameData;
        }

        public void Save(GameData gameData)
        {
            gameData.LastSaveTime = System.DateTime.Now;
            
            var dataString = JsonUtility.ToJson(gameData, false);

            File.WriteAllText(_filePath, dataString);
            
            _gameData = gameData;
        }

        public bool Exists()
        {
            return _gameData != null;
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