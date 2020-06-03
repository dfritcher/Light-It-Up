using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Inhibitor : PowerableBase
{
    [SerializeField]
    private List<ColorType> _userSetColorType = new List<ColorType>();

    [SerializeField]
    private List<ColorType> _currentColorTypes = null;
    public override List<ColorType> CurrentColorTypes { get { return _currentColorTypes ?? (_currentColorTypes = new List<ColorType>()); } }

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
    private List<PowerableBase> _powerables = null;

    [SerializeField]
    private List<PowerSource> _powerSources = null;

    [SerializeField]
    private List<Wire> _wires = null;

    [SerializeField]
    private List<Junction> _junctions = null;

    [SerializeField]
    private List<Bulb> _bulbs = null;

    [SerializeField]
    private List<PowerSource> _poweredBulbs = null;

    private string _objectName = string.Empty;

    private List<ColorType> _emptyColors = new List<ColorType>() { ColorType.None };
    private List<ColorType> _allColors = new List<ColorType>() { ColorType.Red, ColorType.Green, ColorType.Blue };
    public delegate void InhibitorEvent(Inhibitor inhibitor);
    public event InhibitorEvent OnClick;

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
        _currentColorTypes = new List<ColorType>(_originalColorTypes);
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

    public override List<ColorType> GetPowers(PowerableBase requestor)
    {
        return GetPoweredColors(requestor);

        if (_isPowered)
            return GetPoweredColors(requestor);
        else
            return _emptyColors;
    }

    private List<ColorType> GetPoweredColors(PowerableBase requestor)
    {
        var passingingColors = new List<ColorType>();
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
            //Pass all colors that we don't block
            if (!_userSetColorType.Contains(color))
            {
                passingingColors.Add(color);
            }
        }

        return passingingColors;
    }

    private List<ColorType> OLDGetPowers(PowerableBase requestor)
    {
        if (_poweredBulbs == null || _poweredBulbs.Count == 0)
            return _currentColorTypes;

        if (_poweredBulbs.Any(pb => pb.Powerable == requestor))
        {
            var direction = _poweredBulbs.Find(p => p.Powerable == requestor).InputDirection;
            var powerSources = _powerSources.FindAll(p => p.InputDirection != direction);

            var poweredColors = new List<ColorType>();

            foreach (var source in powerSources)
            {
                if (!source.Powerable.GetPoweredState(this))
                    continue;
                var powers = source.Powerable.GetPowers(this);
                foreach (var color in powers)
                {
                    if (!_userSetColorType.Contains(color))
                    {
                        poweredColors.Add(color);
                    }
                }
            }

            poweredColors = poweredColors.Distinct().ToList();
            //If no color is set, then set all colors as the default
            if (CurrentColorTypes.Count == 0 || CurrentColorTypes.Contains(ColorType.None))
                poweredColors = poweredColors.Except(_allColors).ToList();

            //poweredColors = poweredColors.Except(CurrentColorTypes).ToList();

            return poweredColors;
        }
        else if (_powerSources.Any(pb => pb.Powerable == requestor))
        {
            var direction = _powerSources.Find(p => p.Powerable == requestor).InputDirection;
            var powerSources = _powerSources.FindAll(p => p.InputDirection != direction);

            var poweredColors = new List<ColorType>();

            foreach (var source in powerSources)
            {
                if (source.Powerable == requestor)
                    continue;
                if (!source.Powerable.GetPoweredState(this))
                    continue;
                var powers = source.Powerable.GetPowers(this);
                foreach (var color in powers)
                {
                    if (!_userSetColorType.Contains(color))
                    {
                        poweredColors.Add(color);
                    }
                }
            }

            poweredColors = poweredColors.Distinct().ToList();
            //If no color is set, then set all colors as the default
            if (CurrentColorTypes.Count == 0 || CurrentColorTypes.Contains(ColorType.None))
                poweredColors = poweredColors.Except(_allColors).ToList();

            //poweredColors = poweredColors.Except(CurrentColorTypes).ToList();

            return poweredColors;
        }
        return _currentColorTypes;
    }
    private void SetPoweredState()
    {
        foreach (var powerable in _powerables)
        {            
            var currentPower = powerable.GetPowers(this);
            if (currentPower.Any(cp => cp != ColorType.None))
            {
                _isPowered = true;
            }
        }        
    }

    public override void UpdatePowerState(PowerableBase powerableBase)
    {
        SetPoweredState();
        _currentColorTypes.Clear();
        foreach (var powerable in _powerables)
        {
            foreach (var color in powerable.GetPowers(this))
            {
                if (!_userSetColorType.Contains(color))
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
            if (inputDirection != bulb.InputDirection)
                bulb.Powerable.UpdatePowerState(this);
        }

    }

    public override void CascadeReset(PowerableBase powerableBase)
    {
        //_powerables.ForEach(o => { if (o != powerableBase) { o.CascadeReset(this); } });
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
}
