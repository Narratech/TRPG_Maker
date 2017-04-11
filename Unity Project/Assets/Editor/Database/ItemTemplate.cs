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

    public ItemTemplate(string nameId, string description, List<string> tags, List<Formula> formulas, List<Template> slots): 
        base(nameId,description/*,logo*/,tags,formulas,slots)
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
