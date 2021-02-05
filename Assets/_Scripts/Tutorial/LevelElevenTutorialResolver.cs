using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LevelElevenTutorialResolver : TutorialResolverBase
{
    #region Fields, Properties
    [SerializeField]
    private Transform _battery1Location = null;
    [SerializeField]
    private Animator _battery1Animator = null;

    [SerializeField]
    private Transform _inhibitor1Location = null;
    [SerializeField]
    private Animator _inhibitor1Animator = null;

    [SerializeField]
    private Transform _inhibitor2Location = null;
    [SerializeField]
    private Animator _inhibitor2Animator = null;

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
    private Bulb _singleColorBulb = null;

    [SerializeField]
    private Animator _singleColorBulbAnimator = null;

    [SerializeField]
    private Bulb _doubleColorBulb = null;

    [SerializeField]
    private Animator _doubleColorBulbAnimator = null;

    [SerializeField]
    private Animator _batteryOptionsAnimator = null;

    [SerializeField]
    private Animator _inhibitorOptionsAnimator = null;
    #endregion Fields, Properties (end)

    #region Methods
    public override void Setup(Level level, CanvasScaler canvasScaler)
    {
        _level = level;
        _fingerLocations[1] = _level.LevelManager.MainCamera.WorldToScreenPoint(_battery1Location.position);
        _fingerLocations[3] = _level.LevelManager.MainCamera.WorldToScreenPoint(_inhibitor2Location.position);
        _fingerLocations[6] = _level.LevelManager.MainCamera.WorldToScreenPoint(_inhibitor1Location.position);
    }

    public override void OnCloseClicked()
    {
        _battery1Animator.SetTrigger("Reset");
        _inhibitor1Animator.SetTrigger("Reset");
        _inhibitor2Animator.SetTrigger("Reset");
        ResetTriggers();
        base.OnCloseClicked();
    }

    public override void OnSkipClicked()
    {
        _battery1Animator.SetTrigger("Reset");
        _inhibitor1Animator.SetTrigger("Reset");
        _inhibitor2Animator.SetTrigger("Reset");
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
        _singleColorBulbAnimator.SetTrigger("SetNormalImage1");
        _singleColorBulbAnimator.SetTrigger("RedOff");
        _singleColorBulbAnimator.SetTrigger("RedUnlitOn");
        _doubleColorBulbAnimator.SetTrigger("RedBlueOff");
        _doubleColorBulbAnimator.SetTrigger("SetNormalImage1");
        _doubleColorBulb.ResetPowerable();
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
                _nextButton.interactable = false;
                MoveFinger(5, _fingerTransform.localPosition, _fingerLocations[5], true);
                break;
            case 6:
                _nextButton.interactable = false;
                MoveFinger(6, _fingerTransform.position, _fingerLocations[6], false);
                break;
            case 7:
                _nextButton.interactable = false;
                MoveFinger(7, _fingerTransform.localPosition, _fingerLocations[7], true);
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
                _singleColorBulbAnimator.SetTrigger("RedOn");
                _singleColorBulbAnimator.SetTrigger("RedUnlitOff");
                _singleColorBulb.IncreasePower(1, true);
                _doubleColorBulbAnimator.SetTrigger("RedBlueOn");
                _doubleColorBulb.IncreasePower(1, true);
                _wire1Animators.ToList().ForEach(w => w.SetTrigger("AllOn"));
                _wire2Animators.ToList().ForEach(w => w.SetTrigger("AllOn"));
                _nextButton.interactable = true;
                break;
            case 3:
                _inhibitor2Animator.SetTrigger("HighlightOn");
                _batteryOptionsAnimator.SetTrigger("MoveDown");
                _inhibitorOptionsAnimator.SetTrigger("MoveUp");
                _nextButton.interactable = true;
                break;
            case 4:                
                _inhibitor2Animator.SetTrigger("RedOn");
                _singleColorBulbAnimator.SetTrigger("RedOff");
                _singleColorBulbAnimator.SetTrigger("RedUnlitOn");
                _wire1Animators[1].SetTrigger("GreenBlueOn");
                _singleColorBulb.DecreasePower(1);
                _nextButton.interactable = true;
                break;
            case 5:
                _inhibitor2Animator.SetTrigger("GreenBlueOn");
                _wire1Animators[1].SetTrigger("RedOn");
                _singleColorBulbAnimator.SetTrigger("RedOn");
                _singleColorBulbAnimator.SetTrigger("RedUnlitOff");
                _singleColorBulb.IncreasePower(1, true);
                _nextButton.interactable = true;
                break;
            case 6:
                _inhibitor1Animator.SetTrigger("HighlightOn");
                _nextButton.interactable = true;
                break;
            case 7:
                _inhibitor1Animator.SetTrigger("RedOn");
                _wire2Animators[1].SetTrigger("GreenBlueOn");
                _nextButton.gameObject.SetActive(false);
                _closeButton.gameObject.SetActive(true);
                _nextButton.interactable = false;
                break;
        }        
    }

    internal override void MoveFingerEnd()
    {
        switch (_animationIndex)
        {
            case 1:
                _fingerAnimator.SetTrigger("Lvl11_Finger1");
                break;
            case 2:
                _fingerAnimator.SetTrigger("Lvl11_Finger2");
                break;
            case 3:
                _fingerAnimator.SetTrigger("Lvl11_Finger3");
                break;
            case 4:
                _fingerAnimator.SetTrigger("Lvl11_Finger4");
                break;
            case 5:
                _fingerAnimator.SetTrigger("Lvl11_Finger5");
                break;
            case 6:
                _fingerAnimator.SetTrigger("Lvl11_Finger6");
                break;
            case 7:
                _fingerAnimator.SetTrigger("Lvl11_Finger7");
                break;
        }
    }

    private void ResetTriggers()
    {
        _battery1Animator.ResetTrigger("RedOn");
        _battery1Animator.ResetTrigger("RedOff");
        _battery1Animator.ResetTrigger("HighlightOn");
        _battery1Animator.ResetTrigger("HighlightOff");
        _batteryOptionsAnimator.ResetTrigger("MoveUp");
        _inhibitor1Animator.ResetTrigger("HighlightOn");
        _inhibitor2Animator.ResetTrigger("HighlightOn");        
        _battery1Animator.Rebind();
        _inhibitor1Animator.Rebind();
        _inhibitor2Animator.Rebind();        
    }
    #endregion Methods (end)
}