using UnityEngine;

namespace Match3
{
    public sealed class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance {get; private set;}

        [Header("AudioClips")]
        public AudioClip clearSound;
        public AudioClip selectSound;
        public AudioClip swapSound;

        [Header("AudioSources")]
        [SerializeField] private AudioSource _sfxAudioSource;
        [SerializeField] private AudioSource _musicAudioSource;

        void Awake()
        {
            Instance = this;
        }

        public void PlayOneShot(AudioClip audioClip)
        {
            _sfxAudioSource.PlayOneShot(audioClip);
        }
    }
}
