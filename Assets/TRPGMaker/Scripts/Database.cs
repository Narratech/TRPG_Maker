using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Database : ScriptableObject
{
    private static Database _instance = null;
    public List<Attribute> attributes;
    public List<string> tags;

    public Database()
    {
        attributes = new List<Attribute>();
        tags = new List<string>();
    }

    public static Database Instance
    {
        get { return _instance == null ? _instance = UnityEditor.AssetDatabase.LoadAssetAtPath<Database>("Assets/TRPGMaker/Source/Database.asset") : _instance; }
    }
}
