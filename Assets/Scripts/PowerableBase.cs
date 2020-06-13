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
    [SerializeField]
    protected Power _power = null;

    public abstract bool IsClickable { get; }
    public abstract bool IsPowered { get; }
    public abstract List<Power> GetPowers(PowerableBase requestor);
    public abstract List<ColorType> CurrentColorTypes { get; }

    public abstract void ResetPowerable();
    public abstract void UpdatePowerState(PowerableBase powerableBase);

    //Method to determine if a powerableBase object is being powered from the direction other than the direction of the requestor.
    public abstract bool GetPoweredState(PowerableBase requestor);
    
    protected virtual void Awake()
    {
        ResetPowerable();
    }
}
