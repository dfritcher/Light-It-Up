using System.Collections.Generic;
using System;

[Serializable]
public class ExternalPower 
{
    public PowerableBase Powerable;
    public Direction InputDirection;
    
    public ExternalPower() { }
    public ExternalPower(ExternalPower powerSource)
    {
        Powerable = powerSource.Powerable;
        InputDirection = powerSource.InputDirection;        
    }
}
