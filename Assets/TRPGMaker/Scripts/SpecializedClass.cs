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

    // Por si se han añadido nuevos atributos CORE a Database
    public void refreshAttributes()
    {
        if (attributes == null)
            attributes = Database.Instance.attributes.Where(x => x.isCore).ToList();
        else
        {
            List<Attribute> aux = new List<Attribute>();
            aux = attributes.Where(x => !x.isCore && Database.Instance.attributes.Contains(x)).ToList();
            attributes = new List<Attribute>();
            attributes.AddRange(Database.Instance.attributes.Where(x => x.isCore).ToList());
            attributes.AddRange(aux);
        }
    }

}
