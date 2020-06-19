using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Battery : PowerableBase
{
    #region Fields, Properties
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

    [SerializeField]
    private bool _hasVariablePower = false;

    public override bool IsPowered { get { return _power.ColorTypes.Any(c => c != ColorType.None); } }

    [SerializeField]
    private Button _selectBatteryOption = null;

    [SerializeField]
    private Image _lockedIcon = null;

    [SerializeField]
    private TextMeshProUGUI _powerDisplay = null;

    [SerializeField]
    private Button _increasePowerButton = null;

    [SerializeField]
    private Button _decreasePowerButton = null;

    [SerializeField]
    private int _minPower = 0;

    [SerializeField]
    private int _maxPower = 4;
    #endregion Fields, Properties (end)

    #region Delegates, Events
    public delegate void BatteryEvent(Battery battery);
    public event BatteryEvent OnClick; // Event for user selecting a color
    #endregion Delegates, Events (end)

    #region Methods
    #region Unity Engine Methods
    protected override void Awake()
    {
        _selectBatteryOption.interactable = _isClickable;
        _lockedIcon.gameObject.SetActive(!_isClickable);       
    }

    private void Start()
    {
        SetBatteryTypes(_originalColorTypes);
    }
    #endregion Unity Engine Methos (end)

    public void Setup()
    {
        _increasePowerButton.gameObject.SetActive(_hasVariablePower && _isClickable);
        _increasePowerButton.interactable = _power.Amount < _maxPower;

        _decreasePowerButton.gameObject.SetActive(_hasVariablePower && _isClickable);
        _decreasePowerButton.interactable = _power.Amount > _minPower;
        
        _powerDisplay.text = _power.Amount.ToString();
        
    }

    public override void ResetPowerable()
    {
        _power.ColorTypes = new List<ColorType>(_originalColorTypes);
        _power.Amount = _minPower;
        _powerDisplay.text = _minPower.ToString();
        UpdateColorDisplay();
    }

    public void ResetPower()
    {
        SetBatteryTypes(_originalColorTypes);
    }

    #region Unity Called Methods
    /// <summary>
    /// Called from Unity when the player clicks on us.
    /// </summary>
    public void BatteryClicked()
    {
        if(_isClickable)
            OnClick?.Invoke(this);
    }

    public void IncreasePower()
    {
        _power.Amount++;
        if (_power.Amount > _maxPower)
        {
            _power.Amount = _maxPower;
            _increasePowerButton.interactable = false;
        }
        _decreasePowerButton.interactable = true;
        _powerDisplay.text = _power.Amount.ToString();
        UpdatePoweredObjects();
    }

    public void DecreasePower()
    {
        _power.Amount--;
        if (_power.Amount < _minPower)
        {
            _power.Amount = _minPower;
            _decreasePowerButton.interactable = false;
        }
        _increasePowerButton.interactable = true;
        _powerDisplay.text = _power.Amount.ToString();
        UpdatePoweredObjects();
    }
    #endregion Unity Called Methods (end)
    /// <summary>
    /// Sets our current colors.
    /// </summary>
    /// <param name="colorTypes"></param>
    public void SetBatteryTypes(List<ColorType> colorTypes)
    {
        _power.ColorTypes = colorTypes;
        UpdateColorDisplay();

        UpdatePoweredObjects();
    }

    private void UpdatePoweredObjects()
    {
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
        return _power.ColorTypes.Any(c => c != ColorType.None);
    }
    #endregion Methods (end)
}