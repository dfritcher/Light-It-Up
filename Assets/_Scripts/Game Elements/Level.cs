using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Level : MonoBehaviour
{
    #region Fields, Properties
    [Header("Object Variables")]
    // Start is called before the first frame update
    [SerializeField]
    private List<Battery> _batteries = null;

    [SerializeField]
    private List<Bulb> _bulbs = null;
    public List<Bulb> Bulbs { get { return _bulbs; } }

    [SerializeField]
    private List<Wire> _wires = null;

    [SerializeField]
    private List<Inhibitor> _inhibitors = null;
    public bool HasInhibitors { get { return _inhibitors.Count > 0; } }

    [SerializeField]
    private List<PassThrough> _passThroughs = null;
    public bool HasPassThroughs { get { return _passThroughs.Count > 0; } }

    [SerializeField]
    private int _levelNumber = 0;
    public int LevelNumber { get { return _levelNumber; } }

    [Header("Scene References"), Space(8)]
    [SerializeField]
    private GameObject _gameObjectsParent = null;
    [SerializeField]
    private string _victoryMessage = string.Empty;
    public string VictoryMessage { get { return _victoryMessage; } }
    [SerializeField]
    private string _defeatMessage = string.Empty;
    public string DefeatMessage { get { return _defeatMessage; } }
    [SerializeField]
    private Sprite[] _hints = null;

    [SerializeField]
    private AudioClip _levelMusic = null;

    [SerializeField]
    private bool _allBatteryColorsAvailable = false;
    public bool AllBatteryColorsAvailable { get { return _allBatteryColorsAvailable; } }

    private bool _isActive = false;
    public bool IsActive { get { return _isActive; } }

    private LevelManager _levelManager = null;
    public LevelManager LevelManager { get { return _levelManager; } }
    
    [SerializeField]
    private bool _canPlay = false;
    public bool CanPlay { get { return _canPlay; } }
    private bool _hasWon = false;
    private int _hintIndex = 0;
    public bool HasHints { get { return _hints.Count() > 0; } }
    public bool NextHintAvailable { get { return _hintIndex != _hints.Length - 1; } }
    #region Tutorial
    [Header("Tutorial"), Space(8)]
    [SerializeField]
    private bool _hasTutorial = false;
    public bool HasTutorial { get { return _hasTutorial; } }

    [SerializeField]
    private Canvas _levelCanvas = null;

    [SerializeField]
    private TutorialResolverBase _tutorialResolver = null;

    //[SerializeField]
    //private Button _nextButton = null;

    //[SerializeField]
    //private Button _skipButton = null;

    //[SerializeField]
    //private List<TextMeshProUGUI> _tutorialTexts;

    //[SerializeField]
    //private GameObject _tutorialObjectsParent = null;

    //[SerializeField]
    //private Animator _fingerAnimator = null;

    //[SerializeField]
    //private Animator _batteryAnimator = null;

    //[SerializeField]
    //private Animator _wireAnimator = null;

    //[SerializeField]
    //private Animator _bulbAnimator = null;

    //[SerializeField]
    //private Animator _batteryOptionsAnimator = null;

    //private int _tutorialIndex = 0;
    #endregion Tutorial (end)

    #endregion Fields, Properties (end)

    #region Methods
    #region Unity Engine Hooks
    private void Awake()
    {
        _canPlay = true;
        _hasWon = false;
        _batteries = _gameObjectsParent.transform.GetComponentsInChildren<Battery>(true).ToList();
        _bulbs = _gameObjectsParent.transform.GetComponentsInChildren<Bulb>(true).ToList();
        _wires = _gameObjectsParent.transform.GetComponentsInChildren<Wire>(true).ToList();
        _inhibitors = _gameObjectsParent.transform.GetComponentsInChildren<Inhibitor>(true).ToList();
        _passThroughs = _gameObjectsParent.transform.GetComponentsInChildren<PassThrough>(true).ToList();
    }
    
    private void Start()
    {
        foreach (var bulb in _bulbs)
        {
            bulb.Setup();
        }
        foreach (var wire in _wires)
        {
            wire.Setup();
        }
        foreach(var inhibitor in _inhibitors)
        {
            inhibitor.Setup(this);
            inhibitor.OnClick += Inhibitor_OnClick;
        }
        foreach (var passThrough in _passThroughs)
        {
            passThrough.Setup(this);
            passThrough.OnClick += PassThrough_OnClick;
        }
        foreach (var battery in _batteries)
        {
            battery.Setup(this, LevelManager.Instance.MainCamera);
            battery.OnClick += Battery_OnClick;
        }

        if (_hasTutorial)
        {
            _tutorialResolver.Setup(this, _levelManager.MainCanvasScaler);
            //_tutorialObjectsParent.SetActive(true);
            //_gameObjectsParent.SetActive(false);
            //InitializeTutorial(0);
        }
        _levelManager.SetLevelMusic(_levelMusic);
    }

    private void LateUpdate()
    {
        if (_canPlay && IsActive)
        {
            CheckWinCondition();
        }        
    }
    #endregion Unity Engine Hooks (end)

    public void Setup(LevelManager levelManager)
    {
        _levelManager = levelManager;        
    }

    public void RestartLevel()
    {
        _bulbs.ForEach(b => b.ResetPowerable());
        _wires.ForEach(w => w.ResetPowerable());
        _inhibitors.ForEach(i => i.ResetPowerable());
        _passThroughs.ForEach(p => p.ResetPowerable());
        _batteries.ForEach(p => p.ResetPowerable());
        _batteries.ForEach(p => p.ResetPower());
        _levelManager.ResetVictoryState();
        _canPlay = true;
        _hasWon = false;
    }

    public void SetActiveState(bool isActive)
    {
        _isActive = isActive;
        gameObject.SetActive(_isActive);
        if(_isActive)
            _levelManager?.SetLevelDisplay(_levelNumber);
    }

    public Sprite GetHint()
    {       
        return _hints[_hintIndex];
    }

    public Sprite GetNextHint()
    {
        _hintIndex++;
        if (_hintIndex > _hints.Length - 1)
            _hintIndex = _hints.Length - 1;
        return GetHint();
    }

    private void Battery_OnClick(Battery battery)
    {
        _batteries.ForEach(b => {
            if (b != battery)
            {
                b.SetSelectedState(false);
            }
        });
        _levelManager.BatteryOptionsManager.Setup(battery, _allBatteryColorsAvailable);
        _levelManager.InhibitorOptionsManager.AnimateOptionPanel(true);
        _levelManager.PassThroughOptionsManager.AnimateOptionPanel(true);
    }

    private void PassThrough_OnClick(PassThrough passThrough)
    {
        _passThroughs.ForEach(p => {
            if (p != passThrough)
            {
                p.SetSelectedState(false);
            }
        });
        _levelManager.PassThroughOptionsManager.Setup(passThrough);
        _levelManager.BatteryOptionsManager.AnimateOptionPanel(true);
        _levelManager.InhibitorOptionsManager.AnimateOptionPanel(true);
    }

    private void Inhibitor_OnClick(Inhibitor inhibitor)
    {
        _inhibitors.ForEach(i => {
            if (i != inhibitor)
            {
                i.SetSelectedState(false);
            }
        });
        _levelManager.InhibitorOptionsManager.Setup(inhibitor);
        _levelManager.BatteryOptionsManager.AnimateOptionPanel(true);
        _levelManager.PassThroughOptionsManager.AnimateOptionPanel(true);
    }

    private void CheckWinCondition()
    {
        if (_bulbs == null || _levelManager == null)
            return;
        if(_bulbs.All(b => b.IsOn))
        {
            _levelManager.SetVictoryState(true, this);
            _levelManager.InhibitorOptionsManager.ResetOptions();
            _levelManager.BatteryOptionsManager.ResetOptions();
            _levelManager.PassThroughOptionsManager.ResetOptions();
            _canPlay = false;
        }
        else if(_bulbs.Any(b => b.IsBroken))
        {
            _levelManager.SetVictoryState(false, this);
            _levelManager.InhibitorOptionsManager.ResetOptions();
            _levelManager.BatteryOptionsManager.ResetOptions();
            _levelManager.PassThroughOptionsManager.ResetOptions();
            _hasWon = false;
            _canPlay = false;
        }
        if(!_canPlay)
            AnimationController.Instance.StopAllCoroutines();
    }

    #region Tutorial
    public void PlayTutorial()
    {
        _tutorialResolver.InitializeTutorial(0);        
    }
    //public void TutorialSkipClicked()
    //{
    //    _tutorialIndex = 0;
    //    //Stop any animations
    //    _tutorialObjectsParent.SetActive(false);
    //    _gameObjectsParent.SetActive(true);
    //}
    //public void TutorialNextClicked()
    //{
    //    _nextButton.interactable = false;
    //    _tutorialIndex++;
    //    InitializeTutorial(_tutorialIndex);
    //    //Play animation at specific points.
    //    //_levelManger.TriggerAnimation(_levelNumber, _tutorialIndex);
    //    //How can we tie this all together genericly for all levels????
    //}

    //internal void OnFingerAnimationEnd(int index)
    //{        
    //    switch (index)
    //    {
    //        case 3:
    //            _batteryOptionsAnimator.SetBool("BatteryOptions_MoveUp", true);
    //            _nextButton.interactable = true;
    //            break;
    //        case 4:
    //            _batteryAnimator.SetBool($"Battery_RedOn", true);
    //            _bulbAnimator.SetBool($"RedOn", true);
    //            _wireAnimator.SetBool($"RedOn", true);
    //            break;
    //    }        
    //}

    //private void InitializeTutorial(int index)
    //{
    //    if (index < 0)
    //        index = 0;
    //    if (index > _tutorialTexts.Count() - 1)
    //        index = _tutorialTexts.Count() - 1;

    //    //for(int i = 0; i < _tutorialTexts.Count(); i++)
    //    //{
    //    //    _tutorialTexts[i].gameObject.SetActive(i == index);
    //    //}
    //    _tutorialTexts.ForEach(t => t.gameObject.SetActive(false));
    //    _tutorialTexts[index].gameObject.SetActive(true);

    //    switch (index)
    //    {
    //        case 0:                
    //        case 1:
    //        case 2:
    //            _nextButton.interactable = true;
    //            break;
    //        case 3:
    //            _fingerAnimator.SetBool($"Lvl{_levelNumber}_Finger{1}", true);
    //            _nextButton.interactable = false;
    //            break;
    //        case 4:
    //            _fingerAnimator.SetBool($"Lvl{_levelNumber}_Finger{2}", true);
    //            _nextButton.interactable = false;
    //            break;
    //        default:
    //            _nextButton.interactable = true;
    //            break;
    //    }
    //}


    public void TutorialAnimationEnd(int index)
    {
        //_nextButton.interactable = true;
        //_tutorialIndex++;
        //InitializeTutorial(_tutorialIndex);
        _levelManager.TriggerAnimation(_levelNumber, index);
    }

    internal void OnTutorialResolverCloseClicked()
    {
        _levelManager.OnTutorialComplete(this);
    }

    internal void OnTutorialResolvedSkipClosed()
    {
        _levelManager.OnTutorialComplete(this);
    }
    #endregion Tutorial (end)
    #endregion Methods (end)
}
