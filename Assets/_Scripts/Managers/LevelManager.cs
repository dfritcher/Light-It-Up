using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using System;

public class LevelManager : MonoBehaviour
{
    #region Fields, Properties
    [Header("Scene References")]
    [SerializeField]
    private GameObject _victoryDisplay = null;
    [SerializeField]
    private TextMeshProUGUI _victoryMessage = null;

    [SerializeField]
    private GameObject _victoryParent = null;
    [SerializeField]
    private TextMeshProUGUI _defeatMessage = null;
    [SerializeField]
    private GameObject _defeatParent = null;
    [SerializeField]
    private TextMeshProUGUI _levelDisplay = null;
    [SerializeField]
    private List<Level> _levels = null;
    [SerializeField]
    private Transform _levelsParent = null;
    [SerializeField]
    private List<LevelSelect> _selectableLevels = null;

    [SerializeField]
    private Animator _levelTransition = null;
    [SerializeField]
    private GameObject _transitionObject = null;
    [Header("Screens")]
    [SerializeField]
    private CanvasGroup _mainCanvas = null;
    [SerializeField]
    private CanvasGroup _playScreen = null;
    [SerializeField]
    private CanvasGroup _levelOverlay = null;
    [SerializeField]
    private CanvasGroup _levelSelect = null; 

    [SerializeField]
    private Camera _mainCamera;
    public Camera MainCamera { get { return _mainCamera; } }

    [Header("Managers"), Space(8)]
    [SerializeField]
    private BatteryOptionsManager _batteryOptionsManager = null;
    public BatteryOptionsManager BatteryOptionsManager { get { return _batteryOptionsManager; } }

    [SerializeField]
    private InhibitorOptionsManager _inhibitorOptionsManager = null;
    public InhibitorOptionsManager InhibitorOptionsManager { get { return _inhibitorOptionsManager; } }

    [SerializeField]
    private PassThroughOptionsManager _passThroughOptionsManager = null;
    public PassThroughOptionsManager PassThroughOptionsManager { get {return _passThroughOptionsManager; } }

    [SerializeField]
    private BrokenBulbAnimationManager _brokenBulbAnimManager = null;

    [SerializeField]
    private AudioManager _audioManager = null;

    [SerializeField]
    private SaveDataManager _saveDataManager = null;

    [SerializeField]
    private SettingsManager _settingsManager = null;

    [Header("Action Menu"), Space(8)]
    [SerializeField]
    private GameObject _actionsMenu = null;
    [SerializeField, FormerlySerializedAs("_actionMenu")]
    private RectTransform _actionMenuTransform = null;
    [SerializeField]
    private Button _previousLevelButton = null;
    [SerializeField]
    private Button _nextLevelButton = null;

    private float _originalWidth = 0f;
    private float _originalHeight = 0f;

    [SerializeField]
    private float _actionMenuAnimateTime = 0f;

    [SerializeField]
    private List<TextMeshProUGUI> _textItems = null;

    [Header("Tutorial References"), Space(8)]
    [SerializeField]
    private Button _tutorialButton = null;

    [Header("Game Info"), Space(8)]
    [SerializeField]
    private GameSaveInfo _gameInfo = null;

    [Header("Level Select"), Space(8)]
    [SerializeField]
    private Button _previousLevelPageButton = null;
    [SerializeField]
    private Button _nextLevelPageButton = null;
    [SerializeField]
    private List<GameObject> _levelPages = new List<GameObject>();
    

    [Header("Debug Options"), Space(8)]
    [SerializeField]
    private bool _skipTransitions = false;
    [SerializeField]
    private bool _unlockAllLevels = false;


    private static LevelManager _instance = null;
    public static LevelManager Instance { get { return _instance; } }

    private Level _currentLevel = null;
    private Level CurrentLevel {  get { return _currentLevel ?? (_currentLevel = _levels[0]); } }
    #endregion Fields, Properties (end)

