#if YG_SERVICES

using Data.Dynamic;
using UnityEditor;
using UnityEngine;
using YG;


namespace Infrastructure.Services.SaveLoadData
{
    public class SaveLoadDataYGService : ISaveLoadDataService
    {
        private GameData _gameData;

        public GameData GetData()
        {
            return _gameData ??= LoadOrCreateNew();
        }

        public GameData LoadOrCreateNew()
        {
            YandexMetrica.Send("loaded");

            if (Exists() == false)
            {
                _gameData = CreatingNewData();
            }
            else
            {
                var dataJson = YandexGame.savesData.json;

                _gameData = JsonUtility.FromJson<GameData>(dataJson);
            }

            return _gameData;
        }

        public void Save(GameData gameData)
        {
            YandexMetrica.Send("saved");

            var dataJson = JsonUtility.ToJson(gameData, false);

            YandexGame.savesData.isFirstSession = false;
            YandexGame.savesData.json = dataJson;
            YandexGame.SaveProgress();

            _gameData = gameData;
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