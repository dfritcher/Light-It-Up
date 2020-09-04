using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using System;

public class LevelManager : MonoBehaviour
{
    #region Fields, Properties
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
    private BatteryOptionsManager _batteryOptionsManager = null;
    public BatteryOptionsManager BatteryOptionsManager { get { return _batteryOptionsManager; } }

    [SerializeField]
    private InhibitorOptionsManager _inhibitorOptionsManager = null;
    public InhibitorOptionsManager InhibitorOptionsManager { get { return _inhibitorOptionsManager; } }

    [SerializeField]
    private PassThroughOptionsManager _passThroughOptionsManager = null;
    public PassThroughOptionsManager PassThroughOptionsManager { get {return _passThroughOptionsManager; } }

    [SerializeField]
    private GameObject _actionsMenu = null;

    [SerializeField]
    private Animator _levelTransition = null;
    [SerializeField]
    private GameObject _transitionObject = null;

    [SerializeField]
    private CanvasGroup _levelOverlay = null;
    [SerializeField]
    private CanvasGroup _playScreen = null;

    [SerializeField]
    private Camera _mainCamera;
    public Camera MainCamera { get { return _mainCamera; } }

    [Space(10), Header("Action Menu")]
    [SerializeField]
    private RectTransform _actionMenu = null;

    private float _originalWidth = 0f;
    private float _originalHeight = 0f;

    [SerializeField]
    private float _actionMenuAnimateTime = 0f;

    [SerializeField]
    private List<TextMeshProUGUI> _textItems = null;

    [Header("Tutorial References"), Space(8)]
    [SerializeField]
    private Button _tutorialButton = null;

    [Space(10), Header("Debug Options")]
    [SerializeField]
    private bool _skipTransitions = false;


    private static LevelManager _instance = null;
    public static LevelManager Instance { get { return _instance; } }
    #endregion Fields, Properties (end)

    #region Methods
    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(_instance.gameObject);

        _victoryDisplay.SetActive(false);
        _originalWidth = _actionMenu.rect.width;
        _originalHeight = _actionMenu.rect.height;
        _actionMenu.sizeDelta = new Vector2(0f, 0f);

        SetActionMenuInactive();
        SetOverlayState(false);
        SetPlayScreenState(true);
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

    public void ResetVictoryState()
    {
        _victoryDisplay.SetActive(false);
        _victoryMessage.text = string.Empty;
        _defeatMessage.text = string.Empty;
    }

    public void SetVictoryState(bool hasWon, string message = "")
    {
        _victoryDisplay.SetActive(true);
        if (hasWon)
        {
            _victoryParent.SetActive(true);
            _victoryMessage.text = message != string.Empty ? message : "YOU WIN!";
            _defeatParent.SetActive(false);
        }            
        else 
        {
            _defeatParent.SetActive(true);
            _defeatMessage.text = message != string.Empty ? message : "SORRY YOU LOSE! \n TRY AGAIN!";
            _victoryParent.SetActive(false);
        }

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
            AnimationController.Instance.AnimateWidth(_actionMenu.rect.width, 0f, 0f, _actionMenuAnimateTime, (RectTransform)_actionsMenu.transform, SetActionMenuInactive);
            AnimationController.Instance.AnimateHeight(_actionMenu.rect.height, 0f, 0f, _actionMenuAnimateTime, (RectTransform)_actionsMenu.transform, null);
        }        
    }
    
    private void PlayClickedCallback()
    {
        SetPlayScreenState(false);
        SetOverlayState(true);
        InitializeLevel(0);
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
