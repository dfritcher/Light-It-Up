using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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
    internal Animator _fingerAnimator = null;

    [SerializeField]
    internal GameObject _tutorialObjectsParent = null;

    [SerializeField]
    internal GameObject _gameObjectsParent = null;

    internal int _tutorialIndex = 0;
    internal Level _level;
    #endregion Fields, Properties (end)

    #region Methods
    public abstract void Setup(Level level);
    public abstract void InitializeTutorial(int index);
    public abstract void OnFingerAnimationEnd(int index);
    public abstract void TutorialAnimationEnd(int index);

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
    #endregion Methods (end)
}