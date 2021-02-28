using System.Collections.Generic;
using System;

[Serializable]
public class PowerSource 
{
    public PowerableBase Powerable;
    public Direction InputDirection;
    public List<ColorType> ProvidedColors;
}
