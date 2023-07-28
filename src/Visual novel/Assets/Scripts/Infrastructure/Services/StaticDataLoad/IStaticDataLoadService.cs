using Data.Dialogue;

namespace Infrastructure.Services.StaticDataLoad
{
    public interface IStaticDataLoadService
    {
        void Load();
        Part GetPart(string id);
    }
}