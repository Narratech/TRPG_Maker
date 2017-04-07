using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Template
    {
    // Defined in templates
    public string nameId;  // The name of the object identifies it
    public string description;
    public List<string> tags;  // Every tag this Template has
    public List<Formula> formulas;  // Every formula that modifies attributes for this Template
    public List<Template> slots;  // Templates which this Template has
    //public Sprite logo;

    public Template()
        {
        }

    public Template(string nameId, string description/*, Sprite logo*/, List<string> tags, List<Formula> formulas, List<Template> slots)
        {
        // Defined in templates
        this.nameId=nameId;
        this.description=description;
        //this.logo=logo;
        this.tags=tags;
        this.formulas=formulas;
        this.slots=slots;
        }

    public void print()
        {
        Debug.Log("I'm a generic Template");
        }

	}
