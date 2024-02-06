using Data.Dynamic;
using Infrastructure.Services.SaveLoadData;

namespace YG
{
    public class SaveLoadDataYGService : ISaveLoadDataService
    {
        public GameData LoadOrCreateNew()
        {
            if (!Exists())
            {
                Reset();
            }
            else
            {
                YandexMetrica.Send("loaded");
                YandexGame.savesData.Deserialize();
            }

            return YandexGame.savesData;
        }

        public void Save(GameData gameData)
        {
            YandexMetrica.Send("saved");
            gameData.Serialize();

            YandexGame.savesData.isFirstSession = false;
            YandexGame.savesData.dialogues = gameData.dialogues;
            YandexGame.SaveProgress();
        }

        public bool Exists()
        {
            return YandexGame.SDKEnabled && YandexGame.savesData.isFirstSession == false;
        }

        public void Remove()
        {
            Reset();
        }

        private void Reset()
        {
            var dialogues = new DialoguesData[6];

            for (var i = 0; i < dialogues.Length; i++)
            {
                dialogues[i] = new DialoguesData();
            }

            YandexGame.savesData.dialogues = dialogues;
            Save(YandexGame.savesData);
        }
    }

    public partial class SavesYG : GameData
    {
    }
}