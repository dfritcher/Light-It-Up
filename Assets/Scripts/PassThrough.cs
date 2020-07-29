﻿using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
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
    
    [SerializeField] //DEPRECATED - Used for prototype visuals. Displayed colors the user chose.
    private List<Image> _passThroughColors = null;

    [SerializeField]
    private List<Wire> _wires = null;

    [SerializeField]
    private List<Junction> _junctions = null;

    [SerializeField]
    private bool _isClickable = true;
    public override bool IsClickable { get { return _isClickable; } }

    [Space(8),SerializeField, ReadOnly(true)]//DEPRECATED, place bulbs in _powerSources.
    private List<PowerSource> _poweredBulbs = null;

    #endregion Populated in Scene (end)

    #region Populated by Code
    [Header("Populated by Code"), Space(8)]
    [SerializeField]
    private bool _isPowered = false;
    public override bool IsPowered { get { return _isPowered; } }

    [SerializeField]
    private List<ColorType> _currentColorTypes = null;
    public override List<ColorType> CurrentColorTypes { get { return _currentColorTypes ?? (_currentColorTypes = new List<ColorType>()); } }    

    private List<Power> _emptyPower = new List<Power>();
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
        UpdatePowerState(null);
    }

    private void Start()
    {
        GetPoweredState(null);
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
    public void Setup()
    {
        
    }

    public override void ResetPowerable()
    {
        _currentColorTypes = new List<ColorType>(_originalColorTypes);
        _userSetColorTypes = new List<ColorType>(_originalColorTypes);
        UpdateColorDisplay();
    }
    
    public void PassThroughClicked()
    {
        if (_isClickable)
            OnClick?.Invoke(this);
    }

    public void SetUserSelectedPower(List<ColorType> colorTypes)
    {
        _userSetColorTypes = colorTypes;
        UpdateColorDisplay();
        UpdatePowerState(this);
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

    public override List<Power> GetPowers(PowerableBase requestor)
    {
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
            if (source == null || source.Powerable == null || source.Powerable == requestor)
                continue;
            if (source.Powerable.GetPoweredState(this) && source.InputDirection != direction)
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
                if (_userSetColorTypes.Contains(color))
                {
                    passingColors.Add(color);
                }
            }
            if (passingColors.Count > 0)
                passingPowers.Add(new Power() { Amount = power.Amount, ColorTypes = passingColors });
        }

        return passingPowers;
    }

    /// <summary>
    /// Method to have an external powersource tell us they were updated.
    /// </summary>
    /// <param name="powerableBase"></param>
    public override void UpdatePowerState(PowerableBase powerableBase)
    {
        CheckPoweredState(powerableBase);
        _currentColorTypes.Clear();
        foreach(var source in _powerSources)
        {
            if (source.Powerable == null)
                continue;
            foreach(var color in source.Powerable.CurrentColorTypes)
            {
                if (_userSetColorTypes.Contains(color))
                {
                    _currentColorTypes.Add(color);
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

        //foreach (var junction in _junctions)
        //{
        //    junction.UpdatePowerState(this);
        //}

        var inputDirection = _powerSources.Find(ps => ps.Powerable == powerableBase)?.InputDirection;

        //foreach (var bulb in _poweredBulbs)
        //{
        //    if(inputDirection != bulb.InputDirection)
        //        bulb.Powerable.UpdatePowerState(this);
        //}
    }

    private void CheckPoweredState(PowerableBase powerableBase)
    {
        var isPowered = false;
        var inputDirection = _powerSources.Find(ps => ps.Powerable == powerableBase)?.InputDirection;
        foreach (var source in _powerSources)
        {
            if (source == null || source.Powerable == null)
                continue;

            if(source.InputDirection != inputDirection)
                isPowered = source.Powerable.GetPoweredState(this);
            if (isPowered)
                break;
        }
        _isPowered = isPowered;
    }

    /// <summary>
    /// Method to determine if this powerable is getting power from another source.
    /// </summary>
    /// <param name="requestor"></param>
    /// <returns></returns>
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