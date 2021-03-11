using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
public class PassThrough : PowerableBase
{
    #region Fields, Properties
    #region Populated in Scene
    [Header("Populated In Scene")]

    [SerializeField, FormerlySerializedAs("_userSetColorType")]
    private List<ColorType> _userSetColorTypes = new List<ColorType>();
    private List<ColorType> UserSetColorTypes { get { return _userSetColorTypes; } }

    /// <summary>
    /// DEPRECATED - Reference to all the Batteries providing us power and the direction the power is coming from.
    /// This will help us determine which directions power is flowing when asked for our colors.
    /// </summary>
    [SerializeField]
    private List<ExternalPower> _powerSources = null;
    

    private ExternalPower[] _batteries = null;
    private ExternalPower[] Batteries { get { return _batteries ?? (_batteries = _externalPowerSources.FindAll(ps => ps.Powerable is Battery).ToArray()); } }

    private ExternalPower[] _inhibitors = null;
    private ExternalPower[] Inhibitors { get { return _inhibitors ?? (_inhibitors = _externalPowerSources.FindAll(ps => ps.Powerable is Inhibitor).ToArray()); } }

    private ExternalPower[] _passthroughs = null;
    private ExternalPower[] PassThroughs { get { return _passthroughs ?? (_passthroughs = _externalPowerSources.FindAll(ps => ps.Powerable is PassThrough).ToArray()); } }

    private Bulb[] _bulbs = null;
    private Bulb[] Bulbs { get { return _bulbs ?? (_bulbs = _externalPowerSources.FindAll(ps => ps.Powerable is Bulb).Select(ps => ps.Powerable as Bulb).ToArray()); } }

    [SerializeField]
    private List<Wire> _wires = null;

    [SerializeField]
    private bool _isClickable = true;
    public override bool IsClickable { get { return _isClickable; } }

    #endregion Populated in Scene (end)

    #region Populated by Code
    [Header("Populated by Code"), Space(8)]
    [SerializeField]
    private bool _isPowered = false;
    public override bool IsPowered { get { return _isPowered; } }
    //public override bool IsPowered { get { return _externalPowerSources.Where(ps => !(ps.Powerable is Bulb)).Any(ps => ps.Powerable.IsPoweredFromOtherSide(this)); } }
    [SerializeField, ReadOnly(true)]
    private bool _isPoweredFromOtherSide = false;

    private List<Power> _emptyPower = new List<Power>();

    private bool _poweredBySource = false;
    private bool _previousPowerState = false;
    private bool _currentPowerState = false;
    private bool _stateChanged = false;

    private PowerableBase _triggeringSource = null;
    #endregion Populated by Code (end)

    #region Populated By Prefab
    [Space(8), Header("Populated by Prefab")]
    [SerializeField]
    private Image _lockedIcon = null;
    [SerializeField]
    private Button _selectPassThroughOption = null;
    [SerializeField]
    private GameObject _redFullLit = null;
    [SerializeField]
    private GameObject _redTopLit = null;
    [SerializeField]
    private GameObject _greenFullLit = null;
    [SerializeField]
    private GameObject _greenTopLit = null;
    [SerializeField]
    private GameObject _greenBottomLit = null;
    [SerializeField]
    private GameObject _blueFullLit = null;
    [SerializeField]
    private GameObject _blueBottomLit = null;

    [SerializeField]
    private GameObject _redFullUnLit = null;
    [SerializeField]
    private GameObject _redTopUnLit = null;
    [SerializeField]
    private GameObject _greenFullUnLit = null;
    [SerializeField]
    private GameObject _greenTopUnLit = null;
    [SerializeField]
    private GameObject _greenBottomUnLit = null;
    [SerializeField]
    private GameObject _blueFullUnLit = null;
    [SerializeField]
    private GameObject _blueBottomUnLit = null;

    [SerializeField]
    private SpriteRenderer _selectedSprite = null;

    [Header("Audio References"), Space(8)]
    [SerializeField]
    private AudioSource _audioSource = null;
    [SerializeField]
    private AudioClip _powerDownClip = null;
    [SerializeField]
    private AudioClip _powerUpClip = null;
    #endregion Populated By Prefab (end)
    #endregion Fields, Properties (end)

    #region Delegates, Events
    public delegate void PassThroughEvent(PassThrough passThrough);
    public event PassThroughEvent OnClick;
    #endregion Delegates, Events (end)

    #region Methods
    #region Unity Engine Methods
    protected override void Awake()
    {
        base.Awake();
        _selectPassThroughOption.interactable = _isClickable;
        _lockedIcon.gameObject.SetActive(!_isClickable);
        _userSetColorTypes = _originalColorTypes.Clone();
        SetSelectedState(false);        
        SetupPoweredObjects();
    }

    private void Start()
    {
        CheckStateChanged(this, true);
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
                PassThroughClicked();
            }
        }
#endif

#if UNITY_ANDROID
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            var response = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.touches[0].position), Vector2.zero, 100f);

            if (response.transform == transform)
            {
                PassThroughClicked();
            }
        }
#endif
    }
    #endregion Unity Engine Methods (end)

    #region Overrides
    public override void DeterminePowerColorStateChange(PowerableBase powerable, bool checkDirection = false)
    {
        //if (powerable != this)
        //{
        //    var source = _externalPowerSources.Find(ps => ps.Powerable == powerable);
        //    if (source == null)
        //    {
        //        Debug.LogError($"Source IS NULL for {this.name}, Requestor:{powerable.name}");
        //        return;
        //    }
        //}

        //var previousPowers = PoweredColors.Clone();
        //PoweredColors.Clear();
        //foreach (var powerSource in _externalPowerSources)
        //{
        //    if(powerSource.Powerable is Battery battery)
        //    {
        //        if (powerSource.Powerable.CurrentPower.ColorTypes.Any(pc => UserSetColorTypes.Contains(pc)))
        //        {
        //            var power = new Power() { Amount = powerSource.Powerable.CurrentPower.Amount, ColorTypes = new List<ColorType>() };
        //            foreach (var color in powerSource.Powerable.CurrentPower.ColorTypes)
        //            {
        //                if (UserSetColorTypes.Contains(color))
        //                    power.ColorTypes.Add(color);
        //            }
        //            PoweredColors.Add(power);
        //        }
        //    }
        //    else
        //    {
        //        foreach (var poweredColor in powerSource.Powerable.PoweredColors)
        //        {
        //            if (poweredColor.ColorTypes.Any(pc => UserSetColorTypes.Contains(pc)))
        //            {
        //                var power = new Power() { Amount = poweredColor.Amount, ColorTypes = new List<ColorType>() };
        //                foreach (var color in poweredColor.ColorTypes)
        //                {
        //                    if (UserSetColorTypes.Contains(color))
        //                        power.ColorTypes.Add(color);
        //                }
        //                PoweredColors.Add(power);
        //            }
        //        }
        //    }            
        //}
        //if (previousPowers.Count() != PoweredColors.Count())
        //{
        //    _stateChanged = true;            
        //}
        //else
        //{
        //    for (int i = 0; i < PoweredColors.Count(); i++)
        //    {
        //        if (PoweredColors[i].ColorTypes.Intersect(previousPowers[i].ColorTypes).Count() != PoweredColors[i].ColorTypes.Count())
        //        {
        //            //_stateChanged = true;
        //            PoweredColors[i].ColorTypes = previousPowers[i].ColorTypes.Clone();
        //        }
        //    }
        //}
    }

    /// <summary>
    /// Method to determine if this powerable is getting power from another source.
    /// </summary>
    /// <param name="requestor"></param>
    /// <returns></returns>
    public override bool GetPoweredState(PowerableBase requestor)
    {
        //_isPowered = false;
        //foreach (var source in _powerSources)
        //{
        //    if (source.InputDirection != _powerSources.Find(ps => ps.Powerable == requestor)?.InputDirection)
        //        _isPowered = source.Powerable.GetPoweredState(this);
        //    if (_isPowered)
        //        break;
        //}

        return _isPowered;
    }

    public override List<Power> GetPowers(PowerableBase requestor)
    {
        if (IsPowered)
            return GetPoweredColors(requestor);
        else
            return _emptyPower;
    }

    public override void ResetPowerable()
    {
        _userSetColorTypes = _originalColorTypes.Clone();
        _isPowered = false;        
        UpdateColorDisplay();
    }

    public override List<ColorType> GetOtherSideColors(PowerableBase requestor)
    {
        var oppositeColors = new List<ColorType>();
        var inputDirection = _externalPowerSources.Find(ps => ps.Powerable == requestor).InputDirection;
        foreach (var source in _externalPowerSources)
        {
            if (source.InputDirection == inputDirection || source is Bulb)
                continue;
            var otherColors = source.Powerable.GetOtherSideColors(this);
            foreach (var color in otherColors)
            {
                if (UserSetColorTypes.Contains(color))
                {
                    oppositeColors.Add(color);
                }
            }
        }
        return oppositeColors;
    }

    public override bool IsPoweredFromOtherSide(PowerableBase requestor)
    {
        var inputDirection = _externalPowerSources.Find(ps => ps.Powerable == requestor)?.InputDirection;
        _isPoweredFromOtherSide = false;
        foreach (var source in _externalPowerSources)
        {
            if ((source.Powerable is Bulb) || source.InputDirection == inputDirection)
                continue;
            if (source.Powerable is Battery && source.Powerable.IsPowered)
                _isPoweredFromOtherSide = true;
            if (source.Powerable is Inhibitor inhibitor && inhibitor.IsPoweredFromOtherSide(this))
                _isPoweredFromOtherSide = true;
            if (source.Powerable is PassThrough passThrough && passThrough.IsPoweredFromOtherSide(this))
                _isPoweredFromOtherSide = true;
        }
        return _isPoweredFromOtherSide;
    }

    /// <summary>
    /// Method to have an external powersource tell us they were updated.
    /// </summary>
    /// <param name="powerableBase"></param>
    public override void DetermineNewPowerState(PowerableBase powerable)
    {
        _previousPowerState = _isPowered;
        _poweredBySource = false;
        // Get our new powered state from everyone else. 
        //CurrentPower.ColorTypes.Clear();
        //CurrentPower.Amount = 1;
        var newPoweredColors = new List<Power>();
        for (int i = 0; i < Batteries.Length; i++)
        {
            if (Batteries[i].Powerable.IsPowered && Batteries[i].Powerable.CurrentPower.ColorTypes.Any(c => UserSetColorTypes.Contains(c)))
            {
                _poweredBySource = true;
                var power = new Power() { Amount = Batteries[i].Powerable.CurrentPower.Amount, ColorTypes = new List<ColorType>(), Direction = Batteries[i].InputDirection };
                foreach (var color in Batteries[i].Powerable.CurrentPower.ColorTypes)
                {
                    if (UserSetColorTypes.Contains(color))
                        power.ColorTypes.Add(color);                    
                }
                newPoweredColors.Add(power);
            }
        }
        // Get our powered state from other Inhibitors
        foreach (var inhibitorSource in Inhibitors)
        {
            var inhibitor = inhibitorSource.Powerable as Inhibitor;
            if (inhibitor.IsPoweredFromOtherSide(this))// && inhibitor.CurrentPower.ColorTypes.Any(c => UserSetColorTypes.Contains(c)))
            {
                
                var poweredColors = inhibitor.GetPowers(this);
                foreach (var poweredColor in poweredColors)
                {
                    if (poweredColor.ColorTypes.Any(c => UserSetColorTypes.Contains(c)))
                    {
                        var power = new Power() { Amount = poweredColor.Amount, ColorTypes = new List<ColorType>(), Direction = inhibitorSource.InputDirection };
                        foreach (var color in poweredColor.ColorTypes)
                        {
                            if (UserSetColorTypes.Contains(color))
                                power.ColorTypes.Add(color);
                        }
                        newPoweredColors.Add(power);
                        _poweredBySource = true;
                    }
                }
            }
        }
        // Get our powered state from other PassThroughs 
        foreach (var passthroughSource in PassThroughs)
        {
            var passthrough = passthroughSource.Powerable as PassThrough;
            if (passthrough.IsPoweredFromOtherSide(this))// && passthrough.CurrentPower.ColorTypes.Any(c => UserSetColorTypes.Contains(c)))
            {                
                var poweredColors = passthrough.GetPowers(this);
                foreach (var poweredColor in poweredColors)
                {
                    if (poweredColor.ColorTypes.Any(c => UserSetColorTypes.Contains(c)))
                    {
                        var power = new Power() { Amount = poweredColor.Amount, ColorTypes = new List<ColorType>(), Direction = passthroughSource.InputDirection };
                        foreach (var color in poweredColor.ColorTypes)
                        {
                            if (UserSetColorTypes.Contains(color))
                                power.ColorTypes.Add(color);
                        }
                        newPoweredColors.Add(power);
                        _poweredBySource = true;
                    }
                }
            }
        }
        _isPowered = _currentPowerState = _poweredBySource;

        if (_previousPowerState != _currentPowerState)
        {
            _stateChanged = true;
        }
        if (newPoweredColors.Count() != PoweredColors.Count())
        {
            _stateChanged = true;            
        }
        else
        {
            for (int i = 0; i < PoweredColors.Count(); i++)
            {
                if (PoweredColors[i].ColorTypes.Intersect(newPoweredColors[i].ColorTypes).Count() != PoweredColors[i].ColorTypes.Count())
                {
                    _stateChanged = true;                                   
                }
            }
        }
        PoweredColors = newPoweredColors.Clone();
        //foreach (var power in PoweredColors)
        //{
        //    foreach (var color in power.ColorTypes)
        //    {
        //        if (UserSetColorTypes.Contains(color))
        //        {
        //            if (CurrentPower.ColorTypes.Contains(color))
        //            {
        //                CurrentPower.Amount++;
        //            }
        //            else
        //            {
        //                CurrentPower.ColorTypes.Add(color);
        //            }
        //        }
        //    }
        //}
    }

    #endregion Overrides (end)

    public void Setup()
    {

    }

    public override void Setup(PowerableBase powerableBase)
    {
        try
        {
            var source = _externalPowerSources.Find(ps => ps.Powerable == powerableBase);
            if (source == null)
            {
                Debug.LogError($"{name} could not find {powerableBase.name} in its External Power Sources.");
                return;
            }
                
            //source.Powerable.PoweredColors = powerableBase.PoweredColors;
            //PoweredColors.Add(new Power() { Amount = source.Powerable.CurrentPower.Amount, ColorTypes = source.Powerable.CurrentPower.ColorTypes.Clone() });
        }
        catch (Exception ex)
        {
            Debug.LogError($"{ex.Message} Occurred in {name}");
        }
    }

    public void PassThroughClicked()
    {
        if (_isClickable)
        {
            SetSelectedState(true);
            OnClick?.Invoke(this);
        }
    }

    public void SetSelectedState(bool selected)
    {
        _selectedSprite.gameObject.SetActive(selected);
    }

    public void SetUserSelectedPower(List<ColorType> colorTypes, bool playAudio = true)
    {
        StopAllCoroutines();
        StartCoroutine(SetUserSelectedPowerCoroutine(colorTypes, playAudio));
    }

    private IEnumerator SetUserSelectedPowerCoroutine(List<ColorType> colorTypes, bool playAudio = true)
    {
        _userSetColorTypes = colorTypes;
        //CurrentPower.ColorTypes.Clear();
        //foreach (var color in _allColors)
        //{
        //    if (UserSetColorTypes.Contains(color))
        //        CurrentPower.ColorTypes.Add(color);
        //}
        CheckStateChanged(this, true);
        yield return null;
        if (playAudio)
            PlayAudio();
    }

    public override void SetPowerStateOff(PowerableBase requestor)
    {
        _isPowered = false;
        var inputDirection = _externalPowerSources.Find(ps => ps.Powerable == requestor)?.InputDirection;
        foreach (var source in _externalPowerSources)
        {
            if (source == null || source == null || source.Powerable == requestor || source.Powerable is Bulb || source.InputDirection == inputDirection)
                continue;
            source.Powerable.SetPowerStateOff(this);
        }
    }

    public override void CheckStateChanged(PowerableBase powerable, bool forceCheck)
    {
        _triggeringSource = powerable;
        DetermineNewPowerState(powerable);
        //DeterminePowerColorStateChange(powerable);
        TriggerPropagation(powerable);
        ReCheckStateChange();
        UpdateColorDisplay();
    }

    #region Helper Methods
    private void ReCheckStateChange()
    {
        if (_stateChanged)
        {
            _stateChanged = false;
            CheckStateChanged(_triggeringSource, false);
        }
    }

    private List<Power> GetPoweredColors(PowerableBase requestor)
    {
        var providedPowers = new List<Power>();
        var direction = _externalPowerSources.Find(ps => ps.Powerable == requestor)?.InputDirection;

        //foreach (var source in _externalPowerSources)
        //{
        //    if (source == null || source == null || source.Powerable == requestor || source.Powerable is Bulb || source.InputDirection == direction)
        //        continue;
        //    if (source.Powerable.IsPowered)
        //    {
        //        foreach (var poweredColors in source.Powerable.PoweredColors)
        //        {
        //            if (poweredColors.ColorTypes.Any(pc => UserSetColorTypes.Contains(pc)))
        //            {
        //                var power = new Power() { Amount = poweredColors.Amount, ColorTypes = new List<ColorType>() };
        //                foreach (var color in poweredColors.ColorTypes)
        //                {
        //                    if (UserSetColorTypes.Contains(color))
        //                        power.ColorTypes.Add(color);
        //                    break;
        //                }
        //            }
        //        }
        //    }
        //}

        foreach (var poweredColor in PoweredColors)
        {
            if (poweredColor.Direction == direction)
                continue;
            providedPowers.Add(poweredColor);
        }

        return providedPowers;
    }

    private void SetupPoweredObjects()
    {
        _externalPowerSources.ForEach(ps => ps.Powerable.Setup(this));
        _wires.ForEach(w => w.Setup(this));
    }

    private void TriggerPropagation(PowerableBase powerable)
    {
        //TriggerPowerCheckPropagation();
        //TriggerPowerStatePropagation();
        TriggerCheckStateChangePropagation(powerable);
        //CheckStateChanged();
        //UpdateColorDisplay();
    }

    private void TriggerCheckStateChangePropagation(PowerableBase powerable)
    {
        var inputDirection = _externalPowerSources.Find(ps => ps.Powerable == powerable)?.InputDirection;
        // Tell all Inhibitors to check their power state based on batteries
        for (int i = 0; i < Inhibitors.Length; i++)
        {
            if (Inhibitors[i].InputDirection == inputDirection)
                continue;
            Inhibitors[i].Powerable.CheckStateChanged(this, false);
        }

        // Tell all PassThroughs to check their power state based on batteries
        for (int i = 0; i < PassThroughs.Length; i++)
        {
            if (PassThroughs[i].InputDirection == inputDirection)
                continue;
            PassThroughs[i].Powerable.CheckStateChanged(this, false);
        }
        foreach (var wire in _wires)
        {
            wire.CheckStateChanged(this, false);
        }
        for (int i = 0; i < Bulbs.Length; i++)
        {
            Bulbs[i].CheckStateChanged(this, false);
        }
    }

    private void PlayAudio()
    {
        if (_userSetColorTypes.Any(c => c != ColorType.None))
        {
            AudioManager.PlayOneShot(_powerUpClip);
        }
        else
        {
            AudioManager.PlayOneShot(_powerDownClip);
        }
    }
    
    private void UpdateColorDisplay()
    {
        _redFullLit.SetActive(false);
        _redTopLit.SetActive(false);
        _greenFullLit.SetActive(false);
        _greenTopLit.SetActive(false);
        _greenBottomLit.SetActive(false);
        _blueFullLit.SetActive(false);
        _blueBottomLit.SetActive(false);

        _redFullUnLit.SetActive(false);
        _redTopUnLit.SetActive(false);
        _greenFullUnLit.SetActive(false);
        _greenTopUnLit.SetActive(false);
        _greenBottomUnLit.SetActive(false);
        _blueFullUnLit.SetActive(false);
        _blueBottomUnLit.SetActive(false);

        if (IsPowered)
        {
            if (UserSetColorTypes.Contains(ColorType.Red) && !(UserSetColorTypes.Contains(ColorType.Green) || UserSetColorTypes.Contains(ColorType.Blue)))
            {
                _redFullLit.SetActive(true);
            }
            else if (UserSetColorTypes.Contains(ColorType.Green) && !(UserSetColorTypes.Contains(ColorType.Red) || UserSetColorTypes.Contains(ColorType.Blue)))
            {
                _greenFullLit.SetActive(true);
            }
            else if (UserSetColorTypes.Contains(ColorType.Blue) && !(UserSetColorTypes.Contains(ColorType.Red) || UserSetColorTypes.Contains(ColorType.Green)))
            {
                _blueFullLit.SetActive(true);
            }
            else if (UserSetColorTypes.Contains(ColorType.Red) && UserSetColorTypes.Contains(ColorType.Green) && !UserSetColorTypes.Contains(ColorType.Blue))
            {
                _redTopLit.SetActive(true);
                _greenBottomLit.SetActive(true);
            }
            else if (UserSetColorTypes.Contains(ColorType.Red) && UserSetColorTypes.Contains(ColorType.Blue) && !UserSetColorTypes.Contains(ColorType.Green))
            {
                _redTopLit.SetActive(true);
                _blueBottomLit.SetActive(true);
            }
            else if (UserSetColorTypes.Contains(ColorType.Green) && UserSetColorTypes.Contains(ColorType.Blue) && !UserSetColorTypes.Contains(ColorType.Red))
            {
                _greenTopLit.SetActive(true);
                _blueBottomLit.SetActive(true);
            }
        }
        else
        {
            if (UserSetColorTypes.Contains(ColorType.Red) && !(UserSetColorTypes.Contains(ColorType.Green) || UserSetColorTypes.Contains(ColorType.Blue)))
            {
                _redFullUnLit.SetActive(true);
            }
            else if (UserSetColorTypes.Contains(ColorType.Green) && !(UserSetColorTypes.Contains(ColorType.Red) || UserSetColorTypes.Contains(ColorType.Blue)))
            {
                _greenFullUnLit.SetActive(true);
            }
            else if (UserSetColorTypes.Contains(ColorType.Blue) && !(UserSetColorTypes.Contains(ColorType.Red) || UserSetColorTypes.Contains(ColorType.Green)))
            {
                _blueFullUnLit.SetActive(true);
            }
            else if (UserSetColorTypes.Contains(ColorType.Red) && UserSetColorTypes.Contains(ColorType.Green) && !UserSetColorTypes.Contains(ColorType.Blue))
            {
                _redTopUnLit.SetActive(true);
                _greenBottomUnLit.SetActive(true);
            }
            else if (UserSetColorTypes.Contains(ColorType.Red) && UserSetColorTypes.Contains(ColorType.Blue) && !UserSetColorTypes.Contains(ColorType.Green))
            {
                _redTopUnLit.SetActive(true);
                _blueBottomUnLit.SetActive(true);
            }
            else if (UserSetColorTypes.Contains(ColorType.Green) && UserSetColorTypes.Contains(ColorType.Blue) && !UserSetColorTypes.Contains(ColorType.Red))
            {
                _greenTopUnLit.SetActive(true);
                _blueBottomUnLit.SetActive(true);
            }
        }
    }

    #endregion Helper Methods (end)
    #endregion Methods (end)
}