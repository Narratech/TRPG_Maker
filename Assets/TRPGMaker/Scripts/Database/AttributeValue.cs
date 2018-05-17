using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AttributeValue{

    [SerializeField]
    private Attribute _attribute;
    [SerializeField]
    public Attribute attribute
    {
        get
        {
            return _attribute;
        }
        set {
            _attribute = value;
        }
    }
    [SerializeField]
    public int maxValue = 0;
    [SerializeField]
    public int minValue = 0;
    [SerializeField]
    public int value = 0;
}
