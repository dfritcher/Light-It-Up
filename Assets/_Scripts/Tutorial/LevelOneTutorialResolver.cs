using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[Serializable]
public class LevelOneTutorialResolver : TutorialResolverBase
{

    #region Fields, Properties
    [SerializeField]
    private Transform _batteryLocation = null;

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
    
    public override void Setup(Level level, CanvasScaler canvasScaler)
    {
        _level = level;
        _fingerLocations[1] = _level.LevelManager.MainCamera.WorldToScreenPoint(_batteryLocation.position);        
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
        ResetFingerLocation();
        yield return new WaitForEndOfFrame();

        _tutorialIndex = index;
        _tutorialObjectsParent.SetActive(true);
        _gameObjectsParent.SetActive(false);
        _redBulb.ResetPowerable();
        _bulbAnimator.SetTrigger("RedOff");
        _bulbAnimator.SetTrigger("RedUnlitOn");
        _bulbAnimator.SetTrigger("SetNormalImage1");
        _batteryRedSection.SetActive(false);
        index = ValidateIndexValue(index);
        yield return null;
        SetTutorialTextState(index);
        HandleTutorialStateByIndex(index);        
    }

    public override void OnFingerAnimationEnd(int index)
    {
        switch (index)
        {
            case 1:
                _batteryAnimator.SetTrigger("HighlightOn");
                _batteryOptionsAnimator.SetTrigger("MoveUp");
                _nextButton.interactable = true;
                break;
            case 2:
                _batteryAnimator.SetTrigger("RedOn");
                _bulbAnimator.SetTrigger("RedUnlitOff");
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
        _level.TutorialAnimationEnd(index);
    }

    internal override void HandleTutorialStateByIndex(int index)
    {
        switch (index)
        {
            case 0:                
            case 1:
            case 2:
                ResetFingerLocation();
                _nextButton.gameObject.SetActive(true);
                _closeButton.gameObject.SetActive(false);
                _nextButton.interactable = true;
                break;
            case 3:
                MoveFinger(1, _fingerTransform.position, _fingerLocations[1], false);
                _nextButton.interactable = false;
                break;
            case 4:
                _nextButton.interactable = true;
                break;
            case 5:                
                _nextButton.interactable = true;
                break;
            case 6:
                MoveFinger(2, _fingerTransform.localPosition, _fingerLocations[2],  true);
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
                _fingerAnimator.SetTrigger("Lvl1_Finger1");
                break;
            case 2:
                _fingerAnimator.SetTrigger("Lvl1_Finger2");
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
        _animationIndex = 0;
    }

    #endregion Methods (end)
}