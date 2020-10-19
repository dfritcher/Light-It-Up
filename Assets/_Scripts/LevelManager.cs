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

    [Header("Action Menu"), Space(8)]
    [SerializeField]
    private GameObject _actionsMenu = null;
    [SerializeField, FormerlySerializedAs("_actionMenu")]
    private RectTransform _actionMenuTransform = null;

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

    [Header("Debug Options"), Space(8)]
    [SerializeField]
    private bool _skipTransitions = false;


    private static LevelManager _instance = null;
    public static LevelManager Instance { get { return _instance; } }

    private Level _currentLevel = null;
    private Level CurrentLevel {  get { return _currentLevel ?? (_currentLevel = _levels[0]); } }
    #endregion Fields, Properties (end)

    #region Methods
    private void Awake()
    {
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
        
    public void PlayClicked()
    {
        TriggerLevelAnimation(PlayClickedCallback);        
    }

    public void NextLevelClicked()
    {
        if (_skipTransitions)
        {
            InitializeNextLevel();
            ResetVictoryState();
        }
        else
        {
            TriggerLevelAnimation(InitializeNextLevel);
            ResetVictoryState();
        }        
    }
    
    public void PreviousLevelClicked()
    {
        if (_skipTransitions)
        {
            InitializePreviousLevel();
            ResetVictoryState();
        }
        else
        {
            TriggerLevelAnimation(InitializePreviousLevel);
            ResetVictoryState();
        }
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

    public void PlayTutorialClicked()
    {
        SetOverlayState(false);
        _levels.Find(l => l.IsActive).PlayTutorial();        
    }

    internal void LevelSelect_LevelClicked(int levelNumber)
    {
        _currentLevel = _levels.Find(l => l.LevelNumber == levelNumber);
        TriggerLevelAnimation(LevelSelectClickedCallback);
    }

    public void ResetVictoryState()
    {
        _victoryDisplay.SetActive(false);
        _victoryMessage.text = string.Empty;
        _defeatMessage.text = string.Empty;
    }

    internal void SetLevelMusic(AudioClip levelMusic)
    {
        AudioManager.SetMusic(levelMusic, true);
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
        }            
        else 
        {
            // Show broken bulb animation
            _brokenBulbAnimManager.Setup(this, _currentLevel.Bulbs.First(b => b.IsBroken));
            _currentLevel.gameObject.SetActive(false);
            SetOverlayState(false);
            _defeatParent.SetActive(true);
            _defeatMessage.text = _currentLevel.DefeatMessage != string.Empty ? _currentLevel.DefeatMessage : "SORRY YOU LOSE! \n TRY AGAIN!";
            _victoryParent.SetActive(false);
        }
    }

    public void OnBrokenBulbAnimationEnd()
    {
        SetOverlayState(true);
        _currentLevel.gameObject.SetActive(true);
    }

    public void SetLevelDisplay(int levelNumber)
    {
        _levelDisplay.text = $"<size=60%>Level</size> {levelNumber}";
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
    
    private void PlayClickedCallback()
    {
        SetLevelSelectState(true);
        SetOverlayState(false);
        SetPlayScreenState(false);
        InitializeLevelSelectScreen();
    }

    private void LevelSelectClickedCallback()
    {
        SetLevelSelectState(false);
        SetOverlayState(true);
        InitializeLevel(_currentLevel.LevelNumber - 1);
        BatteryOptionsManager.Initialize();
    }

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

    private void SetActionMenuActive()
    {
        _textItems.ForEach(t => t.gameObject.SetActive(true));
    }

    private void SetActionMenuInactive()
    {
        _textItems.ForEach(t => t.gameObject.SetActive(false));
        _actionsMenu.SetActive(false);
    }

    #region Tutorial
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
    #endregion Methods (end)
}
