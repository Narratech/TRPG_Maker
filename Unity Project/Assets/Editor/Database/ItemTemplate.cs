using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTemplate: Template 
    {
    // Attributes defined in templates
    // ...
    // Attributes defined in instances
    // ...

    public ItemTemplate(): base()
        {
        }

    public ItemTemplate(string nameId, string description/*, Sprite logo*/, List<Formula> formulas, List<Template> slots): 
        base(nameId,description/*,logo*/,formulas,slots)
        {
        // Attributes defined in templates
        // ...
        // Attributes defined in instances
        // ...
        }

    public void toConsole()
        {
        Debug.Log(nameId);
        Debug.Log(description);
        Debug.Log(formulas.Count);
        Debug.Log(slots.Count);
        }

    }
