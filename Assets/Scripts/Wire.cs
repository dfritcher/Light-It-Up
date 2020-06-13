using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Wire : PowerableBase
{
    [SerializeField]
    private List<ColorType> _currentColorTypes = null;
    public override List<ColorType> CurrentColorTypes { get { return _currentColorTypes ?? (_currentColorTypes = new List<ColorType>()); } }

    public override bool IsClickable => false;

    [SerializeField]
    private bool _isPowered = false;
    public override bool IsPowered { get { return _isPowered; } }
        
    [SerializeField]
    private List<Image> _wireColors = null;

    [SerializeField]
    private List<PowerableBase> _powerables = null;

    //[SerializeField]
    //private List<PowerSource> _powerSources = null;
    protected override void Awake()
    {
        base.Awake();
        UpdateColorDisplay();
    }

    public void Setup()
    {
        //if(colors != null)
        //    _currentColorTypes = colors;        
    }

    //private void GetCurrentPower()
    //{
    //    foreach (var powerable in _powerables)
    //    {
    //        var currentPower = powerable.GetPowers(null);
    //        foreach (var power in currentPower)
    //        {
    //            if (CurrentColorTypes.Contains(power))
    //            {
    //                _wireColors[GetIndexFromPower(power)].gameObject.SetActive(true);
    //            }
    //        }
    //    }

    //    _wireColors[0].gameObject.SetActive(!_wireColors[1].IsActive() && !_wireColors[2].IsActive() && !_wireColors[3].IsActive());
    //}

    //private int GetIndexFromPower(ColorType power)
    //{
    //    switch (power)
    //    {
    //        case ColorType.None:
    //            return 0;
    //        case ColorType.Red:
    //            return 1;
    //        case ColorType.Green:
    //            return 2;            
    //        case ColorType.Blue:
    //            return 3;
    //        default:
    //            return 0;
    //    }
    //}

    private void UpdateColorDisplay()
    {       
        _wireColors[1].gameObject.SetActive(CurrentColorTypes.Contains(ColorType.Red));
        _wireColors[2].gameObject.SetActive(CurrentColorTypes.Contains(ColorType.Green));
        _wireColors[3].gameObject.SetActive(CurrentColorTypes.Contains(ColorType.Blue));
        _wireColors[0].gameObject.SetActive(!_wireColors[1].IsActive() && !_wireColors[2].IsActive() && !_wireColors[3].IsActive());
    }

    public override List<Power> GetPowers(PowerableBase requestor)
    {
        return new List<Power>() { _power };
    }

    public override void ResetPowerable()
    {
        _currentColorTypes = new List<ColorType>(_originalColorTypes);
        UpdateColorDisplay();
    }

    public override void UpdatePowerState(PowerableBase powerableBase)
    {
        ResetPowerable();
        SetCurrentPower();
        CheckPoweredState();
        UpdateColorDisplay();

        //Notify everyone we updated
        //foreach (var powerable in _powerables)
        //{
        //    if (powerable == powerableBase)
        //        continue;
        //    powerable.UpdatePowerState(this);
        //}
    }

    private void SetCurrentPower()
    {
        _currentColorTypes.Clear();
        foreach(var ps in _powerables)
        {
            _currentColorTypes.AddRange(ps.CurrentColorTypes);
        }

        ////Figure out our current power/colors
        //foreach (var powerable in _powerables)
        //{
        //    if (powerable.IsPowered)
        //    {
        //        var colorsToAdd = powerable.GetPowers(this);
        //        colorsToAdd.Remove(ColorType.None);
        //        colors.AddRange(colorsToAdd);
        //    }           
        //}
        //colors.Distinct().ToList();
        //if (colors.Count > 0)
        //    _currentColorTypes = colors;
        //else
        //{
        //    _currentColorTypes = _originalColorTypes;
        //}
    }

    private void CheckPoweredState()
    {
        var isPowered = false;
        foreach (var source in _powerables)
        {
            if (isPowered)
                break;
            isPowered = source.IsPowered;
        }
        _isPowered = isPowered;
    }

    public override bool GetPoweredState(PowerableBase requestor)
    {
        return _currentColorTypes.Count > 0;
    }
}
