using System.Threading.Tasks;
using Data.Dynamic;

namespace Infrastructure.Services.Progress
{
    public interface IProgressService
    {
        void SetProgress(GameData gameData);
        Task<GameData> GetProgress();
        void CreateNewProgress();
    }
}