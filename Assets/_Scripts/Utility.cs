using System.Collections.Generic;

public static class Utility 
{
    public static List<ColorType> Clone(this List<ColorType> value)
    {
        return new List<ColorType>(value);
    }

    public static List<Power> Clone(this List<Power> value)
    {
        var clone = new List<Power>();
        foreach(var power in value)
        {
            clone.Add(power.Clone());
        }
        return clone;
    }

    public static Power Clone(this Power value)
    {
        return new Power() { Amount = value.Amount, ColorTypes = value.ColorTypes.Clone(), Direction = value.Direction };
    }
}
