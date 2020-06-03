using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _victoryDisplay = null;
    [SerializeField]
    private TextMeshProUGUI _levelDisplay = null;
    [SerializeField]
    private List<Level> _levels = null;
    [SerializeField]
    private Transform _levelsParent = null;
    [SerializeField]
    private BatteryOptionsManager _batteryOptionsManager = null;
    public BatteryOptionsManager BatteryOptionsManager { get { return _batteryOptionsManager; } }

    [SerializeField]
    private InhibitorOptionsManager _inhibitorOptionsManager = null;
    public InhibitorOptionsManager InhibitorOptionsManager { get { return _inhibitorOptionsManager; } }

    [SerializeField]
    private PassThroughOptionsManager _passThroughOptionsManager = null;
    public PassThroughOptionsManager PassThroughOptionsManager { get {return _passThroughOptionsManager; } }

    private static LevelManager _instance = null;
    public static LevelManager Instance { get { return _instance; } }
    private void Start()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(_instance.gameObject);

        _levels = _levelsParent.GetComponentsInChildren<Level>(true).ToList();
        for(int i = 0; i < _levels.Count; ++i)
        {
            _levels[i].Setup(this);
            if(i == 0)
                _levels[i].SetActiveState(true);
            else
                _levels[i].SetActiveState(false);
        }
    }
    public void NextLevelClicked()
    {
        InitializeNextLevel();
        ResetVictoryState();
    }

    public void ResetLevel()
    {
        _levels.Find(l => l.IsActive).RestartLevel();
        ResetVictoryState();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void InitializeNextLevel()
    {
        var index = _levels.IndexOf(_levels.Find(l => l.IsActive));
        if (index >= _levels.Count())
            return;
        else
        {
            _levels[index].SetActiveState(false);
            _levels[index + 1].SetActiveState(true);
            _levels[index + 1].Setup(this);
        }        
    }

    public void ResetVictoryState()
    {
        _victoryDisplay.text = string.Empty;
    }

    public void SetVictoryState(bool hasWon)
    {
        if(hasWon)
            _victoryDisplay.text = "YOU WIN!";
        else 
        {
            _victoryDisplay.text = "SORRY YOU LOSE! \n TRY AGAIN!";
        }
    }

    public void SetLevelDisplay(int levelNumber)
    {
        _levelDisplay.text = $"Level {levelNumber}";
    }
}
