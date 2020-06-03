using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassThroughOptionsManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _optionPrefab = null;

    [SerializeField]
    private List<PassThroughOption> _options;

    [SerializeField]
    private Transform _optionParent = null;

    private PassThrough _activePassThrough = null;
    private void Awake()
    {
        var resetOption = Instantiate(_optionPrefab, _optionParent, false).GetComponent<PassThroughOption>();
        resetOption.Setup(new List<ColorType>() { ColorType.None });
        resetOption.OnClick += Option_OnClick;

        var redOption = Instantiate(_optionPrefab, _optionParent, false).GetComponent<PassThroughOption>();
        redOption.Setup(new List<ColorType>() { ColorType.Red });
        redOption.OnClick += Option_OnClick;

        var blueOption = Instantiate(_optionPrefab, _optionParent, false).GetComponent<PassThroughOption>();
        blueOption.Setup(new List<ColorType>() { ColorType.Blue });
        blueOption.OnClick += Option_OnClick;

        var greenOption = Instantiate(_optionPrefab, _optionParent, false).GetComponent<PassThroughOption>();
        greenOption.Setup(new List<ColorType>() { ColorType.Green });
        greenOption.OnClick += Option_OnClick;

        var redGreenOption = Instantiate(_optionPrefab, _optionParent, false).GetComponent<PassThroughOption>();
        redGreenOption.Setup(new List<ColorType>() { ColorType.Red, ColorType.Green });
        redGreenOption.OnClick += Option_OnClick;

        var blueGreenOption = Instantiate(_optionPrefab, _optionParent, false).GetComponent<PassThroughOption>();
        blueGreenOption.Setup(new List<ColorType>() { ColorType.Green, ColorType.Blue });
        blueGreenOption.OnClick += Option_OnClick;

        var redBlueOption = Instantiate(_optionPrefab, _optionParent, false).GetComponent<PassThroughOption>();
        redBlueOption.Setup(new List<ColorType>() { ColorType.Red, ColorType.Blue });
        redBlueOption.OnClick += Option_OnClick;        
    }

    
    public void Setup(PassThrough passThrough)
    {
        Reset();
        _activePassThrough = passThrough;
        gameObject.SetActive(true);
    }

    private void Option_OnClick(PassThroughOption option)
    {
        _activePassThrough.SetUserSelectedPower(option.OptionColorTypes);
        Reset();
    }

    private void Reset()
    {
        gameObject.SetActive(false);
        _activePassThrough = null;
    }
}
