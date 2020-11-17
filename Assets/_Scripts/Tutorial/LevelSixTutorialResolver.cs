using System;
using System.Linq;
using UnityEngine;

public class LevelSixTutorialResolver : TutorialResolverBase
{
    #region Fields, Properties
    [SerializeField]
    private Animator _battery1Animator = null;
    
    [SerializeField]
    private Animator _battery2Animator = null;

    [SerializeField]
    private Animator[] _wireAnimators = null;

    /// <summary>
    /// Reference to the wire connected to the second battery.
    /// </summary>
    [SerializeField]
    private Animator _battery2WireAnimator = null;

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

    #endregion Fields, Properties (end)

    #region Methods
    public override void Setup(Level level)
    {
        _level = level;
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
        _doubleColorBulb.ResetPowerable();
        _doubleColorBulbAnimator.SetTrigger("SetNormalImage1");
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
                _fingerAnimator.SetTrigger("Lvl6_Finger1");
                break;
            case 2:
                _nextButton.interactable = false;
                _fingerAnimator.SetTrigger("Lvl6_Finger2");                
                break;
            case 3:                
                _nextButton.interactable = false;
                _fingerAnimator.SetTrigger("Lvl6_Finger3");
                break;
            case 4:                
                _nextButton.interactable = false;
                _fingerAnimator.SetTrigger("Lvl6_Finger4");                
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
            case 1:
                _battery1Animator.SetTrigger("HighlightOn");
                _batteryOptionsAnimator.SetTrigger("MoveUp");
                _nextButton.interactable = true;
                break;
            case 2:
                _battery1Animator.SetTrigger("HighlightOff");
                _battery1Animator.SetTrigger("RedOn");
                _singleColorBulbAnimator.SetTrigger("RedOn");
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
                _battery2WireAnimator.SetTrigger("BlueOn");
                _nextButton.gameObject.SetActive(false);
                _closeButton.gameObject.SetActive(true);
                break;            
        }
    }

    private void ResetTriggers()
    {
        _battery1Animator.ResetTrigger("RedOn");
        _battery1Animator.ResetTrigger("RedOff");
        _battery1Animator.ResetTrigger("HighlightOn");
        _battery1Animator.ResetTrigger("HighlightOff");
        _battery2Animator.ResetTrigger("RedOn");
        _battery2Animator.ResetTrigger("RedOff");
        _battery2Animator.ResetTrigger("HighlightOn");
        _battery2Animator.ResetTrigger("HighlightOff");
        _batteryOptionsAnimator.ResetTrigger("MoveUp");
        _doubleColorBulbAnimator.ResetTrigger("SetBrokenImage");
        _doubleColorBulbAnimator.ResetTrigger("RedBlueOn");
        _battery1Animator.Rebind();
        _battery2Animator.Rebind();
    }
    #endregion Methods (end)
}
