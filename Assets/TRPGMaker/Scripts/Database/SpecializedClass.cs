using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Linq;

[Serializable]
public class SpecializedClass : ScriptableObject
{
    public new string name;
    public List<String> tags;
    public List<Slot> slots;
    public List<AttributeValue> attributes = null;
    public FormulaScript formula;
    public List<Skills> skills;

    public void Init()
    {
        tags = new List<string>();
        slots = new List<Slot>();
        formula = ScriptableObject.CreateInstance<FormulaScript>();
    }

    public void refreshAttributes()
    {
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
        }
    }

}
