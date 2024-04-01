using System.Threading.Tasks;
using Data.Dynamic;
using Infrastructure.Services.SaveLoad;

namespace Infrastructure.Services.Progress
{
    public class ProgressService : IProgressService
    {
        public ProgressService(ISaveLoadService saveLoadService)
        {
            _saveLoadService = saveLoadService;
        }

        private readonly ISaveLoadService _saveLoadService;

        private GameData _gameData;

        public void SetProgress(GameData gameData)
        {
            _gameData = gameData;
            _saveLoadService.Save(gameData);
        }

        public async Task<GameData> GetProgress()
        {
            if (_gameData != null)
            {
                return _gameData;
            }

            (GameData gameData, LoadState loadState) result = await _saveLoadService.Load();
            _gameData = result.gameData;

            if (result.loadState == LoadState.NoSavedProgress)
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