using Data.Dynamic;

namespace Infrastructure.Services.Progress
{
    public interface IProgressService
    {
        void SetProgress(GameData gameData);
        GameData GetProgress();
        void CreateNewProgress();
    }
}