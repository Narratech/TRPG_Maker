using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTemplate: Template 
    {
    // Attributes defined in templates
    // ...
    // Attributes defined in instances
    // ...

    public ItemTemplate(string nameId, string description/*, Sprite logo*/, List<Formula> formulas, List<Template> slots): 
        base(nameId,description/*,logo*/,formulas,slots)
        {
        // Attributes defined in templates
        // ...
        // Attributes defined in instances
        // ...
        }

    public new void print()
        {
        Debug.Log("I'm a ItemTemplate");
        }

    }
