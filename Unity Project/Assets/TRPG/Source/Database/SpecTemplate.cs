using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpecTemplate: Template
    {
    [SerializeField]
    private bool isBasicClass;
    
    public SpecTemplate Init(string nameId, string description, bool isBasicClass, List<Formula> formulas, SlotsConfig allowedSlots)
        {
        base.Init(nameId,description,null,formulas,allowedSlots);
        this.isBasicClass=isBasicClass;
        return this;
        }
 
    public bool IsBasicClass
        {
        get { return isBasicClass; }
        set { isBasicClass=value; }
        }
    }
