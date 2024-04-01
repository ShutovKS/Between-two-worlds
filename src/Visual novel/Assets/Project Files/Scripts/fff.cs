    // using System;
    // using GooglePlayGames;
    // using GooglePlayGames.BasicApi;
    // using GooglePlayGames.BasicApi.SavedGame;
    // using Infrastructure.Services.Progress;
    // using ProjectFiles.OOPScripts.Data;
    // using ProjectFiles.OOPScripts.Services.DynamicData.Progress;
    // using ProjectFiles.OOPScripts.Services.DynamicData.SaveLoad;
    // using ProjectFiles.OOPScripts.Services.TimeControlService.DaysAndWeeksChange;
    // using UnityEngine;
    //
    // namespace ProjectFiles.OOPScripts.Services.GooglePlayServices
    // {
    //     public class GooglePlayServices : IGooglePlayServices
    //     {
    //         public event Action NeedCreateProgress;
    //         public event Action ProgressLoaded;
    //         public event Action LoadLocalProgress;
    //         
    //         private readonly IProgressService _progressService;
    //         private readonly ISaveLoadService _saveLoadService;
    //         private readonly IDaysAndWeeksChangeControlService _daysAndWeeksChangeControlService;
    //
    //         const bool allowCreateNew = true;
    //         const bool allowDelete = true;
    //         const uint maxNumToDisplay = 5;
    //         
    //         private bool _isSaving;
    //
    //         private GooglePlayServices(IProgressService progressService,
    //             ISaveLoadService saveLoadService,
    //             IDaysAndWeeksChangeControlService daysAndWeeksChangeControlService)
    //         {
    //             _progressService = progressService;
    //             _saveLoadService = saveLoadService;
    //             _daysAndWeeksChangeControlService = daysAndWeeksChangeControlService;
    //         }
    //
    //         public bool IsAuthenticated()
    //         {
    //             return Social.localUser.authenticated;
    //         }
    //
    //         public void LoginGooglePlayGames(Action callback)
    //         {
    //             PlayGamesPlatform.Instance.Authenticate((success) =>
    //             {
    //                 if (success == SignInStatus.Success)
    //                 {
    //                     Social.localUser.Authenticate((bool successAuth) =>
    //                     {
    //                         if (successAuth)
    //                         {
    //                             Debug.Log("Login with Google Play games successful.");
    //                             callback?.Invoke();
    //                         }
    //                         else
    //                         {
    //                             Debug.Log("Login social user unsuccessful");
    //                             
    //                             LoadLocalProgress?.Invoke();
    //                         }
    //                     });
    //                 }
    //                 else
    //                 {
    //                     Debug.Log("Login Unsuccessful");
    //                     
    //                     LoadLocalProgress?.Invoke();
    //                 }
    //             });
    //         }
    //
    //         public void SaveProgress()
    //         {
    //             _isSaving = true;
    //             
    //            OpenSave();
    //         }
    //
    //         public void LoadProgress()
    //         {
    //             _isSaving = false;
    //             
    //             OpenSave();
    //         }
    //
    //         private void OpenSave()
    //         {
    //             if (Social.localUser.authenticated)
    //             {
    //                 PlayGamesPlatform.Instance.SavedGame.OpenWithAutomaticConflictResolution("FileData", DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLongestPlaytime, OnSavedGameOpened);
    //             }
    //             else
    //             {
    //                 Debug.Log("User not authenticated");
    //             }
    //         }
    //         
    //         private void OnSavedGameOpened(SavedGameRequestStatus status, ISavedGameMetadata metadata)
    //         {
    //             if (status == SavedGameRequestStatus.Success)
    //             {
    //                 if (_isSaving)
    //                 {
    //                     var playerData = _progressService.PlayerProgress.ToByteArray();
    //                     
    //                     var updateForMetadata = new SavedGameMetadataUpdate.Builder().WithUpdatedDescription($"Game was update: {_daysAndWeeksChangeControlService.UtcNow}").Build();
    //                     
    //                     PlayGamesPlatform.Instance.SavedGame.CommitUpdate(metadata, updateForMetadata, playerData, SavedCallBack);
    //                 }
    //                 else
    //                 {
    //                     PlayGamesPlatform.Instance.SavedGame.ReadBinaryData(metadata, OnSavedGameDataRead);
    //                 }
    //             }
    //         }
    //
    //         private void OnSavedGameDataRead(SavedGameRequestStatus status, byte[] data)
    //         {
    //             if (status == SavedGameRequestStatus.Success)
    //             {
    //                 if (data.TryParseByteArrayToPlayerProgress())
    //                 {
    //                     var playerProgress = data.ByteArrayToPlayerProgress();
    //                     
    //                     _progressService.SetProgress(playerProgress);
    //                     
    //                     ProgressLoaded?.Invoke();
    //                 }
    //                 else
    //                 {
    //                     Debug.Log("No progress");
    //                     
    //                     NeedCreateProgress?.Invoke();
    //                 }
    //             }
    //         }
    //
    //         private void SavedCallBack(SavedGameRequestStatus status, ISavedGameMetadata metadata)
    //         {
    //             Debug.Log(status == SavedGameRequestStatus.Success ? "Game was saved" : "Game was not saved");
    //         }
    //
    //         public void ShowSelectUI()
    //         {
    //             var savedGameClient = PlayGamesPlatform.Instance.SavedGame;
    //             savedGameClient.ShowSelectSavedGameUI("Select saved game",
    //                 maxNumToDisplay,
    //                 allowCreateNew,
    //                 allowDelete,
    //                 OnSavedGameSelected);
    //         }
    //
    //
    //         private void OnSavedGameSelected (SelectUIStatus status, ISavedGameMetadata metadata) 
    //         {
    //             if (status == SelectUIStatus.SavedGameSelected) 
    //             {
    //                 PlayGamesPlatform.Instance.SavedGame.ReadBinaryData(metadata, OnSavedGameDataRead);
    //             }
    //         }
    //     }
    // }