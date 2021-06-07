using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class TutorialResolverBase : MonoBehaviour
{
    #region Fields, Properties
    [SerializeField]
    internal Button _nextButton = null;

    [SerializeField]
    internal Button _skipButton = null;

    [SerializeField]
    internal Button _closeButton = null;

    [SerializeField]
    internal List<TextMeshProUGUI> _tutorialTexts;

    [SerializeField]
    internal RectTransform _fingerTransform = null;

    [SerializeField]
    internal Animator _fingerAnimator = null;

    [SerializeField]
    internal Vector2[] _fingerLocations = null;

    [SerializeField]
    internal int _speed = 0;

    [SerializeField]
    internal GameObject _tutorialObjectsParent = null;

    [SerializeField]
    internal GameObject _gameObjectsParent = null;

    [SerializeField]
    internal CanvasScaler _canvasScaler = null;

    internal int _animationIndex = 0;
    internal int _tutorialIndex = 0;
    internal Level _level;
    #endregion Fields, Properties (end)

    #region Methods
    public abstract void Setup(Level level, CanvasScaler canvasScaler);
    public abstract void InitializeTutorial(int index);
    public abstract void OnFingerAnimationEnd(int index);
    public abstract void TutorialAnimationEnd(int index);

    internal abstract void MoveFingerEnd();

    internal abstract void HandleTutorialStateByIndex(int index);
    
    virtual public void OnCloseClicked()
    {
        StartCoroutine(CloseCoroutine());
    }

    internal IEnumerator CloseCoroutine()
    {
        yield return new WaitForEndOfFrame();
        _tutorialObjectsParent.SetActive(false);
        _gameObjectsParent.SetActive(true);
        _level.OnTutorialResolverCloseClicked();
        yield return null;
    }
    virtual public void OnNextClicked()
    {
        _nextButton.interactable = false;
        _tutorialIndex++;
        SetTutorialTextState(_tutorialIndex);
        HandleTutorialStateByIndex(_tutorialIndex);
        //InitializeTutorial(_tutorialIndex);
        //Play animation at specific points.
        //_levelManger.TriggerAnimation(_levelNumber, _tutorialIndex);
        //How can we tie this all together genericly for all levels????
    }

    virtual public void OnSkipClicked()
    {
        StartCoroutine(SkipCoroutine());
    }

    internal IEnumerator SkipCoroutine()
    {
        yield return new WaitForEndOfFrame();
        _tutorialIndex = 0;
        //Stop any animations
        _tutorialObjectsParent.SetActive(false);
        _gameObjectsParent.SetActive(true);
        _level.OnTutorialResolvedSkipClosed();
        yield return null;
    }
    
    internal void SetTutorialTextState(int index)
    {
        _tutorialTexts.ForEach(t => t.gameObject.SetActive(false));
        if(index < _tutorialTexts.Count)
            _tutorialTexts[index].gameObject.SetActive(true);
    }

    internal int ValidateIndexValue(int index)
    {
        if (index < 0)
            index = 0;
        if (index > _tutorialTexts.Count() - 1)
            index = _tutorialTexts.Count() - 1;
        return index;
    }

    internal IEnumerator AnimateObject(RectTransform objectToMove, Vector3 startLocation, Vector3 endlocation, Action callback, bool useLocalPosition = false)
    {
        var reached = false;
        while (!reached)
        {
            startLocation = Vector2.Lerp(startLocation, endlocation, _speed * Time.deltaTime);
            if (useLocalPosition)
                objectToMove.localPosition = startLocation;
            else
                objectToMove.position = startLocation;
            reached = (Mathf.Round(startLocation.x) == Mathf.Round(endlocation.x)) && (Mathf.Round(startLocation.y) == Mathf.Round(endlocation.y));
            yield return reached;
        }

        callback?.Invoke();
    }

    internal void MoveFinger(int index, Vector3 startPosition, Vector3 endPosition, bool useLocalPosition)
    {
        _animationIndex = index;
        StartCoroutine(AnimateObject(_fingerTransform, startPosition, GetFinterPositionAdjustment(endPosition, useLocalPosition), MoveFingerEnd, useLocalPosition));
    }

    internal void ResetFingerLocation()
    {
        _fingerTransform.position = _fingerLocations[0];
    }

    internal Vector2 GetFinterPositionAdjustment(Vector2 originalPos, bool useLocalPosition)
    {
        if(useLocalPosition && _level.LevelManager.MainCamera.aspect > 1.3f && _level.LevelManager.MainCamera.aspect < 1.4f)
        {
            return new Vector2(originalPos.x, originalPos.y - 100);
        }
        return originalPos;
    }
    #endregion Methods (end)
}