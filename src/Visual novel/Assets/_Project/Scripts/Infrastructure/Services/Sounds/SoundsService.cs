using UnityEngine;

namespace Infrastructure.Services.Sounds
{
    public class SoundsService : ISoundsService
    {
        public SoundsService()
        {
            var instance = new GameObject();
            _audioSource = instance.AddComponent<AudioSource>();
            Object.DontDestroyOnLoad(instance);
        }

        private readonly AudioSource _audioSource;

        public void SetClip(string clipName, bool isLoop = false)
        {
            _audioSource.clip = GetClip(clipName);
            _audioSource.loop = isLoop;
            Play();
        }

        public void Stop()
        {
            _audioSource.Stop();
        }

        public void Play()
        {
            _audioSource.Play();
        }

        private AudioClip GetClip(string clipName)
        {
            return Resources.Load<AudioClip>("Data/Sounds/" + clipName);
        }
    }
}
