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

    private Battery _activeBattery = null;
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
    }

    
    public void Setup(Battery battery)
    {
        _activeBattery = battery;        
        gameObject.SetActive(true);
    }

    private void BatteryOption_OnClick(BatteryOption batteryOption)
    {
        _activeBattery.SetBatteryTypes(batteryOption.BatteryOptionColorTypes);
        Reset();
    }

    private void Reset()
    {
        gameObject.SetActive(false);
        _activeBattery = null;
    }
}
