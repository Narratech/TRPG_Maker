using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attribute
    {
    // Defined in editor
    public bool isCore;  // 'true' if is a core attribute
    public string label;  // Three letters identifier
    public string name;  // Long identifier
    public string description;
    public int maxValue;
    public int minValue;
    // Defined in game (now just default values)
    public int value;

    public Attribute(bool isCore, string label, string name, string description, int maxValue, int minValue)
        {
        // Defined in editor
        this.isCore=isCore;
        this.label=label;
        this.name=name;
        this.description=description;
        this.maxValue=maxValue;
        this.minValue=minValue;
        // Defined in game (now just defalut values)
        this.value=minValue;
        }

    public void print()
        {
        Debug.Log("I'm an attribute");
        }
    }
