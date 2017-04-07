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
    public SpecTemplate(): base()
        {
        }

    public SpecTemplate(string nameId, string description/*, Sprite logo*/, List<string> tags, List<Formula> formulas, List<Template> slots): 
        base(nameId,description/*,logo*/,tags,formulas,slots)
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
