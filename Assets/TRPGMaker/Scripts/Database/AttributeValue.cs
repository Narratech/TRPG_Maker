using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AttributeValue {

    public Attribute attribute;
    [SerializeField]
    public int maxValue = 0;
    [SerializeField]
    public int minValue = 0;
    [SerializeField]
    public int value = 0;
}
