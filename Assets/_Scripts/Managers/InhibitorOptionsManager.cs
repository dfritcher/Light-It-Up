using System.Collections.Generic;
using UnityEngine;

public class InhibitorOptionsManager : MonoBehaviour
{
    #region Fields
    [SerializeField]
    private GameObject _optionPrefab = null;

    [SerializeField]
    private List<InhibitorOption> _options;

    [SerializeField]
    private Transform _optionParent = null;

    [SerializeField]
    private Vector2 _startPosition;

    [SerializeField]
    private Vector2 _endPosition;

    [SerializeField]
    private float _animationTime = 1f;

    private Inhibitor _activeInhibitor = null;
    
    private bool _extended = false;
    #endregion Fields (end)

    #region Methods
    private void Awake()
    {
        var resetOption = Instantiate(_optionPrefab, _optionParent, false).GetComponent<InhibitorOption>();
        resetOption.Setup(new List<ColorType>() { ColorType.None });
        resetOption.OnClick += Option_OnClick;

        var redOption = Instantiate(_optionPrefab, _optionParent, false).GetComponent<InhibitorOption>();
        redOption.Setup(new List<ColorType>() { ColorType.Red });
        redOption.OnClick += Option_OnClick;

        var blueOption = Instantiate(_optionPrefab, _optionParent, false).GetComponent<InhibitorOption>();
        blueOption.Setup(new List<ColorType>() { ColorType.Blue });
        blueOption.OnClick += Option_OnClick;

        var greenOption = Instantiate(_optionPrefab, _optionParent, false).GetComponent<InhibitorOption>();
        greenOption.Setup(new List<ColorType>() { ColorType.Green });
        greenOption.OnClick += Option_OnClick;

        var redGreenOption = Instantiate(_optionPrefab, _optionParent, false).GetComponent<InhibitorOption>();
        redGreenOption.Setup(new List<ColorType>() { ColorType.Red, ColorType.Green });
        redGreenOption.OnClick += Option_OnClick;

        var blueGreenOption = Instantiate(_optionPrefab, _optionParent, false).GetComponent<InhibitorOption>();
        blueGreenOption.Setup(new List<ColorType>() { ColorType.Green, ColorType.Blue });
        blueGreenOption.OnClick += Option_OnClick;

        var redBlueOption = Instantiate(_optionPrefab, _optionParent, false).GetComponent<InhibitorOption>();
        redBlueOption.Setup(new List<ColorType>() { ColorType.Red, ColorType.Blue });
        redBlueOption.OnClick += Option_OnClick;

        gameObject.SetActive(false);
        ((RectTransform)transform).anchoredPosition = _startPosition;
    }

    public void Setup(Inhibitor inhibitor)
    {
        _activeInhibitor = inhibitor;
        if (!_extended)
        {
            gameObject.SetActive(true);
            AnimationController.Instance.AnimateDirection(_startPosition, _endPosition, 0f, _animationTime, (RectTransform)transform, null);
        }
        else
        {
            AnimationController.Instance.StopAllCoroutines();
            ((RectTransform)transform).anchoredPosition = _endPosition;
        }
        _extended = true;
        
    }

    private void Option_OnClick(InhibitorOption option)
    {
        _activeInhibitor.SetUserSelectedPower(option.OptionColorTypes);
        AnimationController.Instance.StopAllCoroutines();
        AnimateOptionPanel();
    }

    public void ResetOptions()
    {
        ((RectTransform)transform).anchoredPosition = _startPosition;
        SetInactive();
    }

    public void AnimateOptionPanel(bool immediate = false)
    {
        if (!_extended)
            return;
        if (immediate)
            AnimationController.Instance.AnimateDirection(_endPosition, _startPosition, 0f, _animationTime / 2, (RectTransform)transform, SetInactive);
        else
            AnimationController.Instance.AnimateDirection(_endPosition, _startPosition, 3f, _animationTime / 2, (RectTransform)transform, SetInactive);       
    }

    public void SetActive()
    {
        _extended = true;
    }

    public void SetInactive()
    {
        gameObject.SetActive(false);
        _extended = false;
        _activeInhibitor?.SetSelectedState(false);
    }
    #endregion Methods (end)
}