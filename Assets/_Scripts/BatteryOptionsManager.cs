using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryOptionsManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _batteryOptionPrefab = null;

    [SerializeField]
    private List<BatteryOption> _batteryOptions;

    [SerializeField]
    private Transform _batteryOptionParent = null;

    [SerializeField]
    private Vector2 _startPosition;

    [SerializeField]
    private Vector2 _endPosition;

    [SerializeField]
    private float _animationTime = 1f;

    private Battery _activeBattery = null;

    private bool _extended = false;
    private void Awake()
    {
        var resetOption = Instantiate(_batteryOptionPrefab, _batteryOptionParent, false).GetComponent<BatteryOption>();
        resetOption.Setup(new List<ColorType>() { ColorType.None });
        resetOption.OnClick += BatteryOption_OnClick;

        var redOption = Instantiate(_batteryOptionPrefab, _batteryOptionParent, false).GetComponent<BatteryOption>();
        redOption.Setup(new List<ColorType>() { ColorType.Red });
        redOption.OnClick += BatteryOption_OnClick;

        var blueOption = Instantiate(_batteryOptionPrefab, _batteryOptionParent, false).GetComponent<BatteryOption>();
        blueOption.Setup(new List<ColorType>() { ColorType.Blue });
        blueOption.OnClick += BatteryOption_OnClick;

        var greenOption = Instantiate(_batteryOptionPrefab, _batteryOptionParent, false).GetComponent<BatteryOption>();
        greenOption.Setup(new List<ColorType>() { ColorType.Green });
        greenOption.OnClick += BatteryOption_OnClick;

        var redGreenOption = Instantiate(_batteryOptionPrefab, _batteryOptionParent, false).GetComponent<BatteryOption>();
        redGreenOption.Setup(new List<ColorType>() { ColorType.Red, ColorType.Green });
        redGreenOption.OnClick += BatteryOption_OnClick;

        var blueGreenOption = Instantiate(_batteryOptionPrefab, _batteryOptionParent, false).GetComponent<BatteryOption>();
        blueGreenOption.Setup(new List<ColorType>() { ColorType.Green, ColorType.Blue });
        blueGreenOption.OnClick += BatteryOption_OnClick;

        var redBlueOption = Instantiate(_batteryOptionPrefab, _batteryOptionParent, false).GetComponent<BatteryOption>();
        redBlueOption.Setup(new List<ColorType>() { ColorType.Red, ColorType.Blue });
        redBlueOption.OnClick += BatteryOption_OnClick;

        var redBlueGreenOption = Instantiate(_batteryOptionPrefab, _batteryOptionParent, false).GetComponent<BatteryOption>();
        redBlueGreenOption.Setup(new List<ColorType>() { ColorType.Red, ColorType.Green, ColorType.Blue });
        redBlueGreenOption.OnClick += BatteryOption_OnClick;

        gameObject.SetActive(false);
        ((RectTransform)transform).anchoredPosition = _startPosition;
        _extended = false;
    }

    public void Initialize()
    {
        gameObject.SetActive(true);
    }

    public void Setup(Battery battery)
    {
        _activeBattery = battery;
        if (!_extended)
        {            
            gameObject.SetActive(true);
            AnimationController.Instance.AnimateDirection(_startPosition, _endPosition, 0f, _animationTime, (RectTransform)transform, SetActive);
        }
        else
        {
            AnimationController.Instance.StopAllCoroutines();
            ((RectTransform)transform).anchoredPosition = _endPosition;
        }        
    }

    private void BatteryOption_OnClick(BatteryOption batteryOption)
    {
        _activeBattery.SetBatteryTypes(batteryOption.BatteryOptionColorTypes);
        AnimationController.Instance.StopAllCoroutines();
        AnimateOptionPanel();
    }

    public void ResetOptions()
    {
        ((RectTransform)transform).anchoredPosition = _startPosition;
        SetInActive();
    }

    public void AnimateOptionPanel(bool immediate = false)
    {
        if (!_extended)
            return;
        //gameObject.SetActive(false);
        if(immediate)
            AnimationController.Instance.AnimateDirection(_endPosition, _startPosition, 0f, _animationTime / 2, (RectTransform)transform, SetInActive);
        else
            AnimationController.Instance.AnimateDirection(_endPosition, _startPosition, 3f, _animationTime /2, (RectTransform)transform, SetInActive);
        //_activeBattery = null;
        
    }

    public void SetActive()
    {
        _extended = true;
    }

    public void SetInActive()
    {
        _extended = false;
        _activeBattery?.SetSelectedState(false);
    }
}
