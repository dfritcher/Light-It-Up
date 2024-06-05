using System;
using System.Collections.Generic;
using UnityEngine;

public class Wire : PowerableBase
{
    #region Fields, Properties
    
    private List<ColorType> _currentColorTypes = null;
    public List<ColorType> CurrentColorTypes { get { return _currentColorTypes ?? (_currentColorTypes = new List<ColorType>()); } }

    public override bool IsClickable => false;

    [SerializeField]
    private bool _isPowered = false;
    public override bool IsPowered { get { return _isPowered; } }

    [SerializeField]
    private GameObject _red = null;
    [SerializeField]
    private GameObject _green = null;
    [SerializeField]
    private GameObject _blue = null;
    #endregion Fields, Properties (end)

    #region Methods
    protected override void Awake()
    {
        base.Awake();        
    }

    public void Setup()
    {
        UpdateColorDisplay();
    }

    public override void Setup(PowerableBase powerableBase)
    {
        
    }

    private void UpdateColorDisplay()
    {
        _red.SetActive(CurrentColorTypes.Contains(ColorType.Red));
        _green.SetActive(CurrentColorTypes.Contains(ColorType.Green));
        _blue.SetActive(CurrentColorTypes.Contains(ColorType.Blue));        
    }

    public override List<Power> GetPowers(PowerableBase requestor)
    {
        throw new NotImplementedException();
    }

    public override void ResetPowerable()
    {
        _currentColorTypes = new List<ColorType>(_originalColorTypes);
        UpdateColorDisplay();
    }

    public override void DetermineNewPowerState(PowerableBase powerableBase)
    {
        ResetPowerable();
        SetCurrentPower();
        CheckPoweredState();
        UpdateColorDisplay();
    }

    private void SetCurrentPower()
    {
        _currentColorTypes = _originalColorTypes.Clone();
        foreach (var externalSource in _externalPowerSources)
        {
            if (!externalSource.Powerable.IsPoweredFromOtherSide(this))
                continue;
            if (externalSource.Powerable is Battery battery)
            {
                foreach (var color in battery.CurrentPower.ColorTypes)
                {
                    if (!CurrentColorTypes.Contains(color))
                    {
                        CurrentColorTypes.Add(color);                         
                    }
                }
            }
            else
            {
                foreach (var poweredColor in externalSource.Powerable.PoweredColors)
                {
                    foreach (var color in poweredColor.ColorTypes)
                    {
                        if (!CurrentColorTypes.Contains(color))
                        {
                            CurrentColorTypes.Add(color);
                        }
                    }
                }
            }
        }        
    }

    private void CheckPoweredState()
    {
        var isPowered = false;
        foreach (var source in _externalPowerSources)
        {
            if (isPowered)
                break;
            isPowered = source.Powerable.IsPowered;
        }
        _isPowered = isPowered;
    }

    public override bool GetPoweredState(PowerableBase requestor)
    {
        return _currentColorTypes.Count > 0;
    }

    public override void SetPowerStateOff(PowerableBase requestor)
    {
        //Do Nothing
    }    

    public override void CheckStateChanged(PowerableBase powerableSource, bool forceCheck)
    {
        ResetPowerable();
        SetCurrentPower();
        CheckPoweredState();
        UpdateColorDisplay();
    }

    public override void OnMouseDown()
    {

    }
    #endregion Methods (end)
}