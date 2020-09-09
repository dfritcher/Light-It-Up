using System;
using System.Linq;
using UnityEngine;

public class LevelElevenTutorialResolver : TutorialResolverBase
{
    #region Fields, Properties
    [SerializeField]
    private Animator _battery1Animator = null;
    
    [SerializeField]
    private Animator _inhibitor1Animator = null;

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
    public override void Setup(Level level)
    {
        _level = level;
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
        Debug.Log($"Handle Tutorial Index :{index}");
        switch (index)
        {
            case 0:
                _nextButton.interactable = true;
                break;
            case 1:
                _nextButton.interactable = false;
                _fingerAnimator.SetTrigger("Lvl11_Finger1");
                break;
            case 2:
                _nextButton.interactable = false;
                _fingerAnimator.SetTrigger("Lvl11_Finger2");                
                break;
            case 3:                
                _nextButton.interactable = false;
                _fingerAnimator.SetTrigger("Lvl11_Finger3");
                break;
            case 4:                
                _nextButton.interactable = false;
                _fingerAnimator.SetTrigger("Lvl11_Finger4");
                break;
            case 5:
                _nextButton.interactable = false;
                _fingerAnimator.SetTrigger("Lvl11_Finger5");
                break;
            case 6:
                _nextButton.interactable = false;
                _fingerAnimator.SetTrigger("Lvl11_Finger6");
                break;
            case 7:
                _nextButton.interactable = false;
                _fingerAnimator.SetTrigger("Lvl11_Finger7");
                break;
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
                _singleColorBulbAnimator.SetTrigger("RedOn");
                _singleColorBulb.IncreasePower(1, true);
                _doubleColorBulbAnimator.SetTrigger("RedBlueOn");
                _doubleColorBulb.IncreasePower(1, true);
                _wire1Animators.ToList().ForEach(w => w.SetTrigger("AllOn"));
                _wire2Animators.ToList().ForEach(w => w.SetTrigger("AllOn"));
                _nextButton.interactable = true;
                break;
            case 3:
                _inhibitor1Animator.SetTrigger("HighlightOn");
                _batteryOptionsAnimator.SetTrigger("MoveDown");
                _inhibitorOptionsAnimator.SetTrigger("MoveUp");
                _nextButton.interactable = true;
                break;
            case 4:                
                _inhibitor1Animator.SetTrigger("RedOn");
                _singleColorBulbAnimator.SetTrigger("RedOff");
                _wire1Animators[1].SetTrigger("GreenBlueOn");
                _singleColorBulb.DecreasePower(1);
                _nextButton.interactable = true;
                break;
            case 5:
                _inhibitor1Animator.SetTrigger("GreenBlueOn");
                _wire1Animators[1].SetTrigger("RedOn");
                _singleColorBulbAnimator.SetTrigger("RedOn");
                _singleColorBulb.IncreasePower(1, true);
                _nextButton.interactable = true;
                break;
            case 6:
                _inhibitor2Animator.SetTrigger("HighlightOn");
                _nextButton.interactable = true;
                break;
            case 7:
                _inhibitor2Animator.SetTrigger("RedOn");
                _wire2Animators[1].SetTrigger("GreenBlueOn");
                _nextButton.gameObject.SetActive(false);
                _closeButton.gameObject.SetActive(true);
                _nextButton.interactable = false;
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