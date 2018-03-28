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
    // ¿Lo ponemos como String o como "Tag"?
    public List<String> tags;
    public List<Slot> slots;
    public List<Attribute> attributes = null;
    public List<Formula> formulas;

    public void Init()
    {
        tags = new List<string>();
        slots = new List<Slot>();
        formulas = new List<Formula>();
    }

    // Por si se han añadido nuevos atributos CORE a Database
    public void refreshAttributes()
    {
        if (attributes == null)
            attributes = Extensions.Clone<Attribute>(Database.Instance.attributes.Where(x => x.isCore).ToList()).ToList();
        else
        {
            attributes.AddRange(Extensions.Clone<Attribute>(Database.Instance.attributes.Where(x => !attributes.Any(y => y.id == x.id) && x.isCore).ToList()));
            List<Attribute> aux = new List<Attribute>();
            aux.AddRange(attributes.Where(x => Database.Instance.attributes.Any(y => y.id == x.id)));
            attributes = aux;
        }
    }

}
