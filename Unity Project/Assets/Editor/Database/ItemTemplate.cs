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

    public ItemTemplate(string nameId, string description, List<string> tags, List<Formula> formulas, SlotsConfig allowedSlots): 
        base(nameId,description,tags,formulas,allowedSlots)
        {
        // Attributes defined in templates
        // ...
        // Attributes defined in instances
        // ...
        }

    public void toConsole()
        {
        Debug.Log(_nameId);
        Debug.Log(_description);
        Debug.Log(_formulas.Count);
        }
    }
