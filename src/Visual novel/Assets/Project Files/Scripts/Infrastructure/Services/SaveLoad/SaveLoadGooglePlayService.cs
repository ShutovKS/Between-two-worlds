using System;
using System.Threading.Tasks;
using Data.Dynamic;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using Infrastructure.Services.Authenticate;
using Infrastructure.Services.Progress;
using Tools.Extensions;
using UnityEngine;

namespace Infrastructure.Services.SaveLoad
{
    public class SaveLoadGooglePlayService : ISaveLoadService
    {
        public SaveLoadGooglePlayService(IAuthenticateService authenticateService,
            SaveLoadLocalService saveLoadLocalService)
        {
            _authenticateService = authenticateService;
            _saveLoadLocalService = saveLoadLocalService;
        }

        private const string FILE_NAME = "GameData";

        private readonly IAuthenticateService _authenticateService;
        private readonly SaveLoadLocalService _saveLoadLocalService;

        private bool _isSaving;

        public async Task<(GameData, LoadState)> Load()
        {
            var info = await _saveLoadLocalService.Load();

            if (!_authenticateService.IsAuthenticated)
            {
                return info;
            }

            GameData gameData = null;
            var isResult = false;
            OpenWithAutomaticConflictResolution(async (requestStatus, gameMetadata) =>
            {
                gameData = await LoadWhenOpening(requestStatus, gameMetadata);
                isResult = true;
            });

            var waitTime = 5f;
            while (isResult == false && waitTime > 0)
            {
                waitTime -= Time.deltaTime;
                await Task.Yield();
            }

            if (isResult && gameData != null && (info.Item1.LastSaveTime < gameData.LastSaveTime ||
                                                 info.Item2 == LoadState.NoSavedProgress))
            {
                info.Item1 = gameData;
                info.Item2 = LoadState.Successfully;
            }

            return info;
        }

        public void Save(GameData gameData)
        {
            gameData.LastSaveTime = DateTime.UtcNow;

            if (_authenticateService.IsAuthenticated)
            {
                OpenWithAutomaticConflictResolution((status, meteData) => SaveWhenOpening(status, meteData, gameData));
            }

            _saveLoadLocalService.Save(gameData);
        }

        private void OpenWithAutomaticConflictResolution(Action<SavedGameRequestStatus, ISavedGameMetadata> callback)
        {
            PlayGamesPlatform.Instance.SavedGame.OpenWithAutomaticConflictResolution(FILE_NAME,
                DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLongestPlaytime, callback);
        }

        private void SaveWhenOpening(SavedGameRequestStatus status, ISavedGameMetadata metadata, GameData gameData)
        {
            if (status != SavedGameRequestStatus.Success)
            {
                return;
            }

            var playerData = gameData.SerializeToByteArray();

            var updateForMetadata = new SavedGameMetadataUpdate.Builder()
                .WithUpdatedDescription($"Game was update: {DateTime.UtcNow}").Build();

            PlayGamesPlatform.Instance.SavedGame.CommitUpdate(metadata, updateForMetadata, playerData, SavedCallBack);
            return;

            void SavedCallBack(SavedGameRequestStatus status, ISavedGameMetadata metadata)
            {
                Debug.Log(status == SavedGameRequestStatus.Success ? "Game was saved" : "Game was not saved");
            }
        }

        private async Task<GameData> LoadWhenOpening(SavedGameRequestStatus status, ISavedGameMetadata metadata)
        {
            if (status != SavedGameRequestStatus.Success)
            {
                return null;
            }

            GameData gameData = null;
            var isResult = false;

            PlayGamesPlatform.Instance.SavedGame.ReadBinaryData(metadata, OnSavedGameDataRead);

            var waitTime = 5f;
            while (isResult == false && waitTime > 0)
            {
                waitTime -= Time.deltaTime;
                await Task.Yield();
            }

            return gameData;

            void OnSavedGameDataRead(SavedGameRequestStatus requestStatus, byte[] data)
            {
                if (requestStatus != SavedGameRequestStatus.Success)
                {
                    isResult = true;
                }

                gameData = data.Deserialize<GameData>();
                isResult = true;
            }
        }
    }
}