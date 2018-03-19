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
    public string name;
    public Inventory inventory;
    public List<Slot> Slots;
    public List<Attribute> attributes = null;
    public List<SpecializedClass> specializedClass;
    public int height;
    public int distance;

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