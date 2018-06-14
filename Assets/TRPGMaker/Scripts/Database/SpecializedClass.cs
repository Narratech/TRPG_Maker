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
    public List<Tag> tags;
    public List<Slot> slots;
    [SerializeField]
    public List<AttributeValue> attributes;
    public FormulaScript formula;
    public List<Skills> skills;

    public void Init()
    {
        tags = new List<Tag>();
        slots = new List<Slot>();
        formula = ScriptableObject.CreateInstance<FormulaScript>();
        attributes = new List<AttributeValue>();
        skills = new List<Skills>();
    }

    public void refreshAttributes()
    {
        if (attributes == null)
            attributes = new List<AttributeValue>();
        else
        {
            // Remove Deleted
            attributes.RemoveAll(x => !Database.Instance.attributes.Contains(x.attribute));
        }
    }

}
