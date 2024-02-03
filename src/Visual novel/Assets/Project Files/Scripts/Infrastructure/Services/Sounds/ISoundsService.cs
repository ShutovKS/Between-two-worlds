namespace Infrastructure.Services.Sounds
{
    public interface ISoundsService
    {
        void SetClip(string clipName, bool isLoop = false);
        void Stop();
        void Play();
    }
}
