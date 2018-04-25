using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Database : ScriptableObject
{
    private static Database _instance;
    public List<Attribute> attributes;
    public List<string> tags;
    public List<string> slotTypes;
    public List<Modifier> items;
    public List<SpecializedClass> specializedClasses;
    public List<Character> characters;
    public List<Team> teams;
    public List<Skills> skills;
    [SerializeField]
    private TRPGOptions _TRPGOptions;

    public TRPGOptions battleOptions
    {
        get
        {
            return _TRPGOptions;
        }
        set {
            _TRPGOptions = value;
        }
    }

    public Database()
    {
        attributes = new List<Attribute>();
        tags = new List<string>();
        slotTypes = new List<string>();
        items = new List<Modifier>();
        specializedClasses = new List<SpecializedClass>();
        characters = new List<Character>();
    }

    public static Database Instance
    {
        get { return _instance == null ? _instance = UnityEditor.AssetDatabase.LoadAssetAtPath<Database>("Assets/TRPGMaker/Database/Database.asset") : _instance; }
    }
}
