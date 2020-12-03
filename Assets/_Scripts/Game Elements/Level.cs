﻿using System;
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
    private List<Junction> _junctions = null;

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
    private AudioClip _levelMusic = null;

    [SerializeField]
    private bool _allBatteryColorsAvailable = false;
    public bool AllBatteryColorsAvailable { get { return _allBatteryColorsAvailable; } }

    private bool _isActive = false;
    public bool IsActive { get { return _isActive; } }

    private LevelManager _levelManger = null;

    private bool _canPlay = false;
    private bool _hasWon = false;

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
    private void Awake()
    {
        _canPlay = true;
        _hasWon = false;
        _batteries = _gameObjectsParent.transform.GetComponentsInChildren<Battery>(true).ToList();
        _bulbs = _gameObjectsParent.transform.GetComponentsInChildren<Bulb>(true).ToList();
        _wires = _gameObjectsParent.transform.GetComponentsInChildren<Wire>(true).ToList();
        _junctions = _gameObjectsParent.transform.GetComponentsInChildren<Junction>(true).ToList();
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
        foreach(var junction in _junctions)
        {
            junction.Setup();
        }
        foreach(var inhibitor in _inhibitors)
        {
            inhibitor.Setup();
            inhibitor.OnClick += Inhibitor_OnClick;
        }
        foreach (var passThrough in _passThroughs)
        {
            passThrough.Setup();
            passThrough.OnClick += PassThrough_OnClick;
        }
        foreach (var battery in _batteries)
        {
            battery.Setup(LevelManager.Instance.MainCamera);
            battery.OnClick += Battery_OnClick;
        }

        if (_hasTutorial)
        {
            _tutorialResolver.Setup(this);
            //_tutorialObjectsParent.SetActive(true);
            //_gameObjectsParent.SetActive(false);
            //InitializeTutorial(0);
        }
        _levelManger.SetLevelMusic(_levelMusic);
    }

    private void LateUpdate()
    {
        if (_canPlay && IsActive)
        {
            CheckWinCondition();
        }        
    }

    public void Setup(LevelManager levelManager)
    {
        _levelManger = levelManager;        
    }

    public void RestartLevel()
    {
        _bulbs.ForEach(b => b.ResetPowerable());
        _wires.ForEach(w => w.ResetPowerable());
        _junctions.ForEach(j => j.ResetPowerable());
        _inhibitors.ForEach(i => i.ResetPowerable());
        _passThroughs.ForEach(p => p.ResetPowerable());
        _batteries.ForEach(p => p.ResetPowerable());
        _batteries.ForEach(p => p.ResetPower());
        _levelManger.ResetVictoryState();
        _canPlay = true;
        _hasWon = false;
    }

    public void SetActiveState(bool isActive)
    {
        _isActive = isActive;
        gameObject.SetActive(_isActive);
        if(_isActive)
            _levelManger?.SetLevelDisplay(_levelNumber);
    }

    private void Battery_OnClick(Battery battery)
    {
        _batteries.ForEach(b => {
            if (b != battery)
            {
                b.SetSelectedState(false);
            }
        });
        _levelManger.BatteryOptionsManager.Setup(battery, _allBatteryColorsAvailable);
        _levelManger.InhibitorOptionsManager.AnimateOptionPanel(true);
        _levelManger.PassThroughOptionsManager.AnimateOptionPanel(true);
    }

    private void PassThrough_OnClick(PassThrough passThrough)
    {
        _passThroughs.ForEach(p => {
            if (p != passThrough)
            {
                p.SetSelectedState(false);
            }
        });
        _levelManger.PassThroughOptionsManager.Setup(passThrough);
        _levelManger.BatteryOptionsManager.AnimateOptionPanel(true);
        _levelManger.InhibitorOptionsManager.AnimateOptionPanel(true);
    }

    private void Inhibitor_OnClick(Inhibitor inhibitor)
    {
        _inhibitors.ForEach(i => {
            if (i != inhibitor)
            {
                i.SetSelectedState(false);
            }
        });
        _levelManger.InhibitorOptionsManager.Setup(inhibitor);
        _levelManger.BatteryOptionsManager.AnimateOptionPanel(true);
        _levelManger.PassThroughOptionsManager.AnimateOptionPanel(true);
    }

    private void CheckWinCondition()
    {
        if (_bulbs == null || _levelManger == null)
            return;
        if(_bulbs.All(b => b.IsOn))
        {
            _levelManger.SetVictoryState(true, this);
            _levelManger.InhibitorOptionsManager.ResetOptions();
            _levelManger.BatteryOptionsManager.ResetOptions();
            _levelManger.PassThroughOptionsManager.ResetOptions();
            _canPlay = false;
        }
        else if(_bulbs.Any(b => b.IsBroken))
        {
            _levelManger.SetVictoryState(false, this);
            _levelManger.InhibitorOptionsManager.ResetOptions();
            _levelManger.BatteryOptionsManager.ResetOptions();
            _levelManger.PassThroughOptionsManager.ResetOptions();
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
        _levelManger.TriggerAnimation(_levelNumber, index);
    }

    internal void OnTutorialResolverCloseClicked()
    {
        _levelManger.OnTutorialComplete(this);
    }

    internal void OnTutorialResolvedSkipClosed()
    {
        _levelManger.OnTutorialComplete(this);
    }
    #endregion Tutorial (end)
    #endregion Methods (end)
}