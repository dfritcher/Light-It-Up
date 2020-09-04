using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Bulb : PowerableBase
{
    #region Fields, Properties
    #region Populated in Scene
    [Header("Populated In Scene")]
    [SerializeField]
    private List<Image> _bulbColors = null;

    [SerializeField]
    private TextMeshProUGUI _bulbPowerDisplay = null;

    [SerializeField]
    private TextMeshProUGUI _bulbStatusDisplay = null;

    [SerializeField]
    private int _minPower = 0;

    [SerializeField]
    private int _maxPower = 4;

    [SerializeField]
    private List<PowerableBase> _powerables = null;
    #endregion Populated in Scene (end)

    #region Populated by Code
    [Header("Populated by Code")]
    [SerializeField, Range(0, 4), ReadOnly(true)]
    private int _powerLevel = 0;
    public int CurrentPowerLevel { get { return _powerLevel; } }

    [SerializeField, ReadOnly(true)]
    private List<ColorType> _currentColorTypes = null;
    public override List<ColorType> CurrentColorTypes { get { return _currentColorTypes ?? (_currentColorTypes = new List<ColorType>()); } }

    [SerializeField]
    private bool _isBroken = false;
    public bool IsBroken { get { return _isBroken; } }

    [SerializeField]
    private bool _isOn = false;
    public bool IsOn { get { return _isOn; } }

    public override bool IsClickable => false;

    public override bool IsPowered { get { return false; } }
    #endregion Populated by Code (end)

    #region Populated By Prefab
    [Header("Populated by Prefab")]
    [Header("Lit References")]
    [SerializeField]
    private GameObject _redFullLit = null;
    [SerializeField]
    private GameObject _redTopLit = null;
    [SerializeField]
    private GameObject _redSmallTopLit = null;
    [SerializeField]
    private GameObject _greenFullLit = null;
    [SerializeField]
    private GameObject _greenTopLit = null;
    [SerializeField]
    private GameObject _greenSmallMiddleLit = null;
    [SerializeField]
    private GameObject _greenBottomLit = null;
    [SerializeField]
    private GameObject _blueFullLit = null;
    [SerializeField]
    private GameObject _blueBottomLit = null;
    [SerializeField]
    private GameObject _blueSmallBottomLit = null;

    [Header("UnLit References"), Space(5)]
    [SerializeField]
    private GameObject _redFullUnLit = null;
    [SerializeField]
    private GameObject _redTopUnLit = null;
    [SerializeField]
    private GameObject _redSmallTopUnLit = null;
    [SerializeField]
    private GameObject _greenFullUnLit = null;
    [SerializeField]
    private GameObject _greenTopUnLit = null;
    [SerializeField]
    private GameObject _greenSmallMiddleUnLit = null;
    [SerializeField]
    private GameObject _greenBottomUnLit = null;
    [SerializeField]
    private GameObject _blueFullUnLit = null;
    [SerializeField]
    private GameObject _blueBottomUnLit = null;
    [SerializeField]
    private GameObject _blueSmallBottomUnLit = null;

    [Header("Image References"), Space(5)]
    [SerializeField]
    private SpriteRenderer _bulbImage = null;
    [SerializeField]
    private Sprite _brokenSprite = null;
    [SerializeField]
    private Sprite _normalSprite = null;
    #endregion Populated by Prefab (end)

    #endregion Fields, Properties (end)

    #region Methods
    protected override void Awake()
    {
        base.Awake();
        ResetPowerable();        
    }
    
    public void Setup()
    {
        UpdateUI();
    }
    
    public void IncreasePower(int amount, bool updateDisplay = false)
    {
        _powerLevel = _powerLevel + amount;
        if (updateDisplay)
            UpdatePowerDisplay();
    }

    public void DecreasePower(int amount)
    {
        _powerLevel = _powerLevel - amount;
        if (_powerLevel < _minPower)
            _powerLevel = _minPower;

        UpdatePowerDisplay();
    }

    private void ResetPowerLevel()
    {
        _powerLevel = 0;
    }
   
    private void SetCurrentPower()
    {
        foreach(var powerable in _powerables)
        {
            var currentPower = powerable.GetPowers(this);
            foreach(var power in currentPower)
            {
                foreach(var color in power.ColorTypes)
                {
                    if (CurrentColorTypes.Contains(color))
                    {
                        IncreasePower(power.Amount); //TODO: This will need to change to grabbing the power level plus color coming from the source.
                        break;
                    }
                }                                    
            }
        }
        CheckState();
    }

    private void CheckState()
    {
        //Once we are broken we don't get fixed.
        if (_isBroken)
            return;
        _isBroken = _powerLevel > _maxPower;

        _isOn = _powerLevel == _maxPower && !_isBroken;
    }

    private void UpdateUI()
    {
        UpdatePowerDisplay();
        UpdateColorDisplay();
    }

    private void UpdatePowerDisplay()
    {
        if (_bulbStatusDisplay)
        {
            if (_powerLevel > _maxPower)
                _bulbStatusDisplay.text = $"BROKEN!";
            else
                _bulbStatusDisplay.text = string.Empty;
        }
        if (_bulbImage)
        {
            if (_powerLevel > _maxPower)
            {
                _bulbImage.sprite = _brokenSprite;
                DisableLitImages();
                DisableUnLitImages();
            }
                
        }
        _bulbPowerDisplay.text = $"{_powerLevel}/{_maxPower} ";
    }

    private void UpdateColorDisplay()
    {
        DisableLitImages();
        DisableUnLitImages();
        SetImagesByPowerLevel();
    }

    private void SetImagesByPowerLevel()
    {
        if (_isBroken)
            return;
        if (CurrentPowerLevel > 0)
        {
            if (CurrentColorTypes.Contains(ColorType.Red) && !(CurrentColorTypes.Contains(ColorType.Green) || CurrentColorTypes.Contains(ColorType.Blue)))
            {
                _redFullLit?.SetActive(true);
                _redFullUnLit?.SetActive(false);
            }
            else if (CurrentColorTypes.Contains(ColorType.Green) && !(CurrentColorTypes.Contains(ColorType.Red) || CurrentColorTypes.Contains(ColorType.Blue)))
            {
                _greenFullLit?.SetActive(true);
                _greenFullUnLit?.SetActive(false);
            }
            else if (CurrentColorTypes.Contains(ColorType.Blue) && !(CurrentColorTypes.Contains(ColorType.Red) || CurrentColorTypes.Contains(ColorType.Green)))
            {
                _blueFullLit?.SetActive(true);
                _blueFullUnLit?.SetActive(false);
            }
            else if (CurrentColorTypes.Contains(ColorType.Red) && CurrentColorTypes.Contains(ColorType.Green) && !CurrentColorTypes.Contains(ColorType.Blue))
            {
                _redTopLit?.SetActive(true);
                _greenBottomLit?.SetActive(true);
            }
            else if (CurrentColorTypes.Contains(ColorType.Red) && CurrentColorTypes.Contains(ColorType.Blue) && !CurrentColorTypes.Contains(ColorType.Green))
            {
                _redTopLit?.SetActive(true);
                _blueBottomLit?.SetActive(true);
            }
            else if (CurrentColorTypes.Contains(ColorType.Green) && CurrentColorTypes.Contains(ColorType.Blue) && !CurrentColorTypes.Contains(ColorType.Red))
            {
                _greenTopLit?.SetActive(true);
                _blueBottomLit?.SetActive(true);
            }
            else if (CurrentColorTypes.Contains(ColorType.Red) && CurrentColorTypes.Contains(ColorType.Green) && CurrentColorTypes.Contains(ColorType.Blue))
            {
                _redSmallTopLit?.SetActive(true);
                _greenSmallMiddleLit?.SetActive(true);
                _blueSmallBottomLit?.SetActive(true);
            }
        }
        else
        {
            if (CurrentColorTypes.Contains(ColorType.Red) && !(CurrentColorTypes.Contains(ColorType.Green) || CurrentColorTypes.Contains(ColorType.Blue)))
            {
                _redFullUnLit?.SetActive(true);
                _redFullLit?.SetActive(false);
            }
            else if (CurrentColorTypes.Contains(ColorType.Green) && !(CurrentColorTypes.Contains(ColorType.Red) || CurrentColorTypes.Contains(ColorType.Blue)))
            {
                _greenFullUnLit?.SetActive(true);
                _greenFullLit?.SetActive(false);
            }
            else if (CurrentColorTypes.Contains(ColorType.Blue) && !(CurrentColorTypes.Contains(ColorType.Red) || CurrentColorTypes.Contains(ColorType.Green)))
            {
                _blueFullUnLit?.SetActive(true);
                _blueFullLit?.SetActive(false);
            }
            else if (CurrentColorTypes.Contains(ColorType.Red) && CurrentColorTypes.Contains(ColorType.Green) && !CurrentColorTypes.Contains(ColorType.Blue))
            {
                _redTopUnLit?.SetActive(true);
                _greenBottomUnLit?.SetActive(true);
            }
            else if (CurrentColorTypes.Contains(ColorType.Red) && CurrentColorTypes.Contains(ColorType.Blue) && !CurrentColorTypes.Contains(ColorType.Green))
            {
                _redTopUnLit?.SetActive(true);
                _blueBottomUnLit?.SetActive(true);
            }
            else if (CurrentColorTypes.Contains(ColorType.Green) && CurrentColorTypes.Contains(ColorType.Blue) && !CurrentColorTypes.Contains(ColorType.Red))
            {
                _greenTopUnLit?.SetActive(true);
                _blueBottomUnLit?.SetActive(true);
            }
            else if (CurrentColorTypes.Contains(ColorType.Red) && CurrentColorTypes.Contains(ColorType.Green) && CurrentColorTypes.Contains(ColorType.Blue))
            {
                _redSmallTopUnLit?.SetActive(true);
                _greenSmallMiddleUnLit?.SetActive(true);
                _blueSmallBottomUnLit?.SetActive(true);
            }
        }
    }

    private void DisableUnLitImages()
    {
        _redFullUnLit?.SetActive(false);
        _redTopUnLit?.SetActive(false);
        _redSmallTopUnLit?.SetActive(false);
        _greenFullUnLit?.SetActive(false);
        _greenTopUnLit?.SetActive(false);
        _greenSmallMiddleUnLit?.SetActive(false);
        _greenBottomUnLit?.SetActive(false);
        _blueFullUnLit?.SetActive(false);
        _blueBottomUnLit?.SetActive(false);
        _blueSmallBottomUnLit?.SetActive(false);
    }

    private void DisableLitImages()
    {
        _redFullLit?.SetActive(false);
        _redTopLit?.SetActive(false);
        _redSmallTopLit?.SetActive(false);
        _greenFullLit?.SetActive(false);
        _greenTopLit?.SetActive(false);
        _greenSmallMiddleLit?.SetActive(false);
        _greenBottomLit?.SetActive(false);
        _blueFullLit?.SetActive(false);
        _blueBottomLit?.SetActive(false);
        _blueSmallBottomLit?.SetActive(false);
    }

    public override List<Power> GetPowers(PowerableBase requestor)
    {
        return new List<Power>() { new Power() { Amount = 0, ColorTypes = new List<ColorType>() { ColorType.None } } };
    }

    public override void ResetPowerable()
    {
        _currentColorTypes = new List<ColorType>(_originalColorTypes);
        _isBroken = false;
        _isOn = false;
        _bulbImage.sprite = _normalSprite;
        ResetPowerLevel();
        UpdateUI();        
    }

    public override void UpdatePowerState(PowerableBase powerableBase)
    {
        ResetPowerable();
        SetCurrentPower();
        UpdateUI();       
    }

    public override bool GetPoweredState(PowerableBase requestor)
    {
        return _powerLevel > 0;
    }
    #endregion Methods (end)
}
