using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// THINGS SPECS SHOULD HAVE:
// ...

public class SpecTemplate: Template
    {
    private bool isBasicClass;
    // Attributes defined in templates
    // ...
    // Attributes defined in instances
    public SpecTemplate(): base()
        {
        }

    public SpecTemplate(string nameId, string description, bool isBasicClass, List<Formula> formulas, SlotsConfig allowedSlots): 
        base(nameId,description,null,formulas,allowedSlots)
        {
        this.isBasicClass=isBasicClass;
        // Attributes defined in templates
        // ...
        // Attributes defined in instances
        }
 
    public bool IsBasicClass
        {
        get { return isBasicClass; }
        set { isBasicClass=value; }
        }

    public new void print()
        {
        Debug.Log("I'm a SpecTemplate");
        }

    }
