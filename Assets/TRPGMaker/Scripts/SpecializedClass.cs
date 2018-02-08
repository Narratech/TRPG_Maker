using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Linq;

[Serializable]
[CreateAssetMenu(fileName = "SpecializedClass", menuName = "RPG/SpecializedClass", order = 3)]
public class SpecializedClass : ScriptableObject {

    public string className;
    //private List<Tags> tags;
    public List<Slot> slots;
    public List<Attribute> attributes = null;
    public List<Formula> formulas;    
    
    public void refreshAttributes()
    {
        if(attributes == null)
            attributes = Database.Instance.attributes.Where(x => x.isCore).ToList();
        else {
            List<Attribute> aux = attributes.Where(x => !x.isCore && Database.Instance.attributes.Contains(x)).ToList();
            attributes = new List<Attribute>();
            attributes.AddRange(Database.Instance.attributes.Where(x => x.isCore).ToList());
            attributes.AddRange(aux);
        }
    }

}
