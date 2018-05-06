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
    public List<AttributeValue> attributes;
    [SerializeField]
    public List<AttributeValue> attributesWithFormulas;
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
            attributes = Extensions.Clone<AttributeValue>(Database.Instance.attributes.Where(x => x.isCore).ToList()).ToList();
        else
        {
            // Remove Deleted
            attributes.RemoveAll(x => !Database.Instance.attributes.Contains(x.attribute));

            attributes.AddRange(Extensions.Clone<AttributeValue>(Database.Instance.attributes.Where(x => !attributes.Any(y => y.attribute.id == x.id) && x.isCore).ToList()));
            List<AttributeValue> aux = new List<AttributeValue>();
            aux.AddRange(attributes.Where(x => Database.Instance.attributes.Any(y => y.id == x.attribute.id)));
            attributes = aux;
            // Add attributes of specialized classes
            attributes.AddRange(specializedClasses.SelectMany(z => z.attributes).Where(x => !attributes.Any(y => y.attribute.id == x.attribute.id)));
        }
    }

    public void calculateFormulas()
    {
        var f = FormulaScript.Create("");
        attributesWithFormulas = Extensions.Clone<AttributeValue>(attributes.Select(x => x.attribute).ToList()).ToList();
        // Formulas in slots
        foreach (Slot slot in Slots)
        {
           foreach(Formula formula in slot.modifier.formulas)
           {
                f.formula = formula.formula;
                if (f.FormulaParser.IsValidExpression)
                {
                    var r = f.FormulaParser.Evaluate(attributes);
                    AttributeValue attrbValue = attributesWithFormulas.Find(x => x.attribute.id == formula.attributeID);
                    if(attrbValue != null)
                        attributesWithFormulas.Find(x => x.attribute.id == formula.attributeID).value += (int)r;
                }
           }
        }
        // Formulas in slots of specialized classes
        foreach (SpecializedClass specializedClass in specializedClasses)
        {
            foreach (Slot slot in specializedClass.slots)
            {
                foreach (Formula formula in slot.modifier.formulas)
                {
                    if (f.FormulaParser.IsValidExpression)
                    {
                        f.formula = formula.formula;
                        var r = f.FormulaParser.Evaluate(attributes);
                        attributesWithFormulas.Find(x => x.attribute.id == formula.attributeID).value += (int)r;
                    }
                }
            }
        }
        specializedClassesCount = specializedClasses.Count;
    }
}