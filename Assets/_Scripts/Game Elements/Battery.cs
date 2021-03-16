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
    //public List<ColorType> CurrentColorTypes { get { return Power.ColorTypes; } }

    [SerializeField]
    private List<PowerableBase> _objectsWePower = new List<PowerableBase>();

    [SerializeField]
    private List<Wire> _wires = null;

    [SerializeField]
    private List<Bulb> _bulbs = null;

    [SerializeField]
    private List<Junction> _junctions = null;

    //[SerializeField]
    //private List<Image> _batteryColors = null;

    [SerializeField]
    private bool _isClickable = true;
    public override bool IsClickable { get { return _isClickable; } }

    [SerializeField]
    private bool _hasVariablePower = false;

    private bool _isPowered = false;
    public override bool IsPowered { get { return _isPowered; } }

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
    [SerializeField]
    private float[] _pitch = null;
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

        CurrentPower.ColorTypes = _originalColorTypes.Clone();
        _isPowered = _originalColorTypes.Any(c => c != ColorType.None);
        SetSelectedState(false);
        SetupPoweredObjects();
    }

    private void Start()
    {        
        UpdatePoweredObjects();
        UpdateColorDisplay();
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
            _increasePowerButton.gameObject.SetActive(_hasVariablePower);
            _increasePowerButton.interactable = CurrentPower.Amount < _maxPower;
        }
        if(_decreasePowerButton)
        {
            _decreasePowerButton.gameObject.SetActive(_hasVariablePower);
            _decreasePowerButton.interactable = CurrentPower.Amount > _minPower;
        }
        
        if(_powerDisplay)
            _powerDisplay.text = CurrentPower.Amount.ToString();        
    }

    public override void ResetPowerable()
    {
        CurrentPower.Amount = _minPower;
        CurrentPower.ColorTypes= _originalColorTypes.Clone();
        if(_powerDisplay)
            _powerDisplay.text = _minPower.ToString();
        _increasePowerButton.interactable = CurrentPower.Amount == _maxPower ? false : true;
        _decreasePowerButton.interactable = CurrentPower.Amount == _minPower ? false : true;
        
        UpdateColorDisplay();
    }

    public void ResetPower()
    {
        SetBatteryTypes(_originalColorTypes.Clone(), false);
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
        CurrentPower.Amount++;
        if (CurrentPower.Amount >= _maxPower)
        {
            CurrentPower.Amount = _maxPower;
            _increasePowerButton.interactable = false;
        }

        _decreasePowerButton.interactable = true;
        _powerDisplay.text = CurrentPower.Amount.ToString();
        UpdatePoweredObjects();
        if(IsPowered)
            PlayIncreasePowerAudio();
    }

    public void DecreasePower()
    {
        CurrentPower.Amount--;
        if (CurrentPower.Amount <= _minPower)
        {
            CurrentPower.Amount = _minPower;
            _decreasePowerButton.interactable = false;
        }
        _increasePowerButton.interactable = true;
        _powerDisplay.text = CurrentPower.Amount.ToString();
        UpdatePoweredObjects();
        if(IsPowered)
            PlayDecreasePowerAudio();
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
        CurrentPower.ColorTypes = colorTypes;
        _isPowered = colorTypes.Any(c => c != ColorType.None);
        yield return null;
        UpdateColorDisplay();
        yield return null;        
        UpdatePoweredObjects();
        yield return null;
        if (playAudio)
            PlayPowerStateAudio();
    }

    private void PlayPowerStateAudio()
    {
        if(IsPowered)
        {
            PlayIncreasePowerAudio();           
        }
        else
        {
            PlayDecreasePowerAudio();
        }
    }

    private void PlayIncreasePowerAudio()
    {
        AudioManager.PlayOneShot(_powerUpClip, _pitch[CurrentPower.Amount - 1]);
    }

    private void PlayDecreasePowerAudio()
    {
        AudioManager.PlayOneShot(_powerDownClip, _pitch[CurrentPower.Amount - 1]);
    }

    private void UpdatePoweredObjects()
    {
        // We could make each part a subroutine that waits for others to finish if we need UI responsiveness
        //_objectsWePower.ForEach(p => p.DetermineNewPowerState(this));
        //_objectsWePower.ForEach(p => p.DeterminePowerColorStateChange(this));
        _externalPowerSources.ForEach(p => p.Powerable.CheckStateChanged(this, true));                
        _wires.ForEach(w => w.CheckStateChanged(this, true));        
        _bulbs.ForEach(b => b.CheckStateChanged(this, true));
        _junctions.ForEach(w => w.CheckStateChanged(this, true));
    }
    
    private void SetupPoweredObjects()
    {
        _objectsWePower.ForEach(p => p.Setup(this));
        _wires.ForEach(w => w.Setup(this));
        _bulbs.ForEach(b => b.Setup(this));
        _junctions.ForEach(b => b.Setup(this));
    }

    private void UpdateColorDisplay()
    {
        _redSection.SetActive(CurrentPower.ColorTypes.Contains(ColorType.Red));
        _greenSection.SetActive(CurrentPower.ColorTypes.Contains(ColorType.Green));
        _blueSection.SetActive(CurrentPower.ColorTypes.Contains(ColorType.Blue));        
    }

    /// <summary>
    /// Method to determine what colors we currently provide to other sources.
    /// For Batteries we return whatever color was set at the start or by the player.
    /// </summary>
    /// <param name="requestor"></param>
    /// <returns></returns>
    public override List<Power> GetPowers(PowerableBase requestor)
    {
        throw new NotImplementedException();
    }

    public override void DetermineNewPowerState(PowerableBase powerableSource)
    {
        //Do nothing
    }
   
    /// <summary>
    /// DEPRECATED - Should Check IsPowered Property to determin if this object is powered.
    /// </summary>
    /// <param name="requestor"></param>
    /// <returns></returns>
    public override bool GetPoweredState(PowerableBase requestor)
    {
        return CurrentPower.ColorTypes.Any(c => c != ColorType.None);
    }

    public override void SetPowerStateOff(PowerableBase requestor)
    {
        //Do Nothing.
    }
    
    public void SetSelectedState(bool selected)
    {
        _selectedSprite.gameObject.SetActive(selected);
    }

    public override void DeterminePowerColorStateChange(PowerableBase powerableSource, bool checkDirection = false)
    {
        
    }

    public override List<ColorType> GetOtherSideColors(PowerableBase requestor)
    {
        return CurrentPower.ColorTypes;
    }

    public override void CheckStateChanged(PowerableBase powerableSource, bool forceCheck)
    {
        throw new NotImplementedException();
    }

    public override bool IsPoweredFromOtherSide(PowerableBase requestor)
    {
        return IsPowered;
    }

    #endregion Methods (end)
}