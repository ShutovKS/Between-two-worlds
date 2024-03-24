using Data.Dynamic;
using Infrastructure.Services.SaveLoadData;

namespace Infrastructure.Services.Progress
{
    public interface IProgressService
    {
        void SetProgress(GameData gameData);
        GameData GetProgress();
        void CreateNewProgress();
    }

    public class ProgressService : IProgressService
    {
        public ProgressService(ISaveLoadDataService saveLoadDataService)
        {
            _saveLoadDataService = saveLoadDataService;
        }

        private readonly ISaveLoadDataService _saveLoadDataService;

        private GameData _gameData;

        public void SetProgress(GameData gameData)
        {
            _gameData = gameData;
            _saveLoadDataService.Save(gameData);
        }

        public GameData GetProgress()
        {
            if (_gameData != null)
            {
                return _gameData;
            }

            _gameData = _saveLoadDataService.Load(out var loadState);

            if (loadState == LoadState.NoSavedProgress)
            {
                CreateNewProgress();
            }

            return _gameData;
        }

        public void CreateNewProgress()
        {
            _gameData = new GameData();
        }
    }
}