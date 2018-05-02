using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Inventory))]
[Serializable]
public class Character: ScriptableObject{

    //public ... image/texture
    [SerializeField]
    public string name;
    [SerializeField]
    public Inventory inventory;
    [SerializeField]
    public List<Slot> Slots;
    [SerializeField]
    public List<Attribute> attributes = null;
    [SerializeField]
    public List<Attribute> attributesWithFormulas = null;
    [SerializeField]
    public List<SpecializedClass> specializedClasses;
    public int specializedClassesCount = 0;

    public void init()
    {
        Slots = new List<Slot>();
        specializedClasses = new List<SpecializedClass>();
    }

    public void refresh()
    {

        // Clear deleted specializedClasses
        for (int i = 0; i < specializedClasses.Count; i++)
            if (specializedClasses[i] == null)
                specializedClasses.RemoveAt(i);

        // Refrresh Attributes
        if (attributes == null)
            attributes = Extensions.Clone<Attribute>(Database.Instance.attributes.Where(x => x.isCore).ToList()).ToList();
        else
        {
            attributes.AddRange(Extensions.Clone<Attribute>(Database.Instance.attributes.Where(x => !attributes.Any(y => y.id == x.id) && x.isCore).ToList()));
            List<Attribute> aux = new List<Attribute>();
            aux.AddRange(attributes.Where(x => Database.Instance.attributes.Any(y => y.id == x.id)));
            attributes = aux;
            // Add attributes of specialized classes
            attributes.AddRange(specializedClasses.SelectMany(z => z.attributes).Where(x => !attributes.Any(y => y.id == x.id)));
        }
    }

    public void calculateFormulas()
    {
        var f = FormulaScript.Create("");
        attributesWithFormulas = Extensions.Clone<Attribute>(attributes).ToList();
        // Formulas in slots
        foreach (Slot slot in Slots)
        {
           foreach(Formula formula in slot.modifier.formulas)
           {
               f.formula = formula.formula;
               var r = f.FormulaParser.Evaluate();
                attributesWithFormulas.Find(x => x.id == formula.attributeID).value += (int) r;                    
           }
        }
        // Formulas in slots of specialized classes
        foreach (SpecializedClass specializedClass in specializedClasses)
        {
            foreach (Slot slot in specializedClass.slots)
            {
                foreach (Formula formula in slot.modifier.formulas)
                {
                    f.formula = formula.formula;
                    var r = f.FormulaParser.Evaluate();
                    attributesWithFormulas.Find(x => x.id == formula.attributeID).value += (int)r;
                }
            }
        }
        specializedClassesCount = specializedClasses.Count;
    }
}