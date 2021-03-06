using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class Power 
{
    public int Amount;
    [SerializeField]
    private List<ColorType> colorTypes;
    public List<ColorType> ColorTypes
    {
        get { return colorTypes ??= new List<ColorType>() { ColorType.None}; }
        set { colorTypes = value; }
    }
    public Direction Direction;
}
