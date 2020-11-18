using System;
using UnityEngine;
using System.Collections;

[Serializable]
public class LevelOneTutorialResolver : TutorialResolverBase
{

    #region Fields, Properties
    [SerializeField]
    private Animator _batteryAnimator = null;

    [SerializeField]
    private Animator _wireAnimator = null;

    [SerializeField]
    private Animator _bulbAnimator = null;

    [SerializeField]
    private Animator _batteryOptionsAnimator = null;

    [SerializeField]
    private Bulb _redBulb = null;

    [SerializeField]
    private GameObject _batteryRedSection = null;
    #endregion Fields, Properties (end)

    #region Methods
    public override void Setup(Level level)
    {
        _level = level;                
    }

    public override void OnCloseClicked()
    {
        _batteryAnimator.SetTrigger("Reset");
        ResetTriggers();
        base.OnCloseClicked();               
    }

    public override void OnSkipClicked()
    {
        _batteryAnimator.SetTrigger("Reset");
        ResetTriggers();
        base.OnSkipClicked();
    }
    public override void InitializeTutorial(int index)
    {
        StartCoroutine(InitialezeTutorialCoroutine(index));
    }

    private IEnumerator InitialezeTutorialCoroutine(int index)
    {
        _batteryAnimator.SetTrigger("Reset");
        ResetTriggers();
        yield return new WaitForEndOfFrame();

        _tutorialIndex = index;
        _tutorialObjectsParent.SetActive(true);
        _gameObjectsParent.SetActive(false);
        _redBulb.ResetPowerable();
        _bulbAnimator.SetTrigger("RedOff");
        _bulbAnimator.SetTrigger("SetNormalImage1");
        _batteryRedSection.SetActive(false);
        index = ValidateIndexValue(index);
        yield return null;
        SetTutorialTextState(index);
        HandleTutorialStateByIndex(index);
        
    }

    public override void OnFingerAnimationEnd(int index)
    {
        //Debug.Log($"OnFinger INDEX: {index}");
        switch (index)
        {
            case 1:
                //_batteryAnimator.SetTrigger("RedOff");
                _batteryAnimator.SetTrigger("HighlightOn");
                _batteryOptionsAnimator.SetTrigger("MoveUp");
                _nextButton.interactable = true;
                break;
            case 2:
                //_batteryAnimator.SetTrigger("HighlightOff");
                _batteryAnimator.SetTrigger("RedOn");
                _bulbAnimator.SetTrigger("RedOn");
                _wireAnimator.SetTrigger("RedOn");
                _redBulb.IncreasePower(1, true);
                _nextButton.interactable = true;
                break;
            case 6:
                _closeButton.gameObject.SetActive(true);
                break;
        }
    }

    public override void TutorialAnimationEnd(int index)
    {
        //_nextButton.interactable = true;
        //_tutorialIndex++;
        //InitializeTutorial(_tutorialIndex);
        //_levelManger.TriggerAnimation(_levelNumber, _tutorialIndex);
        _level.TutorialAnimationEnd(index);
    }

    internal override void HandleTutorialStateByIndex(int index)
    {
        Debug.Log($"Handle INDEX: {index}");
        switch (index)
        {
            case 0:
            case 1:
            case 2:
                _nextButton.gameObject.SetActive(true);
                _closeButton.gameObject.SetActive(false);
                _nextButton.interactable = true;
                break;
            case 3:
                _fingerAnimator.SetTrigger("Lvl1_Finger1");
                _nextButton.interactable = false;
                break;
            case 4:
                _nextButton.interactable = true;
                break;
            case 5:                
                _nextButton.interactable = true;
                break;
            case 6:
                _fingerAnimator.SetTrigger("Lvl1_Finger2");
                _nextButton.gameObject.SetActive(false);
                _closeButton.gameObject.SetActive(true);
                break;           
            default:
                _nextButton.interactable = true;
                break;
        }
    }

    private void ResetTriggers()
    {
        _batteryAnimator.ResetTrigger("RedOn");
        _batteryAnimator.ResetTrigger("RedOff");
        _batteryAnimator.ResetTrigger("HighlightOn");
        _batteryAnimator.ResetTrigger("HighlightOff");
        _batteryOptionsAnimator.ResetTrigger("MoveUp");
        _bulbAnimator.ResetTrigger("RedOn");
        _bulbAnimator.SetTrigger("RedOff");
        _wireAnimator.ResetTrigger("RedOn");
        _batteryAnimator.Rebind();
    }
    #endregion Methods (end)
}
