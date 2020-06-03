using UnityEngine;
using System.Collections.Generic;
using System;

public abstract class PowerableBase : MonoBehaviour
{
    /// <summary>
    /// Reference to our starting colors. Used to reset our state to starting state.
    /// </summary>
    [SerializeField]
    protected List<ColorType> _originalColorTypes;


    //public Direction InputDirection;
    public abstract bool IsClickable { get; }
    public abstract bool IsPowered { get; }
    public abstract List<ColorType> GetPowers(PowerableBase requestor);
    
    public abstract List<ColorType> CurrentColorTypes { get; }

    //public List<PowerSource> PowerSources { get; set; }

    //public delegate void PowerEvent(PowerableBase powerable);
    //Event used by terminating items to get their total power state, things like Bulbs and wires that don't update anyone else
    //public event PowerEvent OnTypesSet;
    //public event PowerEvent OnCheckPowerState;

    //public void TypeSetChanged()
    //{
    //    OnTypesSet?.Invoke(this);
    //}

    //public void CheckPowerState()
    //{
    //    OnCheckPowerState?.Invoke(this);
    //}

    public abstract void ResetPowerable();
    public abstract void CascadeReset(PowerableBase powerableBase);
    public abstract void UpdatePowerState(PowerableBase powerableBase);

    //Method to determine if a powerableBase object is being powered from the direction other than the direction of the requestor.
    public abstract bool GetPoweredState(PowerableBase requestor);
    //public bool IsPoweredFromDirection(Direction direction)
    //{
    //    return PowerSources.Find(ps => ps.InputDirection == direction).Powerable.IsPowered;
    //}

    protected virtual void Awake()
    {
        ResetPowerable();
    }
}
