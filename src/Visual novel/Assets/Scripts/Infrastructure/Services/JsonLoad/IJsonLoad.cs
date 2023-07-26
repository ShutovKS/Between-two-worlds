namespace Infrastructure.Services.JsonLoad
{
    public interface IJsonLoad
    {
        void Load();
        Part GetPart(string id);
    }
}