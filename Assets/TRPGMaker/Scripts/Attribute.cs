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
}
