using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InhibitorOptionsManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _optionPrefab = null;

    [SerializeField]
    private List<InhibitorOption> _options;

    [SerializeField]
    private Transform _optionParent = null;

    private Inhibitor _activeInhibitor = null;
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
    }

    
    public void Setup(Inhibitor inhibitor)
    {
        Reset();
        _activeInhibitor = inhibitor;
        gameObject.SetActive(true);
    }

    private void Option_OnClick(InhibitorOption option)
    {
        _activeInhibitor.SetUserSelectedPower(option.OptionColorTypes);
        Reset();
    }

    private void Reset()
    {
        gameObject.SetActive(false);
        _activeInhibitor = null;
    }
}
