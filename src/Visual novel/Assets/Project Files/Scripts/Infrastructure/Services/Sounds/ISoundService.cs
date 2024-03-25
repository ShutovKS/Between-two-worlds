namespace Infrastructure.Services.Sounds
{
    public interface ISoundService
    {
        void SetClip(string clipName, bool isLoop = false);
        void Stop();
        void Play();
    }
}