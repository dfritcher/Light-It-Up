using UnityEngine;

/// <summary>
/// Class responsible for playing all sound in the game. 
/// We use a single class so there is only one reference to the audio Settings class.
/// </summary>
public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private Settings _settings = null;
    private Settings Settings { get { return _settings; } }

    [SerializeField]
    private AudioSource _musicSource = null;
    private AudioSource MusicSource { get { return _musicSource; } }

    [SerializeField]
    private AudioSource _sfxSource = null;
    private AudioSource SFXSource { get { return _sfxSource; } }

    [SerializeField]
    private static AudioManager _instance = null;
    private static AudioManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(_instance);
    }
    public static void PlayOneShot(AudioClip clip, float pitchSetting = 1f)
    {
        if (Instance.Settings.IsSfxOn)
        {
            Instance.SFXSource.pitch = pitchSetting;
            Instance.SFXSource.PlayOneShot(clip);
        }
    }

    public static void SetMusic(AudioClip music, bool startPlaying = false)
    {
        if (Instance.MusicSource.clip.name == music.name)
            return;
        Instance.MusicSource.clip = music;
        if (startPlaying)
            PlayMusic();
    }

    public static void PlayMusic()
    {
        if (Instance.Settings.IsMusicOn)
        {
            Instance.MusicSource.Play();
        }
    }

    public static void StopMusic()
    {
        Instance.MusicSource.Stop();
    }

    public static void StartMusic()
    {
        Instance.MusicSource.Play();
    }
}