#if YG_SERVICES

using Data.Dynamic;
using UnityEditor;
using UnityEngine;
using YG;

namespace Infrastructure.Services.SaveLoad
{
    public class SaveLoadDataYGService : ISaveLoadService
    {
        public GameData Load(out LoadState loadState)
        {
            YandexGame.LoadProgress();
            
            if (Exists())
            {
                loadState = LoadState.Successfully;

                var dataJson = YandexGame.savesData.json;

                var gameData = JsonUtility.FromJson<GameData>(dataJson);

                return gameData;
            }

            loadState = LoadState.NoSavedProgress;

            return null;
        }

        public void Save(GameData gameData)
        {
            var dataJson = JsonUtility.ToJson(gameData, false);

            YandexGame.savesData.isFirstSession = false;
            YandexGame.savesData.json = dataJson;
            YandexGame.SaveProgress();
        }

        private static bool Exists()
        {
            return string.IsNullOrEmpty(YandexGame.savesData.json) == false;
        }

#if UNITY_EDITOR
        [MenuItem("Tools/Data save-load/Reset save for Yandex Game", false, 1000)]
        private static void ResetSave()
        {
            if (Application.isPlaying)
            {
                YandexGame.ResetSaveProgress();
                Debug.Log("Save reset");
            }
            else
            {
                Debug.LogError("You can reset save only in play mode");
            }
        }
#endif
    }
}

#endif