using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PassThrough : PowerableBase
{
    [SerializeField]
    private List<ColorType> _userSetColorType = new List<ColorType>();

    [SerializeField]
    private List<ColorType> _currentColorTypes = null;
    public override List<ColorType> CurrentColorTypes { get { return _currentColorTypes ?? (_currentColorTypes = new List<ColorType>()); } }

    /// <summary>
    /// Reference to all that objects we can get power from.
    /// </summary>    
    [SerializeField]
    private List<PowerableBase> _powerables = null;

    [SerializeField]
    private List<Wire> _wires = null;

    [SerializeField]
    private List<Junction> _junctions = null;

    [SerializeField]
    private List<Bulb> _bulbs = null;

    [SerializeField]
    private List<PowerSource> _poweredBulbs = null;

    [SerializeField]
    private List<Image> _passThroughColors = null;

    /// <summary>
    /// Reference to all the Batteries providing us power and the direction the power is coming from.
    /// This will help us determine which directions power is flowing when asked for our colors.
    /// </summary>
    [SerializeField]
    private List<PowerSource> _powerSources = null;

    [SerializeField]
    private bool _isClickable = true;
    public override bool IsClickable { get { return _isClickable; } }

    [SerializeField]
    private bool _isPowered = false;
    public override bool IsPowered { get { return _isPowered; } } 

    [SerializeField]
    private Button _selectPassThroughOption = null;

    [SerializeField]
    private Image _lockedIcon = null;

    public delegate void PassThroughEvent(PassThrough passThrough);
    public event PassThroughEvent OnClick;

    protected override void Awake()
    {
        base.Awake();
        _selectPassThroughOption.interactable = _isClickable;
        _lockedIcon.gameObject.SetActive(!_isClickable);
        UpdatePowerState(null);
    }
    
    public void Setup()
    {
        
    }

    public override void ResetPowerable()
    {
        _currentColorTypes = new List<ColorType>(_originalColorTypes);
        _userSetColorType = new List<ColorType>(_originalColorTypes);
        UpdateColorDisplay();
    }
    
    public void PassThroughClicked()
    {
        if (_isClickable)
            OnClick?.Invoke(this);
    }

    public void SetUserSelectedPower(List<ColorType> colorTypes)
    {
        _userSetColorType = colorTypes;
        UpdateColorDisplay();
        UpdatePowerState(this);
    }
    
    private void UpdateColorDisplay()
    {
        _passThroughColors[0].gameObject.SetActive(_userSetColorType.Contains(ColorType.Red));
        _passThroughColors[1].gameObject.SetActive(_userSetColorType.Contains(ColorType.Green));
        _passThroughColors[2].gameObject.SetActive(_userSetColorType.Contains(ColorType.Blue));
    }

    public override List<ColorType> GetPowers(PowerableBase requestor)
    {
        //return _currentColorTypes;
        //TODO: Look at all our power sources, check if they are on. 
        //TODO: Return the colors we pass that are also being powered to us.

        var passingColors = new List<ColorType>();
        var poweredColors = new List<ColorType>();

        var direction = _poweredBulbs.Find(ps => ps.Powerable == requestor)?.InputDirection;

        foreach (var source in _powerSources)
        {
            if (source.Powerable == requestor)
                continue;
            if (source.Powerable.GetPoweredState(this) && source.InputDirection != direction)
            {
                poweredColors.AddRange(source.Powerable.GetPowers(this));
            }
        }

        foreach (var color in poweredColors)
        {
            if (_currentColorTypes.Contains(color))
            {
                passingColors.Add(color);
            }
        }

        return passingColors;
    }

    

    public override bool GetPoweredState(PowerableBase requestor)
    {
        var inputDirection = _powerSources.Find(ps => ps.Powerable == requestor)?.InputDirection;
        _isPowered = false;
        foreach(var source in _powerSources)
        {
            if (source.InputDirection != inputDirection)
                _isPowered = source.Powerable.GetPoweredState(this);
            if (_isPowered)
                break;
        }

        return _isPowered;
    }

    /// <summary>
    /// Method to have an external powersource tell us they were updated.
    /// </summary>
    /// <param name="powerableBase"></param>
    public override void UpdatePowerState(PowerableBase powerableBase)
    {
        CheckPoweredState(powerableBase);
        _currentColorTypes.Clear();
        foreach(var powerable in _powerables)
        {
            foreach(var color in powerable.CurrentColorTypes)
            {
                if (_userSetColorType.Contains(color))
                {
                    _currentColorTypes.Add(color);
                }
            }
        }

        //Some source has updated we need to update all the sources that we power
        // We don't need to update the source that is telling us to update.
        foreach (var sources in _powerables)
        {
            if (sources == powerableBase) //skip the guy who is telling us to update
                continue;            
            sources.UpdatePowerState(this);
        }

        foreach (var wire in _wires)
        {
            wire.UpdatePowerState(this);
        }

        foreach (var junction in _junctions)
        {
            junction.UpdatePowerState(this);
        }

        var inputDirection = _powerSources.Find(ps => ps.Powerable == powerableBase)?.InputDirection;

        foreach (var bulb in _poweredBulbs)
        {
            if(inputDirection != bulb.InputDirection)
                bulb.Powerable.UpdatePowerState(this);
        }

        //Go through our list of power sources.
        //Get the direction of that power source.
        //If that source is being powered from any battery
        //Notify all objects in the other directions to update.
    }

    private void CheckPoweredState(PowerableBase powerableBase)
    {
        var isPowered = false;
        var inputDirection = _powerSources.Find(ps => ps.Powerable == powerableBase)?.InputDirection;
        foreach (var source in _powerSources)
        {
            if(source.InputDirection != inputDirection)
                isPowered = source.Powerable.GetPoweredState(this);
            if (isPowered)
                break;
        }
        _isPowered = isPowered;
    }

    public override void CascadeReset(PowerableBase powerableBase)
    {
        //_powerSources.ForEach(o => { if (o.Powerable != powerableBase) { o.Powerable.CascadeReset(this); } });
    }

}