using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CharacterEditorWindow: EditorWindow
    {
    #region Attributes
    // Database instance
    Database d;
    // Constants
    //private static int MAX_CORE_ATTRIBUTES=6;
    // Typical attributes
    string _nameId;
    int _weight;
    int _height;
    string _description;
    // Core core attributes
    int _exp;
    int _maxHP;
    int _maxMP;
    // Core attributes
    int _coreCount;  // number of core attributes (excluding core core) which exist on database
    List<string> _coreAttribsIdList;  // List of core attributes id in database (excluding core core)
    //string[] _coreAttribsIdArray;  // The same list in array format for the Editor
    List<int> _coreAttribsValueList;  // Array of core attributes (excluding core core) which user puts in IntFields
    //int[] _coreAttribsValueArray;  // The same list in array format for the Editor
    // Specs
    List<PassiveTemplate> _passivesList;
    // Items
    List<ItemTemplate> _itemTemplateList;
    // Passives
    List<PassiveTemplate> _passiveTemplateList;
    #endregion

    #region Constructor
    // Constructor
    public CharacterEditorWindow()
        {        
        d=Database.Instance;
        loadInfoFromDatabase();
        createEmptyCharacterEditor();
        }
    #endregion

    #region Methods
    #region GUI: CreateWindow(), OnGUI()
    [MenuItem("TRPG/Game/Create Character")]
    static void CreateWindow()
        {
        CharacterEditorWindow window=(CharacterEditorWindow)EditorWindow.GetWindow(typeof(CharacterEditorWindow));
        window.Show();
        }

    void OnGUI()
        {
        #region Events
        // Events
        // Left mouse click on any button (ADD/MODIFY/DELETE)
        /*if(Event.current.isMouse && Event.current.type == EventType.mouseDown && Event.current.button==0)
            loadItemsFromDatabase();*/
        #endregion
        #region Descriptive attributes zone
        /////////////////////////////
        EditorGUILayout.BeginHorizontal("Box");
        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.LabelField("Basic character info",EditorStyles.boldLabel);
        _nameId=EditorGUILayout.TextField("Name (id)",_nameId);
        _weight=EditorGUILayout.IntField("Weight",_weight);
        _height=EditorGUILayout.IntField("Height",_height);
        _description=EditorGUILayout.TextField("Description",_description,GUILayout.Height(100));
        EditorGUILayout.EndVertical();
        #endregion
        #region Character core core attributes (_exp,_maxHP,_maxMP)
        ///////////////////////////////////////////////////////////
        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.LabelField("Core core attributes",EditorStyles.boldLabel);
        _exp=EditorGUILayout.IntField("Experience",_exp);
        _maxHP=EditorGUILayout.IntField("Max HP",_maxHP);
        _maxMP=EditorGUILayout.IntField("Max MP",_maxMP);
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
        #endregion
        #region Character core attributes (different per game)
        //////////////////////////////////////////////////////
        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.LabelField("Core attributes ("+_coreCount+")",EditorStyles.boldLabel);
        for (int i=0; i<_coreCount; i++)
            {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(_coreAttribsIdList[i]+":");
            _coreAttribsValueList[i]=EditorGUILayout.IntField(_coreAttribsValueList[i]);
            EditorGUILayout.EndHorizontal();  
            }
        EditorGUILayout.EndVertical();
        #endregion
        #region Specs
        /////////////
        #endregion
        #region Items
        /////////////
        #endregion
        #region Passives
        ////////////////
        #endregion
        }
    #endregion

    #region Load Info
    void createEmptyCharacterEditor()
        {
        _nameId="Enter the name of your character (will be identified by it)";
        _description="Enter the description of your character";
        }

    void loadInfoFromDatabase()
        {
        loadCoreAttribs();
        }

    void loadCoreAttribs()
        {
        _coreCount=0;
        _coreAttribsIdList=new List<string>();
        _coreAttribsValueList=new List<int>();
        Dictionary<string,Attribute> attrList=new Dictionary<string,Attribute>(d.Attributes);
        foreach (KeyValuePair<string,Attribute> result in attrList)
            {
            if (result.Key!="EXP" && result.Key!="HPS" && result.Key!="MPS" && result.Value.isCore)
                {
                _coreCount++;
                _coreAttribsIdList.Add(result.Key);
                _coreAttribsValueList.Add(0);
                }
            }
        }
    #endregion
    #endregion
    }
