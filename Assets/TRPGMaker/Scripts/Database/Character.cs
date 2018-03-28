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
        specializedClass = new List<SpecializedClass>();
    }

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