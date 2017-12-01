using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Inventory", menuName = "RPG/Inventory", order = 1)]
public class Inventory : ScriptableObject
{

    public string Name = "Enter item name";
    public string Description = "Enter item description";
    public SlotType SlotType;

    public static Inventory _instance;

    public static Inventory Instance
    {
        //get { return _instance == null ? _instance = new CharacterManager() : _instance; }
        get { return _instance == null ? _instance = UnityEditor.AssetDatabase.LoadAssetAtPath<Inventory>("Assets/RPG/Resources/Inventory.asset") : _instance; }
    }
}