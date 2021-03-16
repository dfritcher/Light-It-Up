using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Junction : PowerableBase
{

    [SerializeField]
    private List<ColorType> _currentColorTypes = null;
    public List<ColorType> CurrentColorTypes { get { return _currentColorTypes ?? (_currentColorTypes = new List<ColorType>()); } }

    public override bool IsClickable => false;

    [SerializeField]
    private bool _isPowered = false;
    public override bool IsPowered { get { return _isPowered; } }

    [SerializeField]
    private List<Image> _junctionColors = null;

    [SerializeField]
    private List<ExternalPower> _poweredBulbs = null;

    /// <summary>
    /// Reference to all the sources providing us power and the direction the power is coming from.
    /// This will help us determine which directions power is flowing when asked for our colors.
    /// </summary>
    [SerializeField]
    private List<ExternalPower> _powerSources = null;

    [SerializeField]
    private ParticleSystem _redSpark = null;

    [SerializeField]
    private ParticleSystem _blueSpark = null;

    [SerializeField]
    private ParticleSystem _greenSpark = null;

    private float _redSparkInterval = 1f;
    private float _blueSparkInterval = 1f;
    private float _greenSparkInterval = 1f;
    private float _redSparkTime = 0f;
    private float _blueSparkTime = 0f;
    private float _greenSparkTime = 0f;

    protected override void Awake()
    {
        base.Awake();
        UpdateColorDisplay();
        _redSparkInterval = UnityEngine.Random.Range(5f, 10f);
        _blueSparkInterval = UnityEngine.Random.Range(5f, 10f);
        _greenSparkInterval = UnityEngine.Random.Range(5f, 10f);
    }

    private void Update()
    {
        if(_redSparkTime > _redSparkInterval)
        {
            if(CurrentColorTypes.Contains(ColorType.Red))
                _redSpark.Play();
            _redSparkTime = 0f;
        }
        else
        {
            _redSparkTime += Time.deltaTime;
        }

        if (_blueSparkTime > _blueSparkInterval)
        {
            if(CurrentColorTypes.Contains(ColorType.Blue))
                _blueSpark.Play();
            _blueSparkTime = 0f;
        }
        else
        {
            _blueSparkTime += Time.deltaTime;
        }

        if (_greenSparkTime > _greenSparkInterval)
        {
            if(CurrentColorTypes.Contains(ColorType.Green))
                _greenSpark.Play();
            _greenSparkTime = 0f;
        }
        else
        {
            _greenSparkTime += Time.deltaTime;
        }
    }

    public void Setup()
    {
        
    }

    private void SetCurrentPower()
    {
        _currentColorTypes = _originalColorTypes.Clone();
        foreach (var externalSource in _externalPowerSources)
        {
            if (!externalSource.Powerable.IsPoweredFromOtherSide(this))
                continue;
            if (externalSource.Powerable is Battery battery)
            {
                foreach (var color in battery.CurrentPower.ColorTypes)
                {
                    if (!CurrentColorTypes.Contains(color))
                    {
                        CurrentColorTypes.Add(color);
                    }
                }
            }
            else
            {
                foreach (var poweredColor in externalSource.Powerable.PoweredColors)
                {
                    foreach (var color in poweredColor.ColorTypes)
                    {
                        if (!CurrentColorTypes.Contains(color))
                        {
                            CurrentColorTypes.Add(color);
                        }
                    }
                }
            }
        }
    }

    private void UpdateColorDisplay()
    {
        //if (_junctionColors == null || _junctionColors.Count <= 0)
        //    return;
        //_junctionColors[1].gameObject.SetActive(CurrentColorTypes.Contains(ColorType.Red));
        //_junctionColors[2].gameObject.SetActive(CurrentColorTypes.Contains(ColorType.Green));
        //_junctionColors[3].gameObject.SetActive(CurrentColorTypes.Contains(ColorType.Blue));

        //_junctionColors[0].gameObject.SetActive(!_junctionColors[1].IsActive() && !_junctionColors[2].IsActive() && !_junctionColors[3].IsActive());
    }
   
    public override List<Power> GetPowers(PowerableBase requestor)
    {
        throw new NotImplementedException();
    }

    public override void ResetPowerable()
    {
        _currentColorTypes = new List<ColorType>(_originalColorTypes);
        UpdateColorDisplay();
    }

    public override void DetermineNewPowerState(PowerableBase powerableBase)
    {
        ResetPowerable();
        SetCurrentPower();
        CheckPoweredState();
        UpdateColorDisplay();       
    }

    private void CheckPoweredState()
    {
        var isPowered = false;
        foreach (var source in _externalPowerSources)
        {
            if (isPowered)
                break;
            isPowered = source.Powerable.IsPowered;
        }
        _isPowered = isPowered;
    }

    public override bool GetPoweredState(PowerableBase requestor)
    {
        return _currentColorTypes.Count > 0;
    }

    public override void SetPowerStateOff(PowerableBase requestor)
    {
        
    }

    public override void DeterminePowerColorStateChange(PowerableBase powerableBase, bool checkDirection = false)
    {
        ResetPowerable();
        SetCurrentPower();
        CheckPoweredState();
        UpdateColorDisplay();
    }

    public override List<ColorType> GetOtherSideColors(PowerableBase requestor)
    {
        throw new NotImplementedException();
    }

    public override void CheckStateChanged(PowerableBase requestor, bool forcheCheck)
    {
        ResetPowerable();
        SetCurrentPower();
        CheckPoweredState();
        UpdateColorDisplay();
    }
}
