using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Bulb : PowerableBase
{
    [SerializeField]
    private List<ColorType> _currentColorTypes = null;
    public override List<ColorType> CurrentColorTypes { get { return _currentColorTypes ?? (_currentColorTypes = new List<ColorType>()); } }

    [SerializeField]
    private List<Image> _bulbColors = null;

    [SerializeField]
    private TextMeshProUGUI _bulbPowerDisplay = null;

    [SerializeField]
    private TextMeshProUGUI _bulbStatusDisplay = null;

    [SerializeField]
    private int _minPower = 0;

    [SerializeField]
    private int _maxPower = 4;

    [SerializeField, Range(0, 4)]
    private int _powerLevel = 0;
    public int CurrentPowerLevel { get { return _powerLevel;  } }

    [SerializeField]
    private bool _isBroken = false;
    public bool IsBroken { get { return _isBroken; } }

    [SerializeField]
    private bool _isOn = false;
    public bool IsOn { get { return _isOn; } }

    public override bool IsClickable => false;

    public override bool IsPowered { get { return false; } }

    [SerializeField]
    private List<PowerableBase> _powerables = null;

    private string _objectname = string.Empty;

    protected override void Awake()
    {
        base.Awake();
        UpdateColorDisplay();
        _objectname = gameObject.name;
    }
    public void Setup()
    {
        UpdateUI();
    }
    
    public void IncreasePower()
    {
        _powerLevel++;        
    }

    public void DecreasePower()
    {
        _powerLevel--;
        if (_powerLevel < _minPower)
            _powerLevel = _minPower;

        UpdatePowerDisplay();
    }

    private void ResetPowerLevel()
    {
        _powerLevel = 0;
    }
   
    private void SetCurrentPower()
    {
        foreach(var powerable in _powerables)
        {
            var currentPower = powerable.GetPowers(this);
            foreach(var power in currentPower)
            {
                if (CurrentColorTypes.Contains(power))
                {
                    IncreasePower(); //TODO: This will need to change to grabbing the power level plus color coming from the source.

                    break;
                }                    
            }
        }
        CheckState();
    }

    private void CheckState()
    {
        //Once we are broken we don't get fixed.
        if (_isBroken)
            return;
        _isBroken = _powerLevel > _maxPower;

        _isOn = _powerLevel == _maxPower && !_isBroken;
    }

    private void UpdateUI()
    {
        UpdatePowerDisplay();
        UpdateColorDisplay();
    }

    private void UpdatePowerDisplay()
    {
        if (_powerLevel > _maxPower)
            _bulbStatusDisplay.text = $"BROKEN!";
        else
            _bulbStatusDisplay.text = string.Empty;
        _bulbPowerDisplay.text = $"{_powerLevel} / {_maxPower} ";
    }

    private void UpdateColorDisplay()
    {
        _bulbColors[0].gameObject.SetActive(CurrentColorTypes.Contains(ColorType.Red));
        _bulbColors[1].gameObject.SetActive(CurrentColorTypes.Contains(ColorType.Green));
        _bulbColors[2].gameObject.SetActive(CurrentColorTypes.Contains(ColorType.Blue));        
    }

    public override List<ColorType> GetPowers(PowerableBase requestor)
    {
        return new List<ColorType>() { ColorType.None };
    }

    public override void ResetPowerable()
    {
        _currentColorTypes = new List<ColorType>(_originalColorTypes);
        _isBroken = false;
        _isOn = false;
        ResetPowerLevel();
        UpdateUI();        
    }

    public override void UpdatePowerState(PowerableBase powerableBase)
    {
        ResetPowerable();
        SetCurrentPower();
        UpdateUI();
        //Bulbs don't update anyone.
    }

    public override void CascadeReset(PowerableBase powerableBase)
    {
        ResetPowerable();
    }
    public override bool GetPoweredState(PowerableBase requestor)
    {
        return _powerLevel > 0;
    }
}
