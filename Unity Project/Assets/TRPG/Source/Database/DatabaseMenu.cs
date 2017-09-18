using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DatabaseMenu
    {
    [MenuItem("TRPG/Create/Main database (empty)",false,-4)]
	[MenuItem("Assets/Create/Databases/Main database (empty)",false,0)]
	public static void createEmptyDatabaseAsset()
        {
        Database d=ScriptableObject.CreateInstance<Database>();
        AssetDatabase.CreateAsset(d,"Assets/TRPG/database.asset");
        AssetDatabase.SaveAssets();
		Selection.activeObject=d;
        d.createEmptyDatabase();
	    }

    [MenuItem("TRPG/Create/Main database (demo)",false,-3)]
    [MenuItem("Assets/Create/Databases/Main database (demo)",false,1)]
	public static void createDemoDatabaseAsset()
        {
        Database d=ScriptableObject.CreateInstance<Database>();
        AssetDatabase.CreateAsset(d,"Assets/TRPG/database.asset");
        AssetDatabase.SaveAssets();
		Selection.activeObject=d;  
        d.createDemoDatabase();
	    }
    
    [MenuItem("TRPG/Create/Character database (empty)",false,-2)]
    [MenuItem("Assets/Create/Databases/Character database (empty)",false,2)]
	public static void createEmptyCharacterDatabaseAsset()
        {
        CharacterManager cm=ScriptableObject.CreateInstance<CharacterManager>();
        AssetDatabase.CreateAsset(cm,"Assets/TRPG/characters.asset");
        AssetDatabase.SaveAssets();
		Selection.activeObject=cm;
	    }
    
    [MenuItem("TRPG/Create/Character database (demo)",false,-1)]
    [MenuItem("Assets/Create/Databases/Character database (demo)",false,3)]
	public static void createDemoCharacterDatabaseAsset()
        {
        CharacterManager cm=ScriptableObject.CreateInstance<CharacterManager>();
        AssetDatabase.CreateAsset(cm,"Assets/TRPG/characters.asset");
        AssetDatabase.SaveAssets();
		Selection.activeObject=cm;
        cm.addDemoCharacters();
	    }
    }
