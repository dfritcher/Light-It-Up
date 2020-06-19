using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Junction : PowerableBase
{

    [SerializeField]
    private List<ColorType> _currentColorTypes = null;
    public override List<ColorType> CurrentColorTypes { get { return _currentColorTypes ?? (_currentColorTypes = new List<ColorType>()); } }

    public override bool IsClickable => false;

    [SerializeField]
    private bool _isPowered = false;
    public override bool IsPowered { get { return _isPowered; } }

    [SerializeField]
    private List<Image> _junctionColors = null;

    [SerializeField]
    private List<PowerableBase> _powerables = null;

    [SerializeField]
    private List<PowerSource> _poweredBulbs = null;

    /// <summary>
    /// Reference to all the sources providing us power and the direction the power is coming from.
    /// This will help us determine which directions power is flowing when asked for our colors.
    /// </summary>
    [SerializeField]
    private List<PowerSource> _powerSources = null;

    protected override void Awake()
    {
        base.Awake();
        UpdateColorDisplay();
    }

    public void Setup()
    {
        
    }

    private void SetCurrentPower()
    {
        if (_powerables == null)
            return;

        var colors = new List<ColorType>();
        //Figure out our current power/colors
        foreach (var powerable in _powerables)
        {
            if (powerable.IsPowered)
            {
                var colorsToAdd = powerable.GetPowers(this);
                colorsToAdd.ForEach(c => c.ColorTypes.Remove(ColorType.None));
                colors.AddRange(colorsToAdd.SelectMany(c => c.ColorTypes));
            }
        }
        colors.Distinct().ToList();
        if (colors.Count > 0)
            _currentColorTypes = colors;
        else
        {
            _currentColorTypes = _originalColorTypes;
        }
    }

    private int GetIndexFromPower(ColorType power)
    {
        switch (power)
        {
            case ColorType.None:
                return 0;
            case ColorType.Red:
                return 1;
            case ColorType.Green:
                return 2;
            case ColorType.Blue:
                return 3;
            default:
                return 0;
        }
    }

    private void UpdateColorDisplay()
    {
        _junctionColors[1].gameObject.SetActive(CurrentColorTypes.Contains(ColorType.Red));
        _junctionColors[2].gameObject.SetActive(CurrentColorTypes.Contains(ColorType.Green));
        _junctionColors[3].gameObject.SetActive(CurrentColorTypes.Contains(ColorType.Blue));

        _junctionColors[0].gameObject.SetActive(!_junctionColors[1].IsActive() && !_junctionColors[2].IsActive() && !_junctionColors[3].IsActive());
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
        CheckPoweredState(powerableBase);
        UpdateColorDisplay();

        ////Notify everyone we updated
        //foreach (var powerable in _powerables)
        //{
        //    if (powerable == powerableBase)
        //        continue;
        //    powerable.UpdatePowerState(this);
        //}
    }

    private void CheckPoweredState(PowerableBase powerableBase)
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
