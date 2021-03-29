using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    #region Fields, Propertes
    [SerializeField]
    private Settings _settings = null;

    [SerializeField]
    private Toggle _musicToggle = null;
    public bool IsMusicOn { get { return _musicToggle.isOn; } }

    [SerializeField]
    private Toggle _sfxToggle = null;
    public bool IsSfxOn { get { return _sfxToggle.isOn; } }

    [SerializeField]
    private CanvasGroup _canvas = null;

    [SerializeField]
    private AudioClip _music = null;
    #endregion Fields, Properties (end)

    #region Methods 
    public void InitializeSettings(SaveData saveData)
    {
        _settings.IsMusicOn = saveData.MusicOn;
        _settings.IsSfxOn = saveData.SoundEffectsOn;
        _musicToggle.SetIsOnWithoutNotify(_settings.IsMusicOn);
        _sfxToggle.SetIsOnWithoutNotify(_settings.IsSfxOn);
    }
    
    public void Setup()
    {
        _musicToggle.SetIsOnWithoutNotify(_settings.IsMusicOn);
        _sfxToggle.SetIsOnWithoutNotify(_settings.IsSfxOn);
        SetCanvasState(true);
        AudioManager.SetMusic(_music, true);
    }
    
    #region Unity Called Methods
    public void CloseClicked()
    {
        SetCanvasState(false);
        LevelManager.SaveSettingChanges();
    }

    public void ToggleMusicClicked()
    {
        _settings.IsMusicOn = !_settings.IsMusicOn;
        if (_settings.IsMusicOn)
            AudioManager.PlayMusic();
        else
            AudioManager.StopMusic();
    }

    public void ToggleSoundEffectsClicked()
    {
        _settings.IsSfxOn = !_settings.IsSfxOn;
    }
    #endregion Unity Called Methods (end)
    
    private void SetCanvasState(bool enabled)
    {
        _canvas.alpha = enabled ? 1 : 0;
        _canvas.blocksRaycasts = enabled;
        _canvas.interactable = enabled;
    }
    #endregion Methods(end)
}
