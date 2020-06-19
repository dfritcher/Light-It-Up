using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using System;

public class Level : MonoBehaviour
{
    #region Fields, Properties
    // Start is called before the first frame update
    [SerializeField]
    private List<Battery> _batteries = null;

    [SerializeField]
    private List<Bulb> _bulbs = null;

    [SerializeField]
    private List<Wire> _wires = null;

    [SerializeField]
    private List<Junction> _junctions = null;

    [SerializeField]
    private List<Inhibitor> _inhibitors = null;

    [SerializeField]
    private List<PassThrough> _passThroughs = null;

    [SerializeField]
    private int _levelNumber = 0;

    private bool _isActive = false;
    public bool IsActive { get { return _isActive; } }

    private LevelManager _levelManger = null;

    private bool _canPlay = false;
    private bool _hasWon = false;
    #endregion Fields, Properties (end)
    
    #region Methods
    private void Start()
    {
        _canPlay = true;
        _hasWon = false;
        _batteries = transform.GetComponentsInChildren<Battery>().ToList();
        _bulbs = transform.GetComponentsInChildren<Bulb>().ToList();
        _wires = transform.GetComponentsInChildren<Wire>().ToList();
        _junctions = transform.GetComponentsInChildren<Junction>().ToList();
        _inhibitors = transform.GetComponentsInChildren<Inhibitor>().ToList();
        _passThroughs = transform.GetComponentsInChildren<PassThrough>().ToList();

        foreach (var bulb in _bulbs)
        {
            bulb.Setup();
        }
        foreach (var wire in _wires)
        {
            wire.Setup();
        }
        foreach(var junction in _junctions)
        {
            junction.Setup();
        }
        foreach(var inhibitor in _inhibitors)
        {
            inhibitor.Setup();
            inhibitor.OnClick += Inhibitor_OnClick;
        }
        foreach (var passThrough in _passThroughs)
        {
            passThrough.Setup();
            passThrough.OnClick += PassThrough_OnClick;
        }
        foreach (var battery in _batteries)
        {
            battery.Setup();
            battery.OnClick += Battery_OnClick;
        }
    }

    private void PassThrough_OnClick(PassThrough passThrough)
    {
        _levelManger.PassThroughOptionsManager.Setup(passThrough);
    }

    private void Inhibitor_OnClick(Inhibitor inhibitor)
    {
        _levelManger.InhibitorOptionsManager.Setup(inhibitor);
    }

    private void LateUpdate()
    {
        if (_canPlay)
        {
            CheckWinCondition();
        }        
    }

    public void Setup(LevelManager levelManager)
    {
        _levelManger = levelManager;        
    }

    public void RestartLevel()
    {
        _bulbs.ForEach(b => b.ResetPowerable());
        _wires.ForEach(w => w.ResetPowerable());
        _junctions.ForEach(j => j.ResetPowerable());
        _inhibitors.ForEach(i => i.ResetPowerable());
        _passThroughs.ForEach(p => p.ResetPowerable());
        _batteries.ForEach(p => p.ResetPowerable());
        _batteries.ForEach(p => p.ResetPower());
        _levelManger.ResetVictoryState();
        _canPlay = true;
        _hasWon = false;
    }
    
    public void SetActiveState(bool isActive)
    {
        _isActive = isActive;
        gameObject.SetActive(_isActive);
        if(_isActive)
            _levelManger?.SetLevelDisplay(_levelNumber);
    }

    private void Battery_OnClick(Battery battery)
    {
        _levelManger.BatteryOptionsManager.Setup(battery);
    }

    private void CheckWinCondition()
    {
        if(_bulbs.All(b => b.IsOn))
        {
            _levelManger.SetVictoryState(true);
            _hasWon = true;
            _canPlay = false;
        }
        else if(_bulbs.Any(b => b.IsBroken))
        {
            _levelManger.SetVictoryState(false);
            _hasWon = false;
            _canPlay = false;
        }
    }
    #endregion Methods (end)
}
