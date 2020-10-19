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
    private AudioSource _audioSource = null;
    private AudioSource AudioSource { get { return _audioSource; } }
    
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
    public static void PlayOneShot(AudioClip clip)
    {
        if (Instance.Settings.IsSfxOn)
        {
            Instance.AudioSource.PlayOneShot(clip);
        }
    }

    public static void SetMusic(AudioClip music, bool startPlaying = false)
    {
        Instance.AudioSource.clip = music;
        if (startPlaying)
            PlayMusic();
    }

    public static void PlayMusic()
    {
        if (Instance.Settings.IsMusicOn)
        {
            Instance.AudioSource.Play();
        }
    }

    public static void StopMusic()
    {
        Instance.AudioSource.Stop();
    }

    public static void StartMusic()
    {
        Instance.AudioSource.Play();
    }
}