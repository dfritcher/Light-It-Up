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

    public override List<ColorType> CurrentColorTypes { get { return _power.ColorTypes; } }    

    private List<Power> _emptyPower = new List<Power>();

    private bool _poweredBySource = false;
    private bool _previousPowerState = false;
    private bool _currentPowerState = false;

    private PowerableBase _triggeringComponent = null;
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
        GetBatteryPowerState(null);
        SetSelectedState(false);
    }

    private void Start()
    {
        //GetPoweredState(null);
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
    public override void DetermineNewPowerState(PowerableBase powerableBase)
    {
        var inputDirection = _powerSources.Find(ps => ps.Powerable == powerableBase)?.InputDirection;
        GetPeerPower(powerableBase, inputDirection);
        UpdateBulbs();
        UpdateColorDisplay();
        UpdateWires();
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

    /// <summary>
    /// Method to have an external powersource tell us they were updated.
    /// </summary>
    /// <param name="powerableBase"></param>
    public override void GetBatteryPowerState(PowerableBase powerableBase)
    {
        _triggeringComponent = powerableBase;
        StartCoroutine(GetBatteryPowerStateCoroutine(powerableBase));
    }

    private IEnumerator GetBatteryPowerStateCoroutine(PowerableBase powerableBase)
    {
        var inputDirection = _powerSources.Find(ps => ps.Powerable == powerableBase)?.InputDirection;
        GetBatteryPowerAndPropagate(powerableBase, inputDirection);
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
        GetBatteryPowerState(this);
        yield return null;
        DetermineNewPowerState(this);
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


    #region Helper Methods

    private void GetPeerPower(PowerableBase powerableBase, Direction? inputDirection)
    {
        var newColors = new List<ColorType>();
        // Get our powered colors from batteries
        for (int i = 0; i < Batteries.Length; i++)
        {
            if (Batteries[i].Powerable.IsPowered && Batteries[i].Powerable.CurrentColorTypes.Any(c => UserSetColorTypes.Contains(c)))
            {
                foreach (var color in Batteries[i].Powerable.CurrentColorTypes)
                {
                    if (Batteries[i].Powerable.IsPowered && UserSetColorTypes.Contains(color))
                    {
                        newColors.Add(color);
                    }
                }
            }
        }

        // Get our powered colors from other Inhibitors
        for (int i = Inhibitors.Length - 1; i >= 0; i--)
        {
            if (Inhibitors[i].Powerable.IsPowered && Inhibitors[i].Powerable.CurrentColorTypes.Any(c => UserSetColorTypes.Contains(c)))
            {
                //_initiallyPowered = true;
                foreach (var color in Inhibitors[i].Powerable.CurrentColorTypes)
                {
                    if (UserSetColorTypes.Contains(color))
                    {
                        newColors.Add(color);
                    }
                }
            }
        }

        // Get our powered colors from other PassThroughs 
        for (int i = PassThroughs.Length - 1; i >= 0; i--)
        {
            if (PassThroughs[i].Powerable.IsPowered && PassThroughs[i].Powerable.CurrentColorTypes.Any(c => UserSetColorTypes.Contains(c)))
            {
                foreach (var color in PassThroughs[i].Powerable.CurrentColorTypes)
                {
                    if (UserSetColorTypes.Contains(color))
                    {
                        newColors.Add(color);
                    }
                }
            }
        }

        if (_previousPowerState != _currentPowerState || newColors.Except(_power.ColorTypes).Any())
        {
            _power.ColorTypes.Clear();
            _power.ColorTypes.AddRange(newColors);
            // if our power state changed after initial check, tell everyone to check again.
            GetBatteryPowerState(powerableBase);         
            for (int i = 0; i < Inhibitors.Length; i++)
            {
                if (Inhibitors[i].Powerable == powerableBase || Inhibitors[i].InputDirection == inputDirection) //skip the guy who is telling us to update
                    continue;
                Inhibitors[i].Powerable.DetermineNewPowerState(this);
            }

            // Tell all PassThroughs to get their power state from peers
            for (int i = 0; i < PassThroughs.Length; i++)
            {
                if (PassThroughs[i].Powerable == powerableBase || PassThroughs[i].InputDirection == inputDirection) //skip the guy who is telling us to update
                    continue;
                PassThroughs[i].Powerable.DetermineNewPowerState(this);
            }
        }                             
    }

    private void GetBatteryPowerAndPropagate(PowerableBase powerableBase, Direction? inputDirection)
    {
        _previousPowerState = _isPowered;
        // Get our new powered state from everyone else. 
        // Start with batteries. 
        for (int i = 0; i < Batteries.Length; i++)
        {
            if (Batteries[i].Powerable.IsPowered) //&& Batteries[i].Powerable.CurrentColorTypes.Any(c => UserSetColorTypes.Contains(c)))
            {
                _poweredBySource = true;               
            }
        }

        // Get our powred state from other Inhibitors
        for (int i = Inhibitors.Length - 1; i >= 0; i--)
        {
            if (Inhibitors[i].Powerable.IsPowered) //&& Inhibitors[i].Powerable.CurrentColorTypes.Any(c => UserSetColorTypes.Contains(c)))
            {
                _poweredBySource = true;                
            }
        }

        // Get our powred state from other PassThroughs 
        for (int i = PassThroughs.Length - 1; i >= 0; i--)
        {
            if (PassThroughs[i].Powerable.IsPowered)// && PassThroughs[i].Powerable.CurrentColorTypes.Any(c => UserSetColorTypes.Contains(c)))
            {
                _poweredBySource = true;                
            }
        }

        _isPowered = _currentPowerState = _poweredBySource;

        if(_previousPowerState != _currentPowerState)
        {
            // Tell all Inhibitors to check their power state based on batteries
            for (int i = 0; i < Inhibitors.Length; i++)
            {
                if (Inhibitors[i].Powerable == powerableBase || Inhibitors[i].InputDirection == inputDirection) //skip the guy who is telling us to update
                    continue;
                Inhibitors[i].Powerable.GetBatteryPowerState(this);
            }

            // Tell all PassThroughs to check their power state based on batteries
            for (int i = 0; i < PassThroughs.Length; i++)
            {
                if (PassThroughs[i].Powerable == powerableBase || PassThroughs[i].InputDirection == inputDirection) //skip the guy who is telling us to update
                    continue;
                PassThroughs[i].Powerable.GetBatteryPowerState(this);
            }
        }        
    }

    private List<Power> GetPoweredColors(PowerableBase requestor)
    {
        var passingPowers = new List<Power>();
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
                poweredColors.AddRange(source.Powerable.GetPowers(this));
            }
        }

        foreach (var power in poweredColors)
        {
            var passingColors = new List<ColorType>();
            foreach (var color in power.ColorTypes)
            {
                //Pass all colors that we don't block
                if (UserSetColorTypes.Contains(color))
                {
                    passingColors.Add(color);
                }
            }
            if (passingColors.Count > 0)
                passingPowers.Add(new Power() { Amount = power.Amount, ColorTypes = passingColors });
        }

        return passingPowers;
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

    private void UpdateBulbs()
    {
        for (int i = 0; i < Bulbs.Length; i++)
        {
            Bulbs[i].GetBatteryPowerState(this);
        }
    }

    private void UpdateWires()
    {
        foreach (var wire in _wires)
        {
            wire.GetBatteryPowerState(this);
        }
    }
    
    #endregion Helper Methods (end)
    #endregion Methods (end)
}