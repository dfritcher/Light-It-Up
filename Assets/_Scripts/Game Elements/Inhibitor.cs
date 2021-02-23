using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using System.Linq;

public class Inhibitor : PowerableBase
{
    #region Fields, Properties
    #region Populated in Scene
    [Header("Populated In Scene")]
    [SerializeField, FormerlySerializedAs("_userSetColorType")]
    private List<ColorType> _userSetColorTypes = new List<ColorType>();
    private List<ColorType> UserSetColorTypes { get { return _userSetColorTypes; } }
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
    [Header("Populated by Code"), Space (8)]
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
    [Space(8),Header("Populated by Prefab")]
    [SerializeField]
    private Image _lockedIcon = null;
    [SerializeField]
    private Button _selectInhibitorOption = null;
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
    #endregion Populated by Prefab (end)

    #endregion Fields, Properties (end)

    #region Delegates, Events
    public delegate void InhibitorEvent(Inhibitor inhibitor);
    public event InhibitorEvent OnClick;
    #endregion Delegates, Events (end)

    #region Methods
    #region Unity Hooks
    protected override void Awake()
    {
        base.Awake();
        _selectInhibitorOption.interactable = _isClickable;
        _lockedIcon.gameObject.SetActive(!_isClickable);
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
                InhibitorClicked();
            }
        }
#endif

#if UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            var response = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.touches[0].position), Vector2.zero, 100f);

            if (response.transform == transform)
            {
                InhibitorClicked();
            }
        }
