using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{

    #region Fields, Properties
    [SerializeField]
    private int _levelNumber = 0;
    public int LevelNumber { get { return _levelNumber; } }

    [SerializeField]
    private LevelManager _levelManager = null;

    [SerializeField]
    private Image _levelImage = null;

    [SerializeField]
    private Sprite _lockedDisplay = null;

    [SerializeField]
    private Sprite _levelIconDisplay = null;

    [SerializeField]
    private TextMeshProUGUI _levelNumberDisplay = null;

    private bool _isLocked = false;
    #endregion Fields, Properites (end)

    #region Methods
    public void Setup(LevelManager manager, int highestLevelUnlocked)
    {
        _levelManager = manager;
        _isLocked = highestLevelUnlocked < _levelNumber;
        _levelImage.sprite = _isLocked ? _lockedDisplay : _levelIconDisplay;
        _levelNumberDisplay.gameObject.SetActive(!_isLocked);
        _levelNumberDisplay.text = _levelNumber.ToString();
    }
    public void OnLevelClicked()
    {
        if (_isLocked)
            return;
        _levelManager.LevelSelect_LevelClicked(_levelNumber);
    }
    #endregion Methods (end)
}