    #region Methods
    #region Unity Engine Hooks
    private void Awake()
    {
        LoadGameInfo();

        if (_instance == null)
            _instance = this;
        else
            Destroy(_instance.gameObject);

        _victoryDisplay.SetActive(false);
        _originalWidth = _actionMenuTransform.rect.width;
        _originalHeight = _actionMenuTransform.rect.height;
        _actionMenuTransform.sizeDelta = new Vector2(0f, 0f);

        SetActionMenuInactive();
        SetMainCanvasState(true);
        SetOverlayState(false);
        SetPlayScreenState(true);
        SetLevelSelectState(false);
    }

    private void Start()
    {
        InitializeAllLevels();        
    }
    #endregion Unity Engine Hooks (en)
    
    public void PlayClicked()
    {        
        TriggerLevelAnimation(PlayClickedCallback);        
    }

    private void PlayClickedCallback()
    {
        SetLevelSelectState(true);
        SetOverlayState(false);
        SetPlayScreenState(false);
        InitializeLevelSelectScreen();
        SetActionMenuButtonsState();
        SetPageLevelButtonState(0);
    }

    #region Audio
    internal void SetLevelMusic(AudioClip levelMusic)
    {
        AudioManager.SetMusic(levelMusic, true);
    }
    #endregion Audio (end)

    #region Victory Defeat State
    public void OnBrokenBulbAnimationEnd()
    {
        SetOverlayState(true);
        _currentLevel.gameObject.SetActive(true);
    }

    public void ResetVictoryState()
    {
        _victoryDisplay.SetActive(false);
        _victoryMessage.text = string.Empty;
        _defeatMessage.text = string.Empty;
    }

    public void SetVictoryState(bool hasWon, Level level)
    {
        _currentLevel = level;
        _victoryDisplay.SetActive(true);
        if (hasWon)
        {
            _victoryParent.SetActive(true);
            _victoryMessage.text = _currentLevel.VictoryMessage != string.Empty ? _currentLevel.VictoryMessage : "YOU WIN!";
            _defeatParent.SetActive(false);
            if(level.IsActive && _gameInfo.HighestLevelUnlocked < _currentLevel.LevelNumber + 1)
            {
                _gameInfo.HighestLevelUnlocked = _currentLevel.LevelNumber + 1;
                SaveGameInfo();
            }            
        }            
        else 
        {
            // Show broken bulb animation
            _brokenBulbAnimManager.Setup(this, _currentLevel.Bulbs.First(b => b.IsBroken));
            _currentLevel.gameObject.SetActive(false);
            SetOverlayState(false);
            _defeatParent.SetActive(true);
            _defeatMessage.text = _currentLevel.DefeatMessage != string.Empty ? _currentLevel.DefeatMessage : "SORRY YOU LOSE! TRY AGAIN!";
            _victoryParent.SetActive(false);
        }
    }
    #endregion Victory Defeat State (end)
    
    #region Level Select 
    internal void LevelSelect_LevelClicked(int levelNumber)
    {
        _currentLevel = _levels.Find(l => l.LevelNumber == levelNumber);
        TriggerLevelAnimation(LevelSelectClickedCallback);
    }

    public void NextPageLevelClicked()
    {
        var index = _levelPages.IndexOf(_levelPages.Find(l => l.activeSelf));
        _levelPages[index].SetActive(false);
        index++;
        SetPageLevelButtonState(index);
        if (index > _levelPages.Count - 1)
        {
            index = _levelPages.Count - 1;
        }
        else if (index < 0)
            index = 0;

        _levelPages[index].SetActive(true);        
    }

    public void PreviousPageLevelClicked()
    {
        var index = _levelPages.IndexOf(_levelPages.Find(l => l.activeSelf));
        _levelPages[index].SetActive(false);
        index--;
        SetPageLevelButtonState(index);

        if (index > _levelPages.Count - 1)
        {
            index = _levelPages.Count - 1;
        }
        else if (index < 0)
            index = 0;

        _levelPages[index].SetActive(true);        
    }

    private void SetPageLevelButtonState(int index)
    {
        _nextLevelPageButton.interactable = index < _levelPages.Count - 1;
        _previousLevelPageButton.interactable = index > 0;
    }

