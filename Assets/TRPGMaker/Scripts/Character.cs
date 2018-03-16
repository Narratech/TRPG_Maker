
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Inventory))]
[Serializable]
public class Character: MonoBehaviour{

    public string name;
    public Inventory inventory;
    public List<Slot> Slots;
    public List<Attribute> attributes;
    public SpecializedClass specializedClass;
    public int altura;
    public int distancia;
}