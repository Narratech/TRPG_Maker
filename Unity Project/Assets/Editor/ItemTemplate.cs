using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTemplate: Template 
    {
    // Attributes defined in templates
    // ...
    // Attributes defined in instances
    // ...

    public ItemTemplate(string name, string description/*, Sprite logo*/, List<Formula> formulas, List<Template> slots): 
        base(name,description/*,logo*/,formulas,slots)
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
