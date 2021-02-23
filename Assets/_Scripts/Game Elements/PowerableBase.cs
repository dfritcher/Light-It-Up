using UnityEngine;
using System.Collections.Generic;

public abstract class PowerableBase : MonoBehaviour
{
    #region Fields, Properties
    /// <summary>
    /// Reference to our starting colors. Used to reset our state to starting state.
    /// </summary>
    [Header("Base Fields"), SerializeField]
    protected List<ColorType> _originalColorTypes;
    public List<ColorType> OriginalColorTypes { get { return _originalColorTypes; } }

    [SerializeField]
    protected Power _power = new Power() { Amount = 1, ColorTypes = new List<ColorType>() { ColorType.None } };    
    public abstract bool IsClickable { get; }
    public abstract bool IsPowered { get;}
    public abstract List<Power> GetPowers(PowerableBase requestor);
    public abstract List<ColorType> CurrentColorTypes { get; }
    #endregion Fields, Properties (end)
    

    #region Methods
    public abstract void ResetPowerable();
    
    public abstract void GetBatteryPowerState(PowerableBase powerableBase);

    public abstract void DetermineNewPowerState(PowerableBase powerableBase, bool checkDirection = false);

    //Method to determine if a powerableBase object is being powered from the direction other than the direction of the requestor.
    public abstract bool GetPoweredState(PowerableBase requestor);

    public abstract void SetPowerStateOff(PowerableBase requestor);

    protected virtual void Awake()
    {
        ResetPowerable();
    }

    public virtual void SetColorTypes(List<ColorType> colorTypes)
    {
        _originalColorTypes = colorTypes;        
    }
    #endregion Methods (end)
}
