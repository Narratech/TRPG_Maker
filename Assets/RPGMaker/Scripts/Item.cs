using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "RPG/Item", order = 1)]
public class Item : ScriptableObject {

    public string Name = "Enter item name";
    public string Description = "Enter item description";
    public SlotType[] SlotType;
}