#endif
    }
    #endregion Unity Hooks (end)

    #region Overrides 
    public override void DetermineNewPowerState(PowerableBase powerableBase, bool checkDirection = false)
    {
        var inputDirection = _powerSources.Find(ps => ps.Powerable == powerableBase)?.InputDirection;
        GetPeerPower(powerableBase, inputDirection, checkDirection);
        UpdateBulbs();
        UpdateColorDisplay();
        UpdateWires();
    }

    public override List<Power> GetPowers(PowerableBase requestor)
    {
        if(IsPowered)
            return GetPoweredColors(requestor);
        else
            return _emptyPower;
    }

    public override bool GetPoweredState(PowerableBase requestor)
    {
        //var inputDirection = _powerSources.Find(ps => ps.Powerable == requestor)?.InputDirection;
        //var newPoweredState = false;        
        //foreach (var source in _powerSources)
        //{
        //    if (source.InputDirection != inputDirection && !(source.Powerable is Bulb))
        //        newPoweredState = source.Powerable.GetPoweredState(this);
        //    if (newPoweredState)
        //        break;
        //}
        //return newPoweredState;        
        return _isPowered;
    }

    public override void ResetPowerable()
    {
        _power.ColorTypes = new List<ColorType>(_originalColorTypes);
        _userSetColorTypes = new List<ColorType>(_originalColorTypes);
        UpdateColorDisplay();
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

    public override void GetBatteryPowerState(PowerableBase powerableBase)
    {
        _triggeringComponent = powerableBase;
        StartCoroutine(GetBatteryPowerStateCoroutine(powerableBase));
    }

    private IEnumerator GetBatteryPowerStateCoroutine(PowerableBase powerableBase)
    {
        var inputDirection = _powerSources.Find(ps => ps.Powerable == _triggeringComponent)?.InputDirection;
        // First wave through
        GetBatteryPowerAndPropagate(powerableBase, inputDirection);
        yield return null;       
    }

    #endregion Overrides (end)
    public void Setup()
    {

    }

    public void InhibitorClicked()
    {
        if (_isClickable)
        {
            OnClick?.Invoke(this);
            SetSelectedState(true);
        }            
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

    public void SetSelectedState(bool selected)
    {
        _selectedSprite.gameObject.SetActive(selected);
    }

    #region Helper Methods 
    
    private List<Power> GetPoweredColors(PowerableBase requestor)
    {
        var passingPowers = new List<Power>();
        var poweredColors = new List<Power>();

        var direction = _powerSources.Find(ps => ps.Powerable == requestor)?.InputDirection;

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
                if (!UserSetColorTypes.Contains(color))
                {
                    passingColors.Add(color);
                }
            }
            if (passingColors.Count > 0)
                passingPowers.Add(new Power() { Amount = power.Amount, ColorTypes = passingColors });
        }

        return passingPowers;
    }

    private void GetPeerPower(PowerableBase powerableBase, Direction? inputDirection, bool checkDirection = false)
    {
        var newColors = new List<ColorType>();
        //Get our powered colors from batteries
        for (int i = 0; i < Batteries.Length; i++)
        {
            if (Batteries[i].Powerable.IsPowered && Batteries[i].Powerable.CurrentColorTypes.Any(c => !UserSetColorTypes.Contains(c)))
            {
                foreach (var color in Batteries[i].Powerable.CurrentColorTypes)
                {
                    if (!UserSetColorTypes.Contains(color))
                    {
                        newColors.Add(color);
                    }
                }
            }
        }
        // Get our powered colors from other Inhibitors
        for (int i = 0; i < Inhibitors.Length; i++)
        {
            if (checkDirection)
                if(Inhibitors[i].InputDirection != inputDirection)//skip the guy who is telling us to update
                    continue;
            if (Inhibitors[i].Powerable.IsPowered && Inhibitors[i].Powerable.CurrentColorTypes.Any(c => !UserSetColorTypes.Contains(c)))
            {
                foreach (var color in Inhibitors[i].Powerable.CurrentColorTypes)
                {
                    if (!UserSetColorTypes.Contains(color))
                    {
                        newColors.Add(color);
                    }
                }
            }
        }

        // Get our powered colors from other Passthroughs
        for (int i = 0; i < PassThroughs.Length; i++)
        {
            if (checkDirection)
                if(PassThroughs[i].InputDirection != inputDirection) //skip the guy who is telling us to update
                    continue;
            if (PassThroughs[i].Powerable.IsPowered && PassThroughs[i].Powerable.CurrentColorTypes.Any(c => !UserSetColorTypes.Contains(c)))
            {
                foreach (var color in PassThroughs[i].Powerable.CurrentColorTypes)
                {
                    if (!UserSetColorTypes.Contains(color))
                    {
                        newColors.Add(color);
                    }
                }
            }
        }

        if (newColors.Except(_power.ColorTypes).Any() || _power.ColorTypes.Except(newColors).Any())
        {
            _power.ColorTypes.Clear();
            _power.ColorTypes.AddRange(newColors);

            GetBatteryPowerState(this);
            // Tell all Inhibitors to get their power state from peers
            for (int i = 0; i < Inhibitors.Length; i++)
            {
                if (Inhibitors[i].Powerable == powerableBase || Inhibitors[i].InputDirection == inputDirection) //skip the guy who is telling us to update
                    continue;
                Inhibitors[i].Powerable.DetermineNewPowerState(this, true);
            }

            // Tell all PassThroughs to get their power state from peers
            for (int i = 0; i < PassThroughs.Length; i++)
            {
                if (PassThroughs[i].Powerable == powerableBase || PassThroughs[i].InputDirection == inputDirection) //skip the guy who is telling us to update
                    continue;
                PassThroughs[i].Powerable.DetermineNewPowerState(this, true);
            }
            GetBatteryPowerState(this);
        }       
    }

    private void GetBatteryPowerAndPropagate(PowerableBase powerableBase, Direction? inputDirection)
    {
        _previousPowerState = _isPowered;
        _poweredBySource = false;
        // Get our new powered state from batteries. 
        for (int i = 0; i < Batteries.Length; i++)
        {
            if (Batteries[i].Powerable.IsPowered && Batteries[i].Powerable.CurrentColorTypes.Any(c => !UserSetColorTypes.Contains(c)))
            {
                _poweredBySource = true;
            }
        }

        // Get our powred state from other Inhibitors
        for (int i = 0; i < Inhibitors.Length; i++)
        {
            if (Inhibitors[i].InputDirection != inputDirection && Inhibitors[i].Powerable.IsPowered && Inhibitors[i].Powerable.CurrentColorTypes.Any(c => !UserSetColorTypes.Contains(c)))
            {
                _poweredBySource = true;               
            }
        }

        // Get our powered state from other Passthroughs
        for (int i = 0; i < PassThroughs.Length; i++)
        {
            if (PassThroughs[i].InputDirection != inputDirection && PassThroughs[i].Powerable.IsPowered && PassThroughs[i].Powerable.CurrentColorTypes.Any(c => !UserSetColorTypes.Contains(c)))
            {
                _poweredBySource = true;                
            }
        }

        _isPowered = _currentPowerState = _poweredBySource;

        if (_previousPowerState != _currentPowerState)
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

    private void UpdateWires()
    {
        foreach (var wire in _wires)
        {
            wire.GetBatteryPowerState(this);
        }
    }

    private void UpdateBulbs()
    {
        for (int i = 0; i < Bulbs.Length; i++)
        {
            Bulbs[i].GetBatteryPowerState(this);
        }
    }

    #endregion Helper Methods (end)
    #endregion Methods (end)
}