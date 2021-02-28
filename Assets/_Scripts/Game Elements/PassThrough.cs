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
    /// Reference to all the Batteries providing us power and the direction the power is coming from.
    /// This will help us determine which directions power is flowing when asked for our colors.
    /// </summary>
    [SerializeField]
    private List<PowerSource> _powerSources = null;

    private PowerSource[] _batteries = null;
    private PowerSource[] Batteries { get { return _batteries ?? (_batteries = _powerSources.FindAll(ps => ps.Powerable is Battery).ToArray()); } }

    private PowerSource[] _inhibitors = null;
    private PowerSource[] Inhibitors { get { return _inhibitors ?? (_inhibitors = _powerSources.FindAll(ps => ps.Powerable is Inhibitor).ToArray()); } }

    private PowerSource[] _passthroughs = null;
    private PowerSource[] PassThroughs { get { return _passthroughs ?? (_passthroughs = _powerSources.FindAll(ps => ps.Powerable is PassThrough).ToArray()); } }

    private Bulb[] _bulbs = null;
    private Bulb[] Bulbs { get { return _bulbs ?? (_bulbs = _powerSources.FindAll(ps => ps.Powerable is Bulb).Select(ps => ps.Powerable as Bulb).ToArray()); } }

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

    [SerializeField, ReadOnly(true)]
    private bool _isPoweredFromOtherSide = false;

    //public override List<ColorType> CurrentColorTypes { get { return _powerSources.Where(ps => !(ps.Powerable is Bulb)).SelectMany(ps => ps.ProvidedColors).ToList(); } }
    public override List<ColorType> CurrentColorTypes { get { return GetProvidedColors(); } }
    private List<ColorType> _providedColors = new List<ColorType>();
    private List<ColorType> GetProvidedColors()
    {
        _providedColors.Clear();
        foreach (var source in _powerSources)
        {
            foreach (var color in source.ProvidedColors)
            {
                if (_providedColors.Contains(color))
                    continue;
                _providedColors.Add(color);
            }
        }
        var providedColors = _powerSources.Where(ps => !(ps.Powerable is Bulb)).SelectMany(ps => ps.ProvidedColors).ToList();
        return _providedColors;
    }


    private List<Power> _emptyPower = new List<Power>();

    private bool _poweredBySource = false;
    private bool _previousPowerState = false;
    private bool _currentPowerState = false;
    private bool _stateChanged = false;
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
        SetSelectedState(false);
    }

    private void Start()
    {
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
    public override void DetermineNewPowerState(PowerableBase powerableBase, bool checkDirection = false)
    {
        GetPeerPower(powerableBase);
        //UpdateBulbs();        
        //UpdateWires();
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
        _power.ColorTypes = new List<ColorType>(_originalColorTypes);
        _userSetColorTypes = new List<ColorType>(_originalColorTypes);
        UpdateColorDisplay();
    }

    public override List<ColorType> GetOtherSideColors(PowerableBase requestor)
    {
        var oppositeColors = new List<ColorType>();
        var inputDirection = _powerSources.Find(ps => ps.Powerable == requestor).InputDirection;
        foreach (var source in _powerSources)
        {
            if (source.InputDirection == inputDirection || source.Powerable is Bulb)
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

    public bool IsPoweredFromOtherSide(PowerableBase requestor)
    {
        var inputDirection = _powerSources.Find(ps => ps.Powerable == requestor).InputDirection;
        _isPoweredFromOtherSide = false;
        foreach (var source in _powerSources)
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
    public override void GetBatteryPowerState(PowerableBase powerableBase)
    {
        StartCoroutine(GetBatteryPowerStateCoroutine(powerableBase));
    }

    private IEnumerator GetBatteryPowerStateCoroutine(PowerableBase powerableBase)
    {
        _previousPowerState = _isPowered;
        _poweredBySource = false;
        // Get our new powered state from everyone else. 
        // Start with batteries. 
        for (int i = 0; i < Batteries.Length; i++)
        {
            if (Batteries[i].Powerable.IsPowered && Batteries[i].Powerable.CurrentColorTypes.Any(c => UserSetColorTypes.Contains(c)))
            {
                _poweredBySource = true;
            }
        }
        // Get our powered state from other Inhibitors
        foreach (var inhibitorSource in Inhibitors)
        {
            var inhibitor = inhibitorSource.Powerable as Inhibitor;
            if (inhibitor.IsPoweredFromOtherSide(this) && inhibitor.CurrentColorTypes.Any(c => UserSetColorTypes.Contains(c)))
            {
                _poweredBySource = true;
            }
        }
        // Get our powered state from other PassThroughs 
        foreach (var passthroughSource in PassThroughs)
        {
            var passthrough = passthroughSource.Powerable as PassThrough;
            if (passthrough.IsPoweredFromOtherSide(this) && passthrough.CurrentColorTypes.Any(c => UserSetColorTypes.Contains(c)))
            {
                _poweredBySource = true;
            }
        }
        _isPowered = _currentPowerState = _poweredBySource;       

        if (_previousPowerState != _currentPowerState)
        {
            _stateChanged = true;            
        }        
        yield return null;        
    }

    #endregion Overrides (end)

    public void Setup()
    {

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
        yield return null;
        DetermineNewPowerState();
        yield return null;
        DetermineNewPowerColors();
        TriggerPropagation();
        yield return null;
        
        if (playAudio)
            PlayAudio();
    }

    public override void SetPowerStateOff(PowerableBase requestor)
    {
        _isPowered = false;
        var inputDirection = _powerSources.Find(ps => ps.Powerable == requestor)?.InputDirection;
        foreach (var source in _powerSources)
        {
            if (source == null || source.Powerable == null || source.Powerable == requestor || source.Powerable is Bulb || source.InputDirection == inputDirection)
                continue;
            source.Powerable.SetPowerStateOff(this);
        }
    }

    public override void CheckStateChanged()
    {
        if (_stateChanged)
        {
            _stateChanged = false;
            TriggerPropagation();
        }
        UpdateColorDisplay();
    }

    #region Helper Methods

    private void GetPeerPower(PowerableBase powerableBase)
    {
        var source = _powerSources.Find(ps => ps.Powerable == powerableBase);
        var previousColors = new List<ColorType>(_power.ColorTypes);
        if (powerableBase is Battery)
        {
            source.ProvidedColors.Clear();
            _power.ColorTypes.Clear();

            if (powerableBase.IsPowered && powerableBase.CurrentColorTypes.Any(c => UserSetColorTypes.Contains(c)))
            {
                foreach (var color in powerableBase.CurrentColorTypes)
                {
                    if (UserSetColorTypes.Contains(color) && !_power.ColorTypes.Contains(color))
                    {
                        _power.ColorTypes.Add(color);
                    }
                }
                source.ProvidedColors.AddRange(_power.ColorTypes);
            }
        }
        else if (powerableBase is Inhibitor || powerableBase is PassThrough)
        {
            source.ProvidedColors.Clear();
            _power.ColorTypes.Clear();
            if (powerableBase.IsPowered && powerableBase.CurrentColorTypes.Any(c => UserSetColorTypes.Contains(c)))
            {
                var colors = source.Powerable.GetOtherSideColors(this);
                foreach (var color in colors)
                {
                    if (UserSetColorTypes.Contains(color) && !_power.ColorTypes.Contains(color))
                    {
                        _power.ColorTypes.Add(color);
                    }
                }
                source.ProvidedColors.AddRange(_power.ColorTypes);
            }
        }

        if (previousColors.Except(_power.ColorTypes).Any() || _power.ColorTypes.Except(previousColors).Any())
        {
            _stateChanged = true;             
        }
    }

    private List<Power> GetPoweredColors(PowerableBase requestor)
    {
        var poweredColors = new List<Power>();

        var direction = _powerSources.Find(ps => ps.Powerable == requestor)?.InputDirection;
        if (direction == null && !(requestor is Wire))
            throw new System.Exception($"Direction is null in {this.name} for {requestor.name}.");

        foreach (var source in _powerSources)
        {
            if (source == null || source.Powerable == null || source.Powerable == requestor || source.Powerable is Bulb)
                continue;
            if (source.Powerable.IsPowered && source.InputDirection != direction)
            {
                poweredColors.Add(new Power() { Amount = 1, ColorTypes = source.ProvidedColors });
            }
        }

        return poweredColors;
    }

    private void DetermineNewPowerState()
    {
        // Get our new powered state from batteries. 
        for (int i = 0; i < Batteries.Length; i++)
        {
            if (Batteries[i].Powerable.IsPowered && Batteries[i].Powerable.CurrentColorTypes.Any(c => UserSetColorTypes.Contains(c)))
            {
                _poweredBySource = true;
            }
        }

        // Get our powred state from other Inhibitors
        for (int i = 0; i < Inhibitors.Length; i++)
        {
            if (Inhibitors[i].Powerable.IsPowered && Inhibitors[i].Powerable.CurrentColorTypes.Any(c => UserSetColorTypes.Contains(c)))
            {
                _poweredBySource = true;
            }
        }

        // Get our powered state from other Passthroughs
        for (int i = 0; i < PassThroughs.Length; i++)
        {
            if (PassThroughs[i].Powerable.IsPowered && PassThroughs[i].Powerable.CurrentColorTypes.Any(c => UserSetColorTypes.Contains(c)))
            {
                _poweredBySource = true;
            }
        }

        _isPowered = _currentPowerState = _poweredBySource;
    }

    private void DetermineNewPowerColors()
    {
        _power.ColorTypes.Clear();
        for (int i = 0; i < Batteries.Length; i++)
        {
            Batteries[i].ProvidedColors.Clear();
            if (Batteries[i].Powerable.IsPowered && Batteries[i].Powerable.CurrentColorTypes.Any(c => UserSetColorTypes.Contains(c)))
            {
                foreach (var color in Batteries[i].Powerable.CurrentColorTypes)
                {
                    if (UserSetColorTypes.Contains(color))
                    {
                        Batteries[i].ProvidedColors.Add(color);
                    }
                }
            }
        }
        // Get our powered colors from other Inhibitors
        for (int i = 0; i < Inhibitors.Length; i++)
        {
            Inhibitors[i].ProvidedColors.Clear();
            if (Inhibitors[i].Powerable.IsPowered && Inhibitors[i].Powerable.CurrentColorTypes.Any(c => UserSetColorTypes.Contains(c)))
            {
                foreach (var color in Inhibitors[i].Powerable.CurrentColorTypes)
                {
                    if (UserSetColorTypes.Contains(color))
                    {
                        Inhibitors[i].ProvidedColors.Add(color);
                    }
                }
            }
        }

        // Get our powered colors from other Passthroughs
        for (int i = 0; i < PassThroughs.Length; i++)
        {
            PassThroughs[i].ProvidedColors.Clear();
            if (PassThroughs[i].Powerable.IsPowered && PassThroughs[i].Powerable.CurrentColorTypes.Any(c => UserSetColorTypes.Contains(c)))
            {
                foreach (var color in PassThroughs[i].Powerable.CurrentColorTypes)
                {
                    if (UserSetColorTypes.Contains(color))
                    {
                        PassThroughs[i].ProvidedColors.Add(color);
                    }
                }
            }
        }
    }

    private void TriggerPropagation()
    {
        TriggerPowerCheckPropagation();
        TriggerPowerStatePropagation();
        TriggerCheckStateChangePropagation();
        CheckStateChanged();
        UpdateColorDisplay();
    }

    private void TriggerPowerCheckPropagation()
    {
        // Tell all Inhibitors to check their power state based on batteries
        for (int i = 0; i < Inhibitors.Length; i++)
        {
            Inhibitors[i].Powerable.GetBatteryPowerState(this);
        }

        // Tell all PassThroughs to check their power state based on batteries
        for (int i = 0; i < PassThroughs.Length; i++)
        {
            PassThroughs[i].Powerable.GetBatteryPowerState(this);
        }        
    }

    private void TriggerPowerStatePropagation()
    {
        // Tell all Inhibitors to check their power state based on batteries
        for (int i = 0; i < Inhibitors.Length; i++)
        {
            Inhibitors[i].Powerable.DetermineNewPowerState(this);
        }

        // Tell all PassThroughs to check their power state based on batteries
        for (int i = 0; i < PassThroughs.Length; i++)
        {
            PassThroughs[i].Powerable.DetermineNewPowerState(this);
        }
        for (int i = 0; i < Bulbs.Length; i++)
        {
            Bulbs[i].DetermineNewPowerState(this);
        }
        foreach (var wire in _wires)
        {
            wire.DetermineNewPowerState(this);
        }
    }

    private void TriggerCheckStateChangePropagation()
    {
        // Tell all Inhibitors to check their power state based on batteries
        for (int i = 0; i < Inhibitors.Length; i++)
        {
            Inhibitors[i].Powerable.CheckStateChanged();
        }

        // Tell all PassThroughs to check their power state based on batteries
        for (int i = 0; i < PassThroughs.Length; i++)
        {
            PassThroughs[i].Powerable.CheckStateChanged();
        }
        foreach (var wire in _wires)
        {
            wire.CheckStateChanged();
        }
        for (int i = 0; i < Bulbs.Length; i++)
        {
            Bulbs[i].CheckStateChanged();
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