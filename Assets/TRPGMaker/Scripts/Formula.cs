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
}
