#region

using System.IO;
using System.Threading.Tasks;
using Data.Dynamic;
using UnityEngine;

#endregion

namespace Infrastructure.Services.SaveLoad
{
    public class SaveLoadLocalService : ISaveLoadService
    {
        public SaveLoadLocalService()
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

        public Task<(GameData, LoadState)> Load()
        {
            LoadState loadState;

            if (File.Exists(_filePath))
            {
                var json = File.ReadAllText(_filePath);
                var gameData = JsonUtility.FromJson<GameData>(json);

                loadState = LoadState.Successfully;

                return Task.FromResult((gameData, loadState));
            }

            loadState = LoadState.NoSavedProgress;

            return Task.FromResult(((GameData)null, loadState));
        }

        public void Save(GameData gameData)
        {
            var dataString = JsonUtility.ToJson(gameData, false);

            File.WriteAllText(_filePath, dataString);
        }
    }
}