#if YG_SERVICES
using Data.Dynamic;
using Infrastructure.Services.SaveLoadData;
using Newtonsoft.Json;


namespace YG
{
    public class SaveLoadDataYGService : ISaveLoadDataService
    {
        public GameData LoadOrCreateNew()
        {
            YandexMetrica.Send("loaded");
            GameData gameData = null;

            if (!Exists())
            {
                CreatingNewData();
            }
            else
            {
                var dataJson = YandexGame.savesData.json;
                gameData = JsonConvert.DeserializeObject<GameData>(dataJson);
            }

            return gameData;
        }

        public void Save(GameData gameData)
        {
            YandexMetrica.Send("saved");
            
            gameData.Serialize();

            var dataJson = JsonConvert.SerializeObject(gameData);
            YandexGame.savesData.isFirstSession = false;
            YandexGame.savesData.json = dataJson;
            YandexGame.SaveProgress();
        }

        public bool Exists()
        {
            return YandexGame.SDKEnabled && YandexGame.savesData.isFirstSession == false;
        }

        public void Remove()
        {
            CreatingNewData();
        }

        private static void CreatingNewData()
        {
            var dialogues = new DialoguesData[6];

            for (var i = 0; i < dialogues.Length; i++)
            {
                dialogues[i] = new DialoguesData();
            }

            GameData gameData = new CustomData();
            gameData.dialogues = dialogues;
            gameData.Serialize();
            
            var dataJson = JsonConvert.SerializeObject(gameData);
            YandexGame.savesData.isFirstSession = false;
            YandexGame.savesData.json = dataJson;
            YandexGame.SaveProgress();
        }
    }
}

#endif