using System.Collections.Generic;
using UnityEngine;
using System;

public class Junction : PowerableBase
{
    [SerializeField] private List<ColorType> _currentColorTypes = null;
    public List<ColorType> CurrentColorTypes { get { return _currentColorTypes ?? (_currentColorTypes = new List<ColorType>()); } }

    public override bool IsClickable => false;

    [SerializeField] private bool _isPowered = false;
    public override bool IsPowered { get { return _isPowered; } }
  
    [SerializeField] private ParticleSystem _redSpark = null;
    [SerializeField] private ParticleSystem _blueSpark = null;
    [SerializeField] private ParticleSystem _greenSpark = null;

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
    public override void OnMouseDown()
    {

    }
}