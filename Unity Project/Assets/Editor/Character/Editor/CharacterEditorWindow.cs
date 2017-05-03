using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CharacterEditorWindow: EditorWindow
    {
    #region Attributes
    // Database instance
    Database d;
    // Basic info
    string _nameId;
    int _weight;
    int _height;
    string _description;
        //Sprite _avatar;
        //IsoEntity _associedEntity;
    // Basic attributes
    int _exp;
    int _maxHP;
    int _maxMP;
    // Core attributes
    int _coreCount;  // number of core attributes (excluding core core) which exist on database
    List<string> _coreAttribsIdList;  // List of core attributes id in database (excluding basic)
    List<int> _coreAttribsValueList;  // Array of core attributes (excluding basic) which user puts in IntFields
    // Class/Specialization
    List<string> _classList;  // List of 'Spec.nameId' for classes (have no father)
    string[] _classArray;  // The same list in array format for the Editor
    int _selectedClass;  // Position in Popup for selected class
    List<string> _specList;  // List of 'Spec.nameId' for specializations (based on the father selected)
    string[] _specArray;  // The same list in array format for the Editor
    int _selectedSpec;  // Position in Popup for selected specialization
    // Items
    List<List<string>> _itemTagsList;  // Tags that filter the possible item
    List<List<string>> _possibleItemsList;  // Possible items that could be chosen
    string[][] _possibleItemsArray;  // The same list in array format for the Editor
    int[] _selectedItemInEachOption;  // Position in Popup for selected Item in each option
    // Passives
    List<List<string>> _passiveTagsList;  // Tags that filter the possible passive
    List<List<string>> _possiblePassivesList;  // Possible passives that could be chosen
    string[][] _possiblePassivesArray;  // The same list in array format for the Editor 
    int[] _selectedPassiveInEachOption;  // Position in Popup for selected Passive in each option
    #endregion

    #region Constructor
    public CharacterEditorWindow()
        {        
        d=Database.Instance;
        loadInfoFromDatabase();
        createEmptyCharacter();
        }
    #endregion

    #region Methods
    #region Database: loadInfoFromDatabase(), auxiliary methods
    void loadInfoFromDatabase()        
        // Called when you open the Editor, when you select <NEW> on Character Popup, or when you finish an 
        // ADD/MODIFY/DELETE operation. Then you have in local variables and structures the updated and 
        // necessary info from database to create a new SpecTemplate with the Editor
        {
        loadCoreAttribs();
        loadInfoForClass();
        loadInfoForSpec();
        loadInfoForItems();
        loadInfoForPassives();
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

    void loadInfoForClass()
        {
        _classList=new List<string>();
        _classList.Insert(0,"Choose Class...");
        foreach (SpecTemplate result in d.Specs.Values)
            {
            if (result.IsBasicClass)
                _classList.Add(result.NameId);
            }
        _classArray=_classList.ToArray();  // List of 'Spec.nameId' for classes (have no father)
        _selectedClass=0;  // Position in Popup for selected class
        }

    void loadInfoForSpec()
        {
        _specList=new List<string>();
        _specList.Insert(0,"Choose Specialization...");
        _specArray=_specList.ToArray();
        _selectedSpec=0;
        }

    void loadInfoForItems()
        {

        }

    void loadInfoForPassives()
        {

        }

    void loadInfoForSpec2()
        {
        _specList=new List<string>();
        _specList.Insert(0,"Choose Specialization...");
        foreach (SpecConfig result in d.Specs[_classList[_selectedClass]].AllowedSlots.SpecCfg)
            {
            foreach (string result2 in result.specIds)
                {                
                _specList.Add(result2);
                }
            }
        _specArray=_specList.ToArray();  // List of 'Spec.nameId' for classes (have no father)
        _selectedSpec=0;  // Position in Popup for selected class
        }
    #endregion

    #region Character: createEmptyCharacter(), loadSelectedCharacter(), constructCurrentCharacter()
    private void createEmptyCharacter()
        {
        _nameId="Enter the name of your character (will be identified by it)";
        _description="Enter the description of your character";
        }

    private void loadSelectedCharacter()
        {

        }

    private void constructCurrentCharacter()
        {

        }
    #endregion

    #region GUI: CreateWindow(), OnGUI()
    [MenuItem("TRPG/Game/Create Character")]
    static void CreateWindow()
        {
        CharacterEditorWindow window=(CharacterEditorWindow)EditorWindow.GetWindow(typeof(CharacterEditorWindow));
        window.Show();
        }

    void OnGUI()
        {
        onGUIBasicInfo();
        onGUIBasicAttributes();
        onGUICoreAttributes();
        onGUIClassSpecialization();
        onGUIItems();
        onGUIPassives();
        }
    
    void onGUIBasicInfo()
        {        
        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.LabelField("Basic info:",EditorStyles.boldLabel);
        _nameId=EditorGUILayout.TextField("Name (id)",_nameId);
        _weight=EditorGUILayout.IntField("Weight",_weight);
        _height=EditorGUILayout.IntField("Height",_height);
        _description=EditorGUILayout.TextField("Description",_description,GUILayout.Height(100));
            //_avatar=...
            //_associatedEntity=...
        EditorGUILayout.EndVertical();
        }

    void onGUIBasicAttributes()
        {
        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.LabelField("Basic attributes:",EditorStyles.boldLabel);
        _exp=EditorGUILayout.IntField("Experience",_exp);
        _maxHP=EditorGUILayout.IntField("Max HP",_maxHP);
        _maxMP=EditorGUILayout.IntField("Max MP",_maxMP);
        EditorGUILayout.EndVertical();
        }

    void onGUICoreAttributes()
        {
        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.LabelField("Core attributes ("+_coreCount+"):",EditorStyles.boldLabel);
        for (int i=0; i<_coreCount; i++)
            {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(_coreAttribsIdList[i]+":");
            _coreAttribsValueList[i]=EditorGUILayout.IntField(_coreAttribsValueList[i]);
            EditorGUILayout.EndHorizontal();  
            }
        EditorGUILayout.EndVertical();
        }

    void onGUIClassSpecialization()
        {
        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.LabelField("Class / Specialization:",EditorStyles.boldLabel);
        _selectedClass=EditorGUILayout.Popup(_selectedClass,_classArray,GUILayout.Width(150));
        _selectedSpec=EditorGUILayout.Popup(_selectedSpec,_specArray,GUILayout.Width(150)); 
        EditorGUILayout.EndVertical();
        }

    void onGUIItems()
        {
        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.LabelField("Items:",EditorStyles.boldLabel);
        EditorGUILayout.EndVertical();

        }

    void onGUIPassives()
        {
        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.LabelField("Passives:",EditorStyles.boldLabel);
        EditorGUILayout.EndVertical();

        }
    #endregion
    #endregion
    }
