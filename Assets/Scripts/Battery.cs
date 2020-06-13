using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Battery : PowerableBase
{
    /// <summary>
    /// Reference to the current colors we are generating.
    /// </summary>
    public override List<ColorType> CurrentColorTypes { get { return _power.ColorTypes; } }

    [SerializeField]
    private List<PowerableBase> _objectsWePower = new List<PowerableBase>();

    [SerializeField]
    private List<Wire> _wires = null;

    [SerializeField]
    private List<Junction> _junctions = null;

    [SerializeField]
    private List<Bulb> _bulbs = null;

    [SerializeField]
    private List<Image> _batteryColors = null;

    [SerializeField]
    private bool _isClickable = true;
    public override bool IsClickable { get { return _isClickable; } }

    public override bool IsPowered { get { return _power.ColorTypes.Any(c => c != ColorType.None); } }

    [SerializeField]
    private Button _selectBatteryOption = null;

    [SerializeField]
    private Image _lockedIcon = null;

    public delegate void BatteryEvent(Battery battery);
    public event BatteryEvent OnClick; // Event for user selecting a color

    protected override void Awake()
    {
        _selectBatteryOption.interactable = _isClickable;
        _lockedIcon.gameObject.SetActive(!_isClickable);       
    }

    private void Start()
    {
        SetBatteryTypes(_originalColorTypes);
    }

    public void Setup()
    {
        
    }

    public override void ResetPowerable()
    {
        _power.ColorTypes = new List<ColorType>(_originalColorTypes);
        UpdateColorDisplay();
    }

    public void ResetPower()
    {
        SetBatteryTypes(_originalColorTypes);
    }

    /// <summary>
    /// Called from Unity when the player clicks on us.
    /// </summary>
    public void BatteryClicked()
    {
        if(_isClickable)
            OnClick?.Invoke(this);
    }

    /// <summary>
    /// Sets our current colors.
    /// </summary>
    /// <param name="colorTypes"></param>
    public void SetBatteryTypes(List<ColorType> colorTypes)
    {
        _power.ColorTypes = colorTypes;
        UpdateColorDisplay();
        //CascadeReset(null);
        foreach (var powerable in _objectsWePower)
        {
            powerable.UpdatePowerState(this);
        }

        foreach (var wire in _wires)
        {
            wire.UpdatePowerState(this);
        }

        foreach (var junction in _junctions)
        {
            junction.UpdatePowerState(this);
        }

        foreach (var bulb in _bulbs)
        {
            bulb.UpdatePowerState(this);
        }
    }

    private void UpdateColorDisplay()
    {
        _batteryColors[0].gameObject.SetActive(CurrentColorTypes.Contains(ColorType.Red));
        _batteryColors[1].gameObject.SetActive(CurrentColorTypes.Contains(ColorType.Green));
        _batteryColors[2].gameObject.SetActive(CurrentColorTypes.Contains(ColorType.Blue));
    }

    /// <summary>
    /// Method to determine what colors we currently provide to other sources.
    /// For Batteries we return whatever color was set at the start or by the player.
    /// </summary>
    /// <param name="requestor"></param>
    /// <returns></returns>
    public override List<Power> GetPowers(PowerableBase requestor)
    {
        return new List<Power>() { _power };
    }

    public override void UpdatePowerState(PowerableBase powerableBase)
    {
        //Do nothing
    }
   
    public override bool GetPoweredState(PowerableBase requestor)
    {
        return _power.ColorTypes.Count > 0;
    }
}