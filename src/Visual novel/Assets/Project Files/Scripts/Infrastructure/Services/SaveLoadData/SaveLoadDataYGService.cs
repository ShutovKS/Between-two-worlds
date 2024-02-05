using Data.Dynamic;
using YG;

namespace Infrastructure.Services.SaveLoadData
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
                YandexGame.savesData.Deserialize();
            }

            return YandexGame.savesData;
        }

        public void Save(GameData gameData)
        {
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
}

namespace YG
{
    public partial class SavesYG : GameData
    {
    }
}