using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// THINGS PASSIVES SHOULD HAVE:
// + Formulas: passives act as formulas while they can 
// X Timer: there are passives that act over time
// X Turn: there are passives that act over turns
// X Counterattack: there are passives that are counterattacks
// ...

public class PassiveTemplate: Template
    {
    // Attributes defined in templates
    // ...
    // Attributes defined in instances
    // ...

    public PassiveTemplate(string name, string description/*, Sprite logo*/, List<Formula> formulas, List<Template> slots): 
        base(name,description/*,logo*/,formulas,slots)
        {
        // Attributes defined in templates
        // ...
        // Attributes defined in instances
        // ...
        }

    public new void print()
        {
        Debug.Log("I'm a PassiveTemplate");
        }

	}
