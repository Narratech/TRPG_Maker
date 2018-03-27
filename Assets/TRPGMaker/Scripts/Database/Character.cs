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
    public List<SpecializedClass> specializedClass;
    [SerializeField]
    public int height;
    [SerializeField]
    public int distance;
    public int attackRange;

    public void init()
    {
        Slots = new List<Slot>();
        attributes = new List<Attribute>();
        specializedClass = new List<SpecializedClass>();
    }

    public void refreshAttributes()
    {
        if (attributes == null)
            attributes = Extensions.Clone<Attribute>(Database.Instance.attributes.Where(x => x.isCore).ToList()).ToList();
        else
        {
            List<Attribute> aux = new List<Attribute>();
            aux = Extensions.Clone<Attribute>(attributes.Where(x => !x.isCore && Database.Instance.attributes.Contains(x)).ToList()).ToList();
            attributes = new List<Attribute>();
            attributes.AddRange(Extensions.Clone<Attribute>(Database.Instance.attributes.Where(x => x.isCore).ToList()));
            attributes.AddRange(aux);
        }
    }
}