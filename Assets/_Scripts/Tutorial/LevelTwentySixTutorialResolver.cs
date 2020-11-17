using System;
using System.Linq;
using UnityEngine;

public class LevelTwentySixTutorialResolver : TutorialResolverBase
{
    #region Fields, Properties
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
    public override void Setup(Level level)
    {
        _level = level;
    }

    public override void OnCloseClicked()
    {
        _battery1Animator.SetTrigger("Reset");
        
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
        _singlePowerBulbAnimator.SetTrigger("SetNormalImage1");
        _doublePowerBulb.ResetPowerable();
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
        Debug.Log($"Handle Tutorial Index :{index}");
        switch (index)
        {
            case 0:
                _nextButton.interactable = true;
                break;
            case 1:
                _nextButton.interactable = false;
                _fingerAnimator.SetTrigger("Lvl26_Finger1");
                break;
            case 2:
                _nextButton.interactable = false;
                _fingerAnimator.SetTrigger("Lvl26_Finger2");                
                break;
            case 3:                
                _nextButton.interactable = false;
                _fingerAnimator.SetTrigger("Lvl26_Finger3");
                break;
            //case 4:                
            //    _nextButton.interactable = false;
            //    _fingerAnimator.SetTrigger("Lvl26_Finger4");
            //    break;
            //case 5:
            //    _nextButton.interactable = false;
            //    _fingerAnimator.SetTrigger("Lvl26_Finger5");
            //    break;
            //case 6:
            //    _nextButton.interactable = false;
            //    _fingerAnimator.SetTrigger("Lvl26_Finger6");
            //    break;
            //case 7:
            //    _nextButton.interactable = false;
            //    _fingerAnimator.SetTrigger("Lvl26_Finger7");
            //    break;
            default:
                _nextButton.interactable = true;
                break;
        }
    }

    public override void OnFingerAnimationEnd(int index)
    {
        Debug.Log($"OnFingerAnimationEnd Index: {index}");
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

    private void ResetTriggers()
    {
        _battery1Animator.ResetTrigger("RedOn");
        _battery1Animator.ResetTrigger("RedOff");
        _battery1Animator.ResetTrigger("HighlightOn");
        _battery1Animator.ResetTrigger("HighlightOff");
        _batteryOptionsAnimator.ResetTrigger("MoveUp");        
    }
    #endregion Methods (end)
}