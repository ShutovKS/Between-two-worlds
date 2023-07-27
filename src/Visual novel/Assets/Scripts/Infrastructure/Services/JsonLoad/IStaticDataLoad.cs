namespace Infrastructure.Services.JsonLoad
{
    public interface IStaticDataLoad
    {
        void Load();
        Part GetPart(string id);
    }
}