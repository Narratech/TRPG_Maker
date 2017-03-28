using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Template
    {
    // Defined in templates
    public string name;
    public string description;
    public List<Formula> formulas;  // Every formula that modifies attributes for this Template
    public List<Template> slots;  // Templates which this Template has
    //public Sprite logo;

    public Template(string name, string description/*, Sprite logo*/, List<Formula> formulas, List<Template> slots)
        {
        // Defined in templates
        this.name=name;
        this.description=description;
        //this.logo=logo;
        this.formulas=formulas;
        this.slots=slots;
        }

    public void print()
        {
        Debug.Log("I'm a generic Template");
        }

	}
