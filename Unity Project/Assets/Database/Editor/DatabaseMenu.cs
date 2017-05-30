using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DatabaseMenu
    {
	[MenuItem("Assets/Create/TRPG Database/Empty")]
	public static void createTRPGDatabaseAsset1()
        {
        Database d=ScriptableObject.CreateInstance<Database>();
        AssetDatabase.CreateAsset(d,"Assets/database.asset");
        AssetDatabase.SaveAssets();
		Selection.activeObject=d;
        d.createEmptyDatabase();
	    }
    [MenuItem("Assets/Create/TRPG Database/TFG Demo")]
	public static void createTRPGDatabaseAsset2()
        {
        Database d=ScriptableObject.CreateInstance<Database>();
        AssetDatabase.CreateAsset(d,"Assets/database.asset");
        AssetDatabase.SaveAssets();
		Selection.activeObject=d;  
        d.createDemoDatabase();
	    }
    }
