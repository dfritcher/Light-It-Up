using System;
using System.Collections;
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

    [SerializeField]
    private GameObject _redSection = null;

    [SerializeField]
    private GameObject _greenSection = null;

    [SerializeField]
    private GameObject _blueSection = null;

    [SerializeField]
    private SpriteRenderer _selectedSprite = null;

    [SerializeField]
    private Canvas _increaseButtonCanvas = null;

    [SerializeField]
    private Canvas _decreaseButtonCanvas = null;

    [Header("Audio References"), Space(8)]
    [SerializeField]
    private AudioSource _audioSource = null;
    [SerializeField]
    private AudioClip _powerDownClip = null;
    [SerializeField]
    private AudioClip _powerUpClip = null;
    #endregion Fields, Properties (end)

    #region Delegates, Events
    public delegate void BatteryEvent(Battery battery);
    public event BatteryEvent OnClick; // Event for user selecting a color
    #endregion Delegates, Events (end)

    #region Methods
    #region Unity Engine Methods
    protected override void Awake()
    {
        if(_selectBatteryOption != null)
            _selectBatteryOption.interactable = _isClickable;
        if(_lockedIcon != null)
            _lockedIcon.gameObject.SetActive(!_isClickable);

        _power.ColorTypes = _originalColorTypes;
        SetSelectedState(false);
    }

    private void Start()
    {
        _power.ColorTypes = _originalColorTypes;
        UpdateColorDisplay();
        UpdatePoweredObjects();
    }

    private void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonUp(0))
        {
            var response = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 100f);
            
            if (response.transform == transform)
            {
                BatteryClicked();
            }                            
        }
#endif

#if UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            var response = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.touches[0].position), Vector2.zero, 100f);

            if (response.transform == transform)
            {
                BatteryClicked();
            }
        }
#endif
    }
    #endregion Unity Engine Methos (end)

    public void Setup(Camera mainCamera)
    {
        _increaseButtonCanvas.worldCamera = mainCamera;
        _decreaseButtonCanvas.worldCamera = mainCamera;

        if(_increasePowerButton)
        {
            _increasePowerButton.gameObject.SetActive(_hasVariablePower && _isClickable);
            _increasePowerButton.interactable = _power.Amount < _maxPower;
        }
        if(_decreasePowerButton)
        {
            _decreasePowerButton.gameObject.SetActive(_hasVariablePower && _isClickable);
            _decreasePowerButton.interactable = _power.Amount > _minPower;
        }
        
        if(_powerDisplay)
            _powerDisplay.text = _power.Amount.ToString();        
    }

    public override void ResetPowerable()
    {
        _power.ColorTypes = new List<ColorType>(_originalColorTypes);
        _power.Amount = _minPower;
        if(_powerDisplay)
            _powerDisplay.text = _minPower.ToString();
        //UpdateColorDisplay();
    }

    public void ResetPower()
    {
        SetBatteryTypes(_originalColorTypes, false);
    }

#region Unity Called Methods
    /// <summary>
    /// Called from Unity when the player clicks on us.
    /// </summary>
    public void BatteryClicked()
    {
        if (_isClickable)
        {
            SetSelectedState(true);
            OnClick?.Invoke(this);
        }            
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
    public void SetBatteryTypes(List<ColorType> colorTypes, bool playAudio = true)
    {
        StopAllCoroutines();
        StartCoroutine(SetBatteryTypesCoroutine(colorTypes, playAudio));
    }

    private IEnumerator SetBatteryTypesCoroutine(List<ColorType> colorTypes, bool playAudio = true)
    {
        _power.ColorTypes = colorTypes;
        yield return null;
        UpdateColorDisplay();
        yield return null;
        
        UpdatePoweredObjects();
        yield return null;
        if (playAudio)
            PlayAudio();
    }

    private void PlayAudio()
    {
        if(_power.ColorTypes.Any(c => c != ColorType.None))
        {
            AudioManager.PlayOneShot(_powerUpClip);            
        }
        else
        {
            AudioManager.PlayOneShot(_powerDownClip);
        }
    }

    private void UpdatePoweredObjects()
    {
        _objectsWePower.ForEach(p => p.SetPowerStateOff(this));
        _objectsWePower.ForEach(p => p.GetBatteryPowerState(this));
        _objectsWePower.ForEach(p => p.DetermineNewPowerState(this));
        _wires.ForEach(w => w.GetBatteryPowerState(this));
        _bulbs.ForEach(b => b.GetBatteryPowerState(this));        
    }

    private void UpdateColorDisplay()
    {
        _redSection.SetActive(CurrentColorTypes.Contains(ColorType.Red));
        _greenSection.SetActive(CurrentColorTypes.Contains(ColorType.Green));
        _blueSection.SetActive(CurrentColorTypes.Contains(ColorType.Blue));        
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

    public override void GetBatteryPowerState(PowerableBase powerableBase)
    {
        //Do nothing
    }
   
    public override bool GetPoweredState(PowerableBase requestor)
    {        
        return _power.ColorTypes.Any(c => c != ColorType.None);
    }

    public override void SetPowerStateOff(PowerableBase requestor)
    {
        //Do Nothing.
    }
    public void SetSelectedState(bool selected)
    {
        _selectedSprite.gameObject.SetActive(selected);
    }

    public override void DetermineNewPowerState(PowerableBase powerableBase, bool checkDirection = false)
    {
        
    }

    #endregion Methods (end)
}