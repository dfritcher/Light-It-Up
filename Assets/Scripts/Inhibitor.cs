using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inhibitor : PowerableBase
{
    #region Fields, Properties
    [SerializeField]
    private List<ColorType> _userSetColorType = new List<ColorType>();

    public override List<ColorType> CurrentColorTypes { get { return _power.ColorTypes; } }

    [SerializeField]
    private List<Image> _inhibitorColors = null;

    [SerializeField]
    private bool _isClickable = true;
    public override bool IsClickable { get { return _isClickable; } }

    [SerializeField]
    private bool _isPowered = false;
    public override bool IsPowered { get { return _isPowered; } }

    [SerializeField]
    private Button _selectInhibitorOption = null;

    [SerializeField]
    private Image _lockedIcon = null;
    
    [SerializeField]
    private List<PowerSource> _powerSources = null;

    [SerializeField]
    private List<Wire> _wires = null;

    [SerializeField]
    private List<Junction> _junctions = null;

    [SerializeField]
    private List<PowerSource> _poweredBulbs = null;

    private string _objectName = string.Empty;

    private List<ColorType> _emptyColors = new List<ColorType>() { ColorType.None };
    private List<Power> _emptyPower = new List<Power>();

    private List<ColorType> _allColors = new List<ColorType>() { ColorType.Red, ColorType.Green, ColorType.Blue };
    private List<Power> _allPowers = new List<Power>() { new Power() { Amount = 1, ColorTypes = new List<ColorType>() { ColorType.Red, ColorType.Green, ColorType.Blue } } };
    #endregion Fields, Properties (end)

    #region Delegates, Events
    public delegate void InhibitorEvent(Inhibitor inhibitor);
    public event InhibitorEvent OnClick;
    #endregion Delegates, Events (end)

    #region Methods
    protected override void Awake()
    {
        base.Awake();
        _selectInhibitorOption.interactable = _isClickable;
        _lockedIcon.gameObject.SetActive(!_isClickable);
        _objectName = gameObject.name;
    }
    // Update is called once per frame
    void Update()
    {

    }

    public void Setup()
    {

    }

    public override void ResetPowerable()
    {
        _power.ColorTypes = new List<ColorType>(_originalColorTypes);
        _userSetColorType = new List<ColorType>(_originalColorTypes);
        UpdateColorDisplay();
    }

    public void InhibitorClicked()
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
        _inhibitorColors[0].gameObject.SetActive(_userSetColorType.Contains(ColorType.Red));
        _inhibitorColors[1].gameObject.SetActive(_userSetColorType.Contains(ColorType.Green));
        _inhibitorColors[2].gameObject.SetActive(_userSetColorType.Contains(ColorType.Blue));
    }

    public override List<Power> GetPowers(PowerableBase requestor)
    {
        //return GetPoweredColors(requestor);
        CheckPoweredState(requestor);
        if (_isPowered)
            return GetPoweredColors(requestor);
        else
            return _emptyPower;
    }

    private List<Power> GetPoweredColors(PowerableBase requestor)
    {
        var passingPowers = new List<Power>();
        var poweredColors = new List<Power>();

        var direction = _powerSources.Find(ps => ps.Powerable == requestor)?.InputDirection;

        foreach (var source in _powerSources)
        {
            if (source.Powerable == requestor)
                continue;
            if (source.Powerable.GetPoweredState(this) && source.InputDirection != direction)
            {
                poweredColors.AddRange(source.Powerable.GetPowers(this));
            }
        }

        foreach (var power in poweredColors)
        {
            var passingColors = new List<ColorType>();
            foreach(var color in power.ColorTypes)
            {
                //Pass all colors that we don't block
                if (!_userSetColorType.Contains(color))
                {
                    passingColors.Add(color);                       
                }                               
            }
            if(passingColors.Count > 0)
                passingPowers.Add(new Power() { Amount = power.Amount, ColorTypes = passingColors });
        }

        return passingPowers;
    }
    
    public override void UpdatePowerState(PowerableBase powerableBase)
    {
        CheckPoweredState(powerableBase);
        _power.ColorTypes.Clear();
        foreach (var source in _powerSources)
        {
            foreach (var color in source.Powerable.CurrentColorTypes)
            {
                if (!_userSetColorType.Contains(color))
                {
                    _power.ColorTypes.Add(color);
                }
            }
        }

        //Some source has updated we need to update all the sources that we power
        // We don't need to update the source that is telling us to update.
        foreach (var source in _powerSources)
        {
            if (source.Powerable == powerableBase) //skip the guy who is telling us to update
                continue;
            source.Powerable.UpdatePowerState(this);
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

        //foreach (var bulb in _poweredBulbs)
        //{
        //    if (inputDirection != bulb.InputDirection)
        //        bulb.Powerable.UpdatePowerState(this);
        //}

    }
    
    private void CheckPoweredState(PowerableBase powerableBase)
    {
        var isPowered = false;
        var inputDirection = _powerSources.Find(ps => ps.Powerable == powerableBase)?.InputDirection;
        foreach (var source in _powerSources)
        {
            if (source.InputDirection != inputDirection)
                isPowered = source.Powerable.GetPoweredState(this);
            if (isPowered)
                break;
        }
        _isPowered = isPowered;       
    }
    
    public override bool GetPoweredState(PowerableBase requestor)
    {
        var inputDirection = _powerSources.Find(ps => ps.Powerable == requestor)?.InputDirection;
        _isPowered = false;
        foreach (var source in _powerSources)
        {
            if (source.InputDirection != inputDirection)
                _isPowered = source.Powerable.GetPoweredState(this);
            if (_isPowered)
                break;
        }

        return _isPowered;
    }
    #endregion Methods (end)
}