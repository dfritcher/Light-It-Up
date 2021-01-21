using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    #region Fields, Propertes
    [SerializeField]
    private Settings _settings = null;

    [SerializeField]
    private Toggle _musicToggle = null;

    [SerializeField]
    private Toggle _sfxToggle = null;

    [SerializeField]
    private CanvasGroup _canvas = null;

    [SerializeField]
    private AudioClip _music = null;
    #endregion Fields, Properties (end)

    #region Methods 
    public void Setup()
    {
        _musicToggle.isOn = _settings.IsMusicOn;
        _sfxToggle.isOn = _settings.IsSfxOn;
        SetCanvasState(true);
        //AudioManager.SetMusic(_music);
        //AudioManager.PlayMusic();
    }

    public void CloseClicked()
    {
        SetCanvasState(false);
    }

    public void ToggleMusicClicked()
    {
        _settings.IsMusicOn = !_settings.IsMusicOn;
    }

    public void ToggleSoundEffectsClicked()
    {
        _settings.IsSfxOn = !_settings.IsSfxOn;
    }

    private void SetCanvasState(bool enabled)
    {
        _canvas.alpha = enabled ? 1 : 0;
        _canvas.blocksRaycasts = enabled;
        _canvas.interactable = enabled;
    }
    #endregion Methods(end)
}
