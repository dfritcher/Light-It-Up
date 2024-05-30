using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LevelSixTutorialResolver : TutorialResolverBase
{
    #region Fields, Properties
    [SerializeField] private Transform _battery1Location = null;

    [SerializeField] private Animator _battery1Animator = null;

    [SerializeField] private Transform _battery2Location = null;

    [SerializeField] private Animator _battery2Animator = null;

    [SerializeField] private Animator[] _wireAnimators = null;

    /// <summary>
    /// Reference to the wire connected to the second battery.
    /// </summary>
    [SerializeField] private Animator _battery2WireAnimator = null;

    /// <summary>
    /// Reference to the wire connected to the second battery.
    /// </summary>
    [SerializeField] private Animator _battery3WireAnimator = null;

    [SerializeField] private Bulb _singleColorBulb = null;

    [SerializeField] private Animator _singleColorBulbAnimator = null;

    [SerializeField] private Bulb _singleColor2Bulb = null;

    [SerializeField] private Animator _singleColor2BulbAnimator = null;

    [SerializeField] private Bulb _doubleColorBulb = null;

    [SerializeField] private Animator _doubleColorBulbAnimator = null;

    [SerializeField] private Animator _batteryOptionsAnimator = null;

    #endregion Fields, Properties (end)

    #region Methods
    public override void Setup(Level level, CanvasScaler canvasScaler)
    {
        _level = level;
        _fingerLocations[1] = _level.LevelManager.MainCamera.WorldToScreenPoint(_battery1Location.position);
        _fingerLocations[3] = _level.LevelManager.MainCamera.WorldToScreenPoint(_battery2Location.position);
        _canvasScaler = canvasScaler;
    }

    public override void OnCloseClicked()
    {
        _battery1Animator.SetTrigger("Reset");
        _battery2Animator.SetTrigger("Reset");
        _doubleColorBulbAnimator.SetTrigger("ExitBroken");        
        ResetTriggers();
        base.OnCloseClicked();
    }

    public override void OnSkipClicked()
    {
        _battery1Animator.SetTrigger("Reset");
        _battery2Animator.SetTrigger("Reset");
        ResetTriggers();
        base.OnSkipClicked();
    }

    public override void InitializeTutorial(int index)
    {
        _tutorialIndex = index;
        _tutorialObjectsParent.SetActive(true);
        _gameObjectsParent.SetActive(false);
        _nextButton.gameObject.SetActive(true);
        _closeButton.gameObject.SetActive(false);
        _singleColorBulb.ResetPowerable();
        _singleColor2Bulb.ResetPowerable();
        _doubleColorBulb.ResetPowerable(); //Used mainly to reset the text display;
        _singleColorBulbAnimator.SetTrigger("RedOff");
        _singleColorBulbAnimator.SetTrigger("RedUnlitOn");
        _singleColorBulbAnimator.SetTrigger("SetNormalImage1");
        _singleColor2BulbAnimator.SetTrigger("BlueOff");
        _singleColor2BulbAnimator.SetTrigger("BlueUnlitOn");
        _singleColor2BulbAnimator.SetTrigger("SetNormalImage1");
        _doubleColorBulbAnimator.SetTrigger("RedBlueUnlitOn");
        _doubleColorBulbAnimator.SetTrigger("SetNormalImage1");
        
        index = ValidateIndexValue(index);
        SetTutorialTextState(index);
        HandleTutorialStateByIndex(index);
        ResetFingerLocation();
    }
    
    public override void TutorialAnimationEnd(int index)
    {
        _level.TutorialAnimationEnd(index);
    }

    internal override void HandleTutorialStateByIndex(int index)
    {
        switch (index)
        {
            case 0:
                ResetFingerLocation();
                _nextButton.interactable = true;
                break;
            case 1:
                _nextButton.interactable = false;
                MoveFinger(1, _fingerTransform.position, _fingerLocations[1], false);
                break;
            case 2:
                _nextButton.interactable = false;
                MoveFinger(2, _fingerTransform.localPosition, _fingerLocations[2], true);
                break;
            case 3:                
                _nextButton.interactable = false;
                MoveFinger(3, _fingerTransform.position, _fingerLocations[3], false);
                break;
            case 4:                
                _nextButton.interactable = false;
                MoveFinger(4, _fingerTransform.localPosition, _fingerLocations[4], true);
                break;
            case 5:
                _nextButton.gameObject.SetActive(false);
                _closeButton.gameObject.SetActive(true);
                break;
            default:
                _nextButton.interactable = true;
                break;
        }
    }

    internal override void MoveFingerEnd()
    {
        switch (_animationIndex)
        {
            case 1:
                _fingerAnimator.SetTrigger("Lvl6_Finger1");
                break;
            case 2:
                _fingerAnimator.SetTrigger("Lvl6_Finger2");
                break;
            case 3:
                _fingerAnimator.SetTrigger("Lvl6_Finger3");
                break;
            case 4:
                _fingerAnimator.SetTrigger("Lvl6_Finger4");
                break;            
        }
    }

    public override void OnFingerAnimationEnd(int index)
    {
        switch (index)
        {
            case 1:
                _battery1Animator.SetTrigger("HighlightOn");
                _batteryOptionsAnimator.SetTrigger("MoveUp");
                _nextButton.interactable = true;
                break;
            case 2:
                _battery1Animator.SetTrigger("HighlightOff");
                _battery1Animator.SetTrigger("RedOn");
                _singleColorBulbAnimator.SetTrigger("RedOn");
                _singleColorBulbAnimator.SetTrigger("RedUnlitOff");
                _singleColorBulb.IncreasePower(1, true);
                _doubleColorBulbAnimator.SetTrigger("RedBlueOn");
                _doubleColorBulb.IncreasePower(1, true);
                _wireAnimators.ToList().ForEach(w => w.SetTrigger("RedOn"));
                _nextButton.interactable = true;
                break;
            case 3:
                _battery2Animator.SetTrigger("HighlightOn");
                _nextButton.interactable = true;
                break;
            case 4:
                _doubleColorBulbAnimator.SetTrigger("RedBlueOff");
                _doubleColorBulbAnimator.SetTrigger("SetBrokenImage1");
                _battery2Animator.SetTrigger("BlueOn");
                _doubleColorBulb.IncreasePower(1, true);
                _singleColor2Bulb.IncreasePower(1, true);
                _singleColor2BulbAnimator.SetTrigger("BlueOn");
                _battery2WireAnimator.SetTrigger("BlueOn");
                _battery3WireAnimator.SetTrigger("BlueOn");
                _nextButton.interactable = true;
                break;            
        }
    }

    private void ResetTriggers()
    {
        if (_battery1Animator.isActiveAndEnabled)
        {
            _battery1Animator.ResetTrigger("RedOn");
            _battery1Animator.ResetTrigger("RedOff");
            _battery1Animator.ResetTrigger("HighlightOn");
            _battery1Animator.ResetTrigger("HighlightOff");
            _battery1Animator.Rebind();
        }
        if (_battery2Animator.isActiveAndEnabled)
        {
            _battery2Animator.ResetTrigger("RedOn");
            _battery2Animator.ResetTrigger("RedOff");
            _battery2Animator.ResetTrigger("HighlightOn");
            _battery2Animator.ResetTrigger("HighlightOff");
            _battery2Animator.Rebind();
        }
        
        _batteryOptionsAnimator.ResetTrigger("MoveUp");
        _doubleColorBulbAnimator.ResetTrigger("RedBlueOn");
                
        _animationIndex = 0;
    }
    #endregion Methods (end)
}