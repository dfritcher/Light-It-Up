using System;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

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

    [SerializeField]
    private BulbType _bulbType;
    public BulbType BulbType { get { return _bulbType; } }

    [SerializeField]
    private bool _shouldUpdateVisuals = true;
    [SerializeField]
    private bool _shouldResetState = true;

    [Header("Animation")]
    [SerializeField]
    private Animator _animator = null;
    #endregion Populated in Scene (end)

    #region Populated by Code
    [Header("Populated by Code")]
    [SerializeField, Range(0, 4), ReadOnly(true)]
    private int _powerLevel = 0;
    public int CurrentPowerLevel { get { return _powerLevel; } }

    [SerializeField, ReadOnly(true)]
    private List<ColorType> _currentColorTypes = null;
    public List<ColorType> CurrentColorTypes { get { return _currentColorTypes ?? (_currentColorTypes = new List<ColorType>()); } }

    [SerializeField]
    private bool _isBroken = false;
    public bool IsBroken { get { return _isBroken; } }

    [SerializeField]
    private bool _isOn = false;
    public bool IsOn { get { return _isOn; } }

    public override bool IsClickable => false;

    public override bool IsPowered { get { return false; } }

    private List<Material> _originalSprites;
    private List<Material> _animatedSprites;
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

    [Header("Audio References"), Space(8)]
    [SerializeField]
    private AudioSource _bulbAudioSource = null;
    [SerializeField]
    private AudioClip _glassBreakClip = null;
    [SerializeField]
    private AudioClip _increasePowerClip = null;

    #endregion Populated by Prefab (end)

    #endregion Fields, Properties (end)

    #region Delegates, Events
    public delegate void BulbEvent(Bulb bulb);
    public event BulbEvent BrokenBulbAnimationEnd;
    #endregion Delegates, Events (end)

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

    public override void Setup(PowerableBase powerableBase)
    {
        try
        {
            //var source = _externalPowerSources.Find(ps => ps.Powerable == powerableBase);
            //if(source == null)
            //{
            //    Debug.LogError($"{name} could not find {powerableBase.name} in its External Power Sources.");
            //    return;
            //}                
        }
        catch(Exception ex)
        {
            Debug.LogError($"{ex.Message} Occurred in {name}");
        }
    }

    public void IncreasePower(int amount, bool updateDisplay = false)
    {
        _powerLevel += amount;
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

    public override List<Power> GetPowers(PowerableBase requestor)
    {
        throw new NotImplementedException();
    }

    public override void ResetPowerable()
    {
        if (!_shouldResetState)
            return;
        _currentColorTypes = new List<ColorType>(_originalColorTypes);
        _isBroken = false;
        _isOn = false;
        _bulbImage.sprite = _normalSprite;
        ResetPowerLevel();
        UpdateUI();
    }

    public override void DetermineNewPowerState(PowerableBase powerableSource)
    {
        //ResetPowerable();
        //SetCurrentPower();
        //UpdateUI();
    }

    public override bool GetPoweredState(PowerableBase requestor)
    {
        return _powerLevel > 0;
    }

    public override void SetColorTypes(List<ColorType> colorTypes)
    {
        base.SetColorTypes(colorTypes);
        _currentColorTypes = colorTypes;
    }

    public void BreakBulb()
    {
        _isBroken = true;
    }

    public void UpdateUI()
    {
        UpdatePowerDisplay();
        UpdateColorDisplay();
    }

    #region Broken Bulb Animation 
    public void ResetBrokenAnimation()
    {
        _isBroken = false;
        _animator.SetTrigger("ExitBroken");
        _bulbImage.sprite = _normalSprite;
    }
    
    public void OnBrokenBulbAnimationStart()
    {
        _isBroken = false;
        _animatedSprites = GetLitMaterials();
        _originalSprites = _animatedSprites.Select(t => new Material(t)).ToList();
        AudioManager.PlayOneShot(_increasePowerClip);
        //UpdateColorDisplay();
    }

    public void OnIncreaseIntensity()
    {
        //Red Bulb
        if (CurrentColorTypes.Contains(ColorType.Red) && !(CurrentColorTypes.Contains(ColorType.Green) || CurrentColorTypes.Contains(ColorType.Blue)))
        {
            _animatedSprites[0].SetColor("_Color", new Color(_animatedSprites[0].color.r + 1f, _animatedSprites[0].color.g, _animatedSprites[0].color.b, _animatedSprites[0].color.a));
        }
        //Green Bulb
        else if (CurrentColorTypes.Contains(ColorType.Green) && !(CurrentColorTypes.Contains(ColorType.Red) || CurrentColorTypes.Contains(ColorType.Blue)))
        {
            _animatedSprites[0].SetColor("_Color", new Color(_animatedSprites[0].color.r, _animatedSprites[0].color.g + 1f, _animatedSprites[0].color.b, _animatedSprites[0].color.a));
        }
        //Blue Bulb
        else if (CurrentColorTypes.Contains(ColorType.Blue) && !(CurrentColorTypes.Contains(ColorType.Red) || CurrentColorTypes.Contains(ColorType.Green)))
        {
            _animatedSprites[0].SetColor("_Color", new Color(_animatedSprites[0].color.r, _animatedSprites[0].color.g, _animatedSprites[0].color.b + 1f, _animatedSprites[0].color.a));
        }
        // Red Green Bulb
        else if (CurrentColorTypes.Contains(ColorType.Red) && CurrentColorTypes.Contains(ColorType.Green) && !CurrentColorTypes.Contains(ColorType.Blue))
        {
            _animatedSprites[0].SetColor("_Color", new Color(_animatedSprites[0].color.r + 1f, _animatedSprites[0].color.g, _animatedSprites[0].color.b, _animatedSprites[0].color.a));
            _animatedSprites[1].SetColor("_Color", new Color(_animatedSprites[1].color.r, _animatedSprites[1].color.g + 1f, _animatedSprites[1].color.b, _animatedSprites[1].color.a));
        }
        // Red Blue Bulb
        else if (CurrentColorTypes.Contains(ColorType.Red) && CurrentColorTypes.Contains(ColorType.Blue) && !CurrentColorTypes.Contains(ColorType.Green))
        {
            _animatedSprites[0].SetColor("_Color", new Color(_animatedSprites[0].color.r + 1f, _animatedSprites[0].color.g, _animatedSprites[0].color.b, _animatedSprites[0].color.a));
            _animatedSprites[1].SetColor("_Color", new Color(_animatedSprites[1].color.r, _animatedSprites[1].color.g, _animatedSprites[1].color.b + 1f, _animatedSprites[1].color.a));
        }
        // Green Blue Bulb
        else if (CurrentColorTypes.Contains(ColorType.Green) && CurrentColorTypes.Contains(ColorType.Blue) && !CurrentColorTypes.Contains(ColorType.Red))
        {
            _animatedSprites[0].SetColor("_Color", new Color(_animatedSprites[0].color.r, _animatedSprites[0].color.g + 1f, _animatedSprites[0].color.b, _animatedSprites[0].color.a));
            _animatedSprites[1].SetColor("_Color", new Color(_animatedSprites[1].color.r, _animatedSprites[1].color.g, _animatedSprites[1].color.b + 1f, _animatedSprites[1].color.a));
        }
        // All Three Colors
        else if (CurrentColorTypes.Contains(ColorType.Red) && CurrentColorTypes.Contains(ColorType.Green) && CurrentColorTypes.Contains(ColorType.Blue))
        {
            _animatedSprites[0].SetColor("_Color", new Color(_animatedSprites[0].color.r + 1f, _animatedSprites[0].color.g, _animatedSprites[0].color.b, _animatedSprites[0].color.a));
            _animatedSprites[1].SetColor("_Color", new Color(_animatedSprites[1].color.r, _animatedSprites[1].color.g + 1f, _animatedSprites[1].color.b, _animatedSprites[1].color.a));
            _animatedSprites[2].SetColor("_Color", new Color(_animatedSprites[2].color.r, _animatedSprites[2].color.g, _animatedSprites[2].color.b + 1f, _animatedSprites[2].color.a));
        }
    }

    public void OnBulbBreaksAnimation()
    {
        //Play broken bulb sound.
        AudioManager.PlayOneShot(_glassBreakClip);
        //_bulbAudioSource.PlayOneShot(_glassBreakClip);
        //Disable the bulb colors
        BreakBulb();
        UpdateUI();
    }

    public void OnBrokenBulbAnimationEnd()
    {
        for (int i = 0; i < _animatedSprites.Count; i++)
        {
            if (_animatedSprites[i].color == null)
                continue;

            _animatedSprites[i].color = _originalSprites[i].color;

            //var newColor = new Color(materials.color.r * 1.75f, _animatedSprites[i].color.b * 1.75f, _animatedSprites[i].color.b * 1.75f);
            //foreach (var material in materials)
            //{
            //    material.SetColor("_Color", new Color(1.1f, 1.1f, 1.1f));
            //}
        }
        //Hide animation screen and return game to defeat screen.
        BrokenBulbAnimationEnd?.Invoke(this);
    }
    #endregion Broken Bulb Animation (end)

    #region Private Methods
    private void ResetPowerLevel()
    {
        _powerLevel = 0;
    }
   
    private void SetCurrentPower()
    {            
        foreach (var externalSource in _externalPowerSources)
        {
            if (!externalSource.Powerable.IsPoweredFromOtherSide(this))
                continue;
            if (externalSource.Powerable is Battery battery)
            {
                if (battery.CurrentPower.ColorTypes.Any(c => CurrentColorTypes.Contains(c)))
                {
                    IncreasePower(battery.CurrentPower.Amount);
                }
            }
            else
            {
                var poweredsources = externalSource.Powerable.GetPowers(this);
                foreach (var power in poweredsources)
                {
                    if (power.ColorTypes.Any(c => CurrentColorTypes.Contains(c)))
                    {
                        IncreasePower(power.Amount);
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
            if (_isBroken)
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
        if (_shouldUpdateVisuals)
        {
            DisableLitImages();
            DisableUnLitImages();
            SetImagesByPowerLevel();
        }        
    }

    private void SetImagesByPowerLevel()
    {
        if (_isBroken)
            return;
        if (CurrentPowerLevel == _maxPower)
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
                _redTopUnLit?.SetActive(false);
                _greenBottomLit?.SetActive(true);
                _greenBottomUnLit?.SetActive(false);
            }
            else if (CurrentColorTypes.Contains(ColorType.Red) && CurrentColorTypes.Contains(ColorType.Blue) && !CurrentColorTypes.Contains(ColorType.Green))
            {
                _redTopLit?.SetActive(true);
                _redTopUnLit?.SetActive(false);
                _blueBottomLit?.SetActive(true);
                _blueBottomUnLit?.SetActive(false);
            }
            else if (CurrentColorTypes.Contains(ColorType.Green) && CurrentColorTypes.Contains(ColorType.Blue) && !CurrentColorTypes.Contains(ColorType.Red))
            {
                _greenTopLit?.SetActive(true);
                _greenTopUnLit?.SetActive(false);
                _blueBottomLit?.SetActive(true);
                _blueBottomUnLit?.SetActive(false);
            }
            else if (CurrentColorTypes.Contains(ColorType.Red) && CurrentColorTypes.Contains(ColorType.Green) && CurrentColorTypes.Contains(ColorType.Blue))
            {
                _redSmallTopLit?.SetActive(true);
                _redSmallTopUnLit?.SetActive(false);
                _greenSmallMiddleLit?.SetActive(true);
                _greenSmallMiddleUnLit?.SetActive(false);
                _blueSmallBottomLit?.SetActive(true);
                _blueSmallBottomUnLit?.SetActive(false);
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

    private List<Material> GetLitMaterials()
    {
        List<Material> materials = null;
        // Red Bulb
        if (CurrentColorTypes.Contains(ColorType.Red) && !(CurrentColorTypes.Contains(ColorType.Green) || CurrentColorTypes.Contains(ColorType.Blue)))
        {
            return new List<Material>((Material[])_redFullLit?.GetComponent<SpriteRenderer>().materials.Clone());
        }
        //Green Bulb
        else if (CurrentColorTypes.Contains(ColorType.Green) && !(CurrentColorTypes.Contains(ColorType.Red) || CurrentColorTypes.Contains(ColorType.Blue)))
        {
            return new List<Material>((Material[])_greenFullLit?.GetComponent<SpriteRenderer>().materials.Clone());
        }
        //Blue Bulb
        else if (CurrentColorTypes.Contains(ColorType.Blue) && !(CurrentColorTypes.Contains(ColorType.Red) || CurrentColorTypes.Contains(ColorType.Green)))
        {
            return new List<Material>((Material[])_blueFullLit?.GetComponent<SpriteRenderer>().materials.Clone());
        }
        // Red Green Bulb
        else if (CurrentColorTypes.Contains(ColorType.Red) && CurrentColorTypes.Contains(ColorType.Green) && !CurrentColorTypes.Contains(ColorType.Blue))
        {
            materials = new List<Material>((Material[])_redTopLit?.GetComponent<SpriteRenderer>().materials.Clone());
            materials.AddRange((Material[])_greenBottomLit?.GetComponent<SpriteRenderer>().materials.Clone());
        }
        // Red Blue Bulb
        else if (CurrentColorTypes.Contains(ColorType.Red) && CurrentColorTypes.Contains(ColorType.Blue) && !CurrentColorTypes.Contains(ColorType.Green))
        {
            materials = new List<Material>((Material[])_redTopLit?.GetComponent<SpriteRenderer>().materials.Clone());
            materials.AddRange((Material[])_blueBottomLit?.GetComponent<SpriteRenderer>().materials.Clone());            
        }
        // Green Blue Bulb
        else if (CurrentColorTypes.Contains(ColorType.Green) && CurrentColorTypes.Contains(ColorType.Blue) && !CurrentColorTypes.Contains(ColorType.Red))
        {
            materials = new List<Material>((Material[])_greenTopLit?.GetComponent<SpriteRenderer>().materials.Clone());
            materials.AddRange((Material[])_blueBottomLit?.GetComponent<SpriteRenderer>().materials.Clone());            
        }
        // All Three Colors
        else if (CurrentColorTypes.Contains(ColorType.Red) && CurrentColorTypes.Contains(ColorType.Green) && CurrentColorTypes.Contains(ColorType.Blue))
        {
            materials = new List<Material>((Material[])_redSmallTopLit?.GetComponent<SpriteRenderer>().materials.Clone());
            materials.AddRange((Material[])_greenSmallMiddleLit?.GetComponent<SpriteRenderer>().materials.Clone());
            materials.AddRange((Material[])_blueSmallBottomLit?.GetComponent<SpriteRenderer>().materials.Clone());            
        }
        
        return materials;
        
    }

    public override void SetPowerStateOff(PowerableBase requestor)
    {
        //Do Nothing
    }

    public override void DeterminePowerColorStateChange(PowerableBase powerableSource, bool checkDirection = false)
    {
        //ResetPowerable();
        //SetCurrentPower();
        //UpdateUI();
    }

    public override List<ColorType> GetOtherSideColors(PowerableBase requestor)
    {
        throw new NotImplementedException();
    }

    public override void CheckStateChanged(PowerableBase powerableSource, bool forceCheck)
    {
        ResetPowerable();
        SetCurrentPower();        
        UpdateUI();
    }
    #endregion Private Methods (end)
    #endregion Methods (end)
}