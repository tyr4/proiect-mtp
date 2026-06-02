using UnityEngine;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private SoundLibrary soundLibrary;
    
    public static AudioManager Instance;
    public static SoundLibrary Sounds => Instance.soundLibrary;
    
    public AudioSource MusicSource => musicSource;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        AudioEvents.OnSFXRequested += PlaySFX;
        AudioEvents.OnMusicRequested += PlayMusic;
    }

    private void OnDisable()
    {
        AudioEvents.OnSFXRequested -= PlaySFX;
        AudioEvents.OnMusicRequested -= PlayMusic;
    }

    private void PlaySFX(AudioClip clip)
    {
        sfxSource.pitch = Random.Range(0.9f, 1.1f);
        sfxSource.PlayOneShot(clip);
    }

    private void PlayMusic(AudioClip clip)
    {
        if (musicSource.clip == clip) return;
        musicSource.clip = clip;
        musicSource.Play();
    }
}
