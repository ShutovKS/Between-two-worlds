using System;
using Data.Dynamic;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using UnityEngine;

namespace Infrastructure.Services.SaveLoadData
{
    public class SaveLoadDataGooglePlayService : ISaveLoadDataService
    {
        private const string FILE_NAME = "GameData";

        private static bool IsAuthenticated => PlayGamesPlatform.Instance.IsAuthenticated();
        private static ISavedGameClient SavedGameClient => PlayGamesPlatform.Instance.SavedGame;

        private readonly SaveLoadDataLocalService _saveLoadDataLocalService = new();

        private GameData _gameData;

        public GameData GetData()
        {
            return _gameData ??= LoadOrCreateNew();
        }

        public GameData LoadOrCreateNew()
        {
            _gameData = _saveLoadDataLocalService.LoadOrCreateNew();

            if (IsAuthenticated)
            {
                _gameData = CheckOldData(_gameData, LoadFromCloud());
            }

            return _gameData ??= new GameData();
        }

        public void Save(GameData gameData)
        {
            if (IsAuthenticated)
            {
                SaveToCloud(gameData);
            }

            _gameData = gameData;
            _saveLoadDataLocalService.Save(gameData);
        }

        public bool Exists()
        {
            return _gameData != null;
        }

        public void Remove()
        {
            if (IsAuthenticated)
            {
                OpenSavedGame(RemoveDataFromCloud);
            }

            _gameData = null;
            _saveLoadDataLocalService.Remove();
        }

        private static GameData CheckOldData(GameData dataFromLocal, GameData dataFromCloud)
        {
            return (dataFromCloud == null, dataFromLocal == null) switch
            {
                (true, true) => null,
                (false, true) => dataFromCloud,
                (true, false) => dataFromLocal,
                (false, false) => dataFromLocal!.LastSaveTime > dataFromCloud!.LastSaveTime
                    ? dataFromLocal
                    : dataFromCloud
            };
        }

        private static GameData LoadFromCloud()
        {
            var isComplete = false;
            GameData gameData = null;

            OpenSavedGame((status, game) =>
            {
                gameData = LoadGame(status, game);
                isComplete = true;
            });

            while (!isComplete)
            {
            }

            return gameData;

            GameData LoadGame(SavedGameRequestStatus status, ISavedGameMetadata game)
            {
                if (status != SavedGameRequestStatus.Success)
                {
                    return null;
                }

                GameData gameData = null;

                SavedGameClient.ReadBinaryData(game, (status2, data) =>
                {
                    if (status2 == SavedGameRequestStatus.Success)
                    {
                        var dataString = System.Text.Encoding.UTF8.GetString(data);
                        gameData = JsonUtility.FromJson<GameData>(dataString);
                    }

                    Debug.LogError("Error loading game: " + status2);
                });

                return gameData;
            }
        }

        private static void SaveToCloud(GameData gameData)
        {
            OpenSavedGame(SaveGame);
            return;

            void SaveGame(SavedGameRequestStatus status, ISavedGameMetadata game)
            {
                if (status != SavedGameRequestStatus.Success)
                {
                    return;
                }

                var dataString = JsonUtility.ToJson(gameData, false);
                var data = System.Text.Encoding.UTF8.GetBytes(dataString);

                var update = new SavedGameMetadataUpdate.Builder()
                    .WithUpdatedDescription("Saved game at " + DateTime.Now).Build();

                SavedGameClient.CommitUpdate(game, update, data, (status2, metadata) =>
                {
                    if (status2 == SavedGameRequestStatus.Success)
                    {
                        Debug.Log("Game saved successfully");
                    }
                    else
                    {
                        Debug.LogError("Error saving game: " + status2);
                    }
                });
            }
        }

        private static void RemoveDataFromCloud(SavedGameRequestStatus savedGameRequestStatus,
            ISavedGameMetadata metadata)
        {
            if (savedGameRequestStatus == SavedGameRequestStatus.Success)
            {
                SavedGameClient.Delete(metadata);
            }
        }


        private static void OpenSavedGame(Action<SavedGameRequestStatus, ISavedGameMetadata> callback)
        {
            var savedGameClient = PlayGamesPlatform.Instance.SavedGame;
            savedGameClient.OpenWithAutomaticConflictResolution(FILE_NAME, DataSource.ReadNetworkOnly,
                ConflictResolutionStrategy.UseMostRecentlySaved, callback);
        }
    }
}