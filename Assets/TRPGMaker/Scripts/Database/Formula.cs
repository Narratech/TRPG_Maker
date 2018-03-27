using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[Serializable]
public class Formula {

    public enum operatorFormula{
        Sum,
        Sub
    }

    public Attribute attributeModified;
    public operatorFormula operation;
    public float value;

    public void setAttributeModified(Attribute attribute)
    {
        this.attributeModified = new Attribute(attribute.name, attribute.id, attribute.description, attribute.maxValue, attribute.minValue, attribute.value, attribute.isCore);
    }
}
