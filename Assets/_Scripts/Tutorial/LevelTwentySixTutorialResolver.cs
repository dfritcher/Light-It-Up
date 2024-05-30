using UnityEngine;
using UnityEngine.UI;

public class LevelTwentySixTutorialResolver : TutorialResolverBase
{
    #region Fields, Properties
    [SerializeField]
    private Transform _battery1Location = null;

    [SerializeField]
    private Battery _battery1 = null;

    [SerializeField]
    private Animator _battery1Animator = null;
   
    /// <summary>
    /// Reference to the wires connected to the first Inhibitor.
    /// </summary>
    [SerializeField]
    private Animator[] _wire1Animators = null;

    /// <summary>
    /// Reference to the wire connected to the second Inhibitor.
    /// </summary>
    [SerializeField]
    private Animator[] _wire2Animators = null;
    
    [SerializeField]
    private Bulb _singlePowerBulb = null;

    [SerializeField]
    private Animator _singlePowerBulbAnimator = null;

    [SerializeField]
    private Bulb _doublePowerBulb = null;

    [SerializeField]
    private Animator _doublePowerBulbAnimator = null;

    [SerializeField]
    private Animator _batteryOptionsAnimator = null;

    #endregion Fields, Properties (end)

    #region Methods
    public override void Setup(Level level, CanvasScaler canvasScaler)
    {
        _level = level;
        _fingerLocations[1] = _level.LevelManager.MainCamera.WorldToScreenPoint(_battery1Location.position);
        _fingerLocations[3] = _level.LevelManager.MainCamera.WorldToScreenPoint(new Vector3(_battery1Location.position.x + .8f, _battery1Location.position.y, _battery1Location.position.z));
        _canvasScaler = canvasScaler;
    }

    public override void OnCloseClicked()
    {
        _battery1Animator.SetTrigger("Reset");
        _singlePowerBulbAnimator.SetTrigger("SetNormalImage1");
        _doublePowerBulbAnimator.SetTrigger("SetNormalImage2");
        ResetTriggers();
        base.OnCloseClicked();
    }

    public override void OnSkipClicked()
    {
        _battery1Animator.SetTrigger("Reset");
        
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
        _singlePowerBulb.ResetPowerable();
        _singlePowerBulbAnimator.SetTrigger("RedUnlitOn");
        _singlePowerBulbAnimator.SetTrigger("RedOff");
        _singlePowerBulbAnimator.SetTrigger("SetNormalImage1");
        _doublePowerBulb.ResetPowerable();
        _doublePowerBulbAnimator.SetTrigger("RedOff");
        _doublePowerBulbAnimator.SetTrigger("SetNormalImage2");
        _battery1.ResetPowerable();
        index = ValidateIndexValue(index);
        SetTutorialTextState(index);
        HandleTutorialStateByIndex(index);
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
            default:
                _nextButton.interactable = true;
                break;
        }
    }

    public override void OnFingerAnimationEnd(int index)
    {
        switch (index)
        {
            case 0:
                _nextButton.interactable = true;
                break;
            case 1:
                _battery1Animator.SetTrigger("HighlightOn");
                _batteryOptionsAnimator.SetTrigger("MoveUp");
                _nextButton.interactable = true;
                break;
            case 2:
                _battery1Animator.SetTrigger("HighlightOff");
                _battery1Animator.SetTrigger("AllOn");
                _wire1Animators[0].SetTrigger("AllOn");
                _wire2Animators[0].SetTrigger("AllOn");
                _singlePowerBulbAnimator.SetTrigger("RedOn");
                _singlePowerBulbAnimator.SetTrigger("RedUnlitOff");
                _singlePowerBulb.IncreasePower(1, true);
                _doublePowerBulb.IncreasePower(1, true);
                _nextButton.interactable = true;
                break;
            case 3:
                _batteryOptionsAnimator.SetTrigger("MoveDown");
                _battery1.IncreasePower();
                _singlePowerBulb.IncreasePower(1, true);
                _doublePowerBulb.IncreasePower(1, true);
                _singlePowerBulbAnimator.SetTrigger("RedOff");                
                _singlePowerBulbAnimator.SetTrigger("SetBrokenImage1");
                _doublePowerBulbAnimator.SetTrigger("RedOn");
                _closeButton.gameObject.SetActive(true);
                _nextButton.interactable = false;
                _nextButton.gameObject.SetActive(false);
                break;            
        }        
    }

    internal override void MoveFingerEnd()
    {
        switch (_animationIndex)
        {
            case 1:
                _fingerAnimator.SetTrigger("Lvl26_Finger1");
                break;
            case 2:
                _fingerAnimator.SetTrigger("Lvl26_Finger2");
                break;
            case 3:
                _fingerAnimator.SetTrigger("Lvl26_Finger3");
                break;
            case 4:
                _fingerAnimator.SetTrigger("Lvl26_Finger4");
                break;
            case 5:
                _fingerAnimator.SetTrigger("Lvl26_Finger5");
                break;
            case 6:
                _fingerAnimator.SetTrigger("Lvl26_Finger6");
                break;
            case 7:
                _fingerAnimator.SetTrigger("Lvl26_Finger7");
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
        }
        if(_batteryOptionsAnimator.isActiveAndEnabled)
            _batteryOptionsAnimator.ResetTrigger("MoveUp");        
    }
    #endregion Methods (end)
}