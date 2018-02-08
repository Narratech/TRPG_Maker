using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Attribute{

    // Defined in editor
    public string name;  // Long identifier
    public string id;  // Three letters identifier      
    public string description;
    public int maxValue;
    public int minValue;
    // Defined in game (now just default values)
    public int value;
    public bool isCore;  // 'true' if is a basic or core attribute    

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
}
