using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skills : ScriptableObject, ICloneable  {

    [SerializeField]
    public string name;
    [SerializeField]
    public string description;
    [SerializeField]
    public SkillTypes skillType;
    [SerializeField]
    public float damage;

    public object Clone()
    {
        return this.MemberwiseClone();
    }
}
