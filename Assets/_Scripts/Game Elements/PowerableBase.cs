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
    /// <summary>
    /// Reference to all the other powerables that surround us.
    /// </summary>
    [SerializeField]
    protected List<ExternalPower> _externalPowerSources = new List<ExternalPower>();

    /// <summary>
    /// Reference to the the list of powers we are currently getting from all other sources.
    /// We use this to determine which colors we provide outward.
    /// Also used to determine if we have changed based on a triggered change.
    /// </summary>
    [SerializeField]
    protected List<Power> _poweredColors;
    public List<Power> PoweredColors { 
        get { return _poweredColors ??= _poweredColors = new List<Power>(); } 
        set { _poweredColors = value; }
    }

    public List<ColorType> OriginalColorTypes { get { return _originalColorTypes; } }

    
    [SerializeField]
    protected static List<ColorType> _allColors = new List<ColorType>() {ColorType.Red, ColorType.Green, ColorType.Blue};

    /// <summary>
    /// Reference to the power we currently supply to all others
    /// </summary>
    [SerializeField]
    protected Power _currentPower = new Power() { Amount = 1, ColorTypes = new List<ColorType>() { ColorType.None } };
    public Power CurrentPower { get { return _currentPower; } }

    public abstract bool IsClickable { get; }
    public abstract bool IsPowered { get; }

    /// <summary>
    /// Method for Bulbs and wires to ask other Powerables what power they are provided.
    /// </summary>
    /// <param name="requestor"></param>
    /// <returns></returns>
    public abstract List<Power> GetPowers(PowerableBase requestor);
    
    public virtual void Setup(PowerableBase powerableBase) { }
    #endregion Fields, Properties (end)


    #region Methods
    public abstract void ResetPowerable();

    public virtual void DetermineNewPowerState(PowerableBase powerable) { }

    //Method to determine if a powerableBase object is being powered from the direction other than the direction of the requestor.
    public abstract bool GetPoweredState(PowerableBase requestor);

    public abstract void SetPowerStateOff(PowerableBase requestor);

    public virtual List<ColorType> GetOtherSideColors(PowerableBase requestor){ return null; }

    public abstract void CheckStateChanged(PowerableBase powerable, bool forceCheck);

    protected virtual void Awake()
    {
        ResetPowerable();
    }

    public virtual void SetColorTypes(List<ColorType> colorTypes)
    {
        _originalColorTypes = colorTypes;        
    }

    public virtual bool IsPoweredFromOtherSide(PowerableBase requestor)
    {
        return false;
    }

    public abstract void OnMouseDown();
    
    #endregion Methods (end)
}
