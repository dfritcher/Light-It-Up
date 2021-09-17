using System.Collections.Generic;
using UnityEngine;
public static class Utility 
{
    private static Vector2 _largeGrid = new Vector2(300, 300);
    private static Vector2 _smallGrid = new Vector2(150, 150);
    private static Vector2 _standardGrid = new Vector2(250, 250);
    private static Vector2 _oneAndAHalfScale = new Vector3(1.5f, 1.5f, 1.5f);
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

    public static Vector2 GetGridSize()
    {
        if (Camera.main.aspect > 1.3 && Camera.main.aspect < 1.3333334)
        {
            return _largeGrid;
        }
        if (Camera.main.aspect > 1.7 && Camera.main.aspect < 1.7778)
        {
            return _standardGrid;
        }
        if (Camera.main.aspect > 2 && Camera.main.aspect < 2.11112)
        {
            return _smallGrid;
        }
        else if (Camera.main.aspect > 2 && Camera.main.aspect < 2.141)
        {
            return _smallGrid;
        }
        else if (Camera.main.aspect > 2 && Camera.main.aspect < 2.17)
        {
            return _standardGrid;
            
        }
        else if (Camera.main.aspect  > 2 && Camera.main.aspect  < 2.2223)
        {
            return _smallGrid;
        }
        return _smallGrid;
    }

    public static Vector2 GetScale()
    {
        if (Camera.main.aspect > 1.3 && Camera.main.aspect < 1.3333334)
        {
            return _oneAndAHalfScale;
        }
        if (Camera.main.aspect  > 1.7 && Camera.main.aspect < 1.7778)
        {
            return Vector2.one;
        }
        if (Camera.main.aspect  > 2 && Camera.main.aspect < 2.11112)
        {
            return Vector2.one;
        }
        else if (Camera.main.aspect > 2 && Camera.main.aspect < 2.141)
        {
            return Vector2.one;
        }
        else if (Camera.main.aspect > 2 && Camera.main.aspect < 2.17)
        {
            return Vector2.one;
        }
        else if (Camera.main.aspect > 2 && Camera.main.aspect < 2.2223)
        {
            return _oneAndAHalfScale;
        }

        return Vector2.one;
    }
}
