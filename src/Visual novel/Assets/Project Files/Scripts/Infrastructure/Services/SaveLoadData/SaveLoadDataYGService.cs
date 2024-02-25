#if YG_SERVICES
using Data.Dynamic;
using Infrastructure.Services.SaveLoadData;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace YG
{
    public class SaveLoadDataYGService : ISaveLoadDataService
    {
        public GameData LoadOrCreateNew()
        {
            YandexMetrica.Send("loaded");
            GameData gameData;

            if (Exists() == false)
            {
                gameData = CreatingNewData();
            }
            else
            {
                var dataJson = YandexGame.savesData.json;

                gameData = JsonUtility.FromJson<GameData>(dataJson);
            }

            return gameData;
        }

        public void Save(GameData gameData)
        {
            YandexMetrica.Send("saved");

            var dataJson = JsonUtility.ToJson(gameData, false);

            YandexGame.savesData.isFirstSession = false;
            YandexGame.savesData.json = dataJson;
            YandexGame.SaveProgress();
        }

        public bool Exists()
        {
            return YandexGame.SDKEnabled &&
                   YandexGame.savesData.isFirstSession == false &&
                   string.IsNullOrEmpty(YandexGame.savesData.json) == false;
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