using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// THINGS SPECS SHOULD HAVE:
// ...

public class SpecTemplate: Template
    {
    // Attributes defined in templates
    // ...
    // Attributes defined in instances
    
    public SpecTemplate(string name, string description/*, Sprite logo*/, List<Formula> formulas, List<Template> slots): 
        base(name,description/*,logo*/,formulas,slots)
        {
        // Attributes defined in templates
        // ...
        // Attributes defined in instances
        }

    public new void print()
        {
        Debug.Log("I'm a SpecTemplate");
        }

    }
