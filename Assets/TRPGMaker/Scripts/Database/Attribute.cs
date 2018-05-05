using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Attribute : ICloneable
{
    [SerializeField]
    public string name;
    [SerializeField]
    public string id;
    [SerializeField]
    public string description;
    [SerializeField]
    public bool isCore;

    public Attribute() {  }

    public Attribute(string name, string id, string description, bool isCore)
    {
        // Defined in editor
        this.isCore = isCore;
        this.id = id;
        this.name = name;
    }

    public object Clone()
    {
        AttributeValue attributeValue = new AttributeValue();
        attributeValue.attribute = this;
        return attributeValue;
    }
}