    private void LevelSelectClickedCallback()
    {
        SetActionMenuButtonsState();
        ResetLevel();
        SetLevelSelectState(false);
        SetOverlayState(true);
        var index = _levels.IndexOf(_levels.Find(l => l.IsActive));
        if (index == -1)
            index = 0;

        DisableLevel(index);
        InitializeLevel(_currentLevel.LevelNumber - 1);
        BatteryOptionsManager.Initialize();
    }
    #endregion Level Select (end)

    #region Animation
    private void TriggerLevelAnimation(Action callback)
    {
        StartCoroutine(LevelAnimationRoutine(callback));
    }

    private IEnumerator LevelAnimationRoutine(Action callback)
    {
        _levelTransition.ResetTrigger("Start");
        _levelTransition.SetTrigger("End");
        yield return new WaitForSecondsRealtime(2f);
        _levelTransition.SetTrigger("Start");
        _levelTransition.ResetTrigger("End");
        
        callback?.Invoke();
    }
    #endregion Animation (end)

    #region Initialization 
    public void SetLevelDisplay(int levelNumber)
    {
        _levelDisplay.text = $"<size=60%>Level</size> {levelNumber}";
    }
    
    private void InitializeAllLevels()
    {
        _levels = _levelsParent.GetComponentsInChildren<Level>(true).ToList();
        for (int i = 0; i < _levels.Count; ++i)
        {
            _levels[i].Setup(this);
            //if (i == 0)
            //    _levels[i].SetActiveState(true);
            //else
            //    _levels[i].SetActiveState(false);
        }
    }

    private void InitializePreviousLevel()
    {
        var index = _levels.IndexOf(_levels.Find(l => l.IsActive));
        if (index == -1)
            index = 0;
        DisableLevel(index);
        InitializeLevel(index - 1);
    }

    private void InitializeNextLevel()
    {
        var index = _levels.IndexOf(_levels.Find(l => l.IsActive));
        if (index == -1)
            index = 0;

        DisableLevel(index);
        InitializeLevel(index + 1);
    }

    private void DisableLevel(int index)
    {
        _levels[index].SetActiveState(false);
    }

    private void InitializeLevel(int index)
    {
        if (index >= _levels.Count())
            index = _levels.Count();
        else if (index < 0)
            index = 0;
        else
        {
            _levels[index].SetActiveState(true);
            _levels[index].Setup(this);
        }

        if (_levels[index].HasTutorial)
        {
            _tutorialButton.gameObject.SetActive(true);
            SetOverlayState(false);
            _levels[index].PlayTutorial();
            _batteryOptionsManager.SetInActive();
            _inhibitorOptionsManager.SetInactive();
            _passThroughOptionsManager.SetInactive();
        }
        else
        {
            _tutorialButton.gameObject.SetActive(false);
            SetOverlayState(true);
        }
    }

    private void InitializeLevelSelectScreen()
    {
        foreach(var level in _selectableLevels)
        {
            level.Setup(this, _gameInfo.HighestLevelUnlocked);
        }
    }

    private void SetMainCanvasState(bool enabled)
    {
        _mainCanvas.alpha = enabled ? 1 : 0;
        _mainCanvas.blocksRaycasts = enabled;
        _mainCanvas.interactable = enabled;
    }
    
    private void SetPlayScreenState(bool enabled)
    {
        _playScreen.alpha = enabled ? 1 : 0;
        _playScreen.blocksRaycasts = enabled;
        _playScreen.interactable = enabled;
    }

    private void SetOverlayState(bool enabled)
    {
        _levelOverlay.alpha = enabled ? 1 : 0;
        _levelOverlay.blocksRaycasts = enabled;
        _levelOverlay.interactable = enabled;
    }

    private void SetLevelSelectState(bool enabled)
    {
        _levelSelect.alpha = enabled ? 1 : 0;
        _levelSelect.blocksRaycasts = enabled;
        _levelSelect.interactable = enabled;
    }
    #endregion Initialization (end)

