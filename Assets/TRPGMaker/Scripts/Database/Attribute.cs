using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Attribute : ICloneable
{

    // Defined in editor
    [SerializeField]
    public string name;  // Long identifier
    [SerializeField]
    public string id;  // Three letters identifier      
    [SerializeField]
    public string description;
    [SerializeField]
    public int maxValue;
    [SerializeField]
    public int minValue;
    // Defined in game (now just default values)
    [SerializeField]
    public int value;
    [SerializeField]
    public bool isCore;  // 'true' if is a basic or core attribute    

    public Attribute(string name, string id, string description, int maxValue, int minValue, int value, bool isCore)
    {
        // Defined in editor
        this.isCore = isCore;
        this.id = id;
        this.name = name;
        this.description = description;
        this.minValue = minValue;
        this.maxValue = maxValue;
        // Defined in game (now just defalut values)
        this.value = value;
    }

    public object Clone()
    {
        return this.MemberwiseClone();
    }

    protected bool Equals(Attribute other)
    {
        return id == other.id;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Attribute)obj);
    }

    public override int GetHashCode()
    {
        return id.GetHashCode();
    }
}
