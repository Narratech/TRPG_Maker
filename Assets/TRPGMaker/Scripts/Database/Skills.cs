using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skills : ScriptableObject, ICloneable  {

    public string name;
    public string description;
    public SkillTypes skillType;
    public int areaRange;
    public List<Formula> formulas;

    public object Clone()
    {
        return this.MemberwiseClone();
    }
}