    #region Action Menu
    public void ResetLevel()
    {
        if (!_levels.Any(l => l.IsActive))
            return;

        _levels.Find(l => l.IsActive).RestartLevel();
        ResetVictoryState();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void NextLevelClicked()
    {
        if (_skipTransitions)
        {
            ResetLevel();
            InitializeNextLevel();
            ResetVictoryState();            
        }
        else
        {
            TriggerLevelAnimation(() => 
            {
                ResetLevel();
                InitializeNextLevel(); 
            });
            ResetVictoryState();
        }
        SetActionMenuButtonsState();
    }

    public void PreviousLevelClicked()
    {
        if (_skipTransitions)
        {
            ResetLevel();
            InitializePreviousLevel();
            ResetVictoryState();
        }
        else
        {
            TriggerLevelAnimation(() =>
            {
                ResetLevel();
                InitializePreviousLevel();
            });
            ResetVictoryState();
        }
        SetActionMenuButtonsState();
    }

    public void LevelSelectClicked()
    {
        InitializeLevelSelectScreen();
        SetLevelSelectState(true);
    }

    public void SettingsClicked()
    {
        TriggerLevelAnimation(InitializeSettings);
    }

    private void InitializeSettings()
    {
        _settingsManager.Setup();
    }

    public void ToggleActionMenu()
    {
        if (!_actionsMenu.activeSelf)
        {
            _actionsMenu.SetActive(true);
            AnimationController.Instance.AnimateWidth(0f, _originalWidth, 0f, _actionMenuAnimateTime, (RectTransform)_actionsMenu.transform, SetActionMenuActive);
            AnimationController.Instance.AnimateHeight(0f, _originalHeight, 0f, _actionMenuAnimateTime, (RectTransform)_actionsMenu.transform, null);

        }
        else
        {
            _textItems.ForEach(t => t.gameObject.SetActive(false));
            AnimationController.Instance.AnimateWidth(_actionMenuTransform.rect.width, 0f, 0f, _actionMenuAnimateTime, (RectTransform)_actionsMenu.transform, SetActionMenuInactive);
            AnimationController.Instance.AnimateHeight(_actionMenuTransform.rect.height, 0f, 0f, _actionMenuAnimateTime, (RectTransform)_actionsMenu.transform, null);
        }
    }

    private void SetActionMenuActive()
    {
        _textItems.ForEach(t => t.gameObject.SetActive(true));
    }

    private void SetActionMenuInactive()
    {
        _textItems.ForEach(t => t.gameObject.SetActive(false));
        _actionsMenu.SetActive(false);
    }

    private void SetActionMenuButtonsState()
    {
        if(_currentLevel!= null)
        {
            _previousLevelButton.interactable = _currentLevel.LevelNumber > 1;
            _nextLevelButton.interactable = _currentLevel.LevelNumber < _levels.Count - 1;
        }
        else
        {
            _nextLevelButton.interactable = true;
            _previousLevelButton.interactable = false;
        }
    }
    #endregion Action Menu (end)

    #region Tutorial
    public void PlayTutorialClicked()
    {
        SetOverlayState(false);
        _levels.Find(l => l.IsActive).PlayTutorial();
    }
    internal void TriggerAnimation(int levelNumber, int tutorialIndex)
    {
        
    }

    internal void OnTutorialComplete(Level level)
    {
        SetOverlayState(true);
        _batteryOptionsManager.gameObject.SetActive(true);
        _passThroughOptionsManager.gameObject.SetActive(level.HasPassThroughs);
        _inhibitorOptionsManager.gameObject.SetActive(level.HasInhibitors);
    }
    #endregion Tutorial (end)

    #region Saving/Loading
    private void SaveGameInfo()
    {
        //Debug.Log("Save Game Started.");
        _saveDataManager.SaveGame(SaveGameCallback);
    }

    
    private void SaveGameCallback()
    {
       // Debug.Log("Save Game Completed.");
    }

    private void LoadGameInfo()
    {
        //Debug.Log("Load Game Started.");
        _saveDataManager.LoadGame(LoadGameCallback);
    }

    private void LoadGameCallback()
    {
        //Debug.Log("Load Game Completed.");
        if(_unlockAllLevels)
            _gameInfo.HighestLevelUnlocked = 30;
    }

    
    #endregion Saving/Loading (end)
    #endregion Methods (end)
}