using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class CharacterEditorWindow: EditorWindow
    {
    #region Attributes
    // Database and CharacterManager instances
    Database d;
    CharacterManager cm;
    // Constants
    private static int MAX_SLOTS=48;  // Maximum number of slots are 24 from Class an 24 from Specialization
    // Basic info
    bool _foldoutBasicInfo;
    string _nameId;
    string _description;
        //Sprite _avatar;
    // Basic attributes
    bool _foldoutBasicAttr;
    int _exp;
    int _maxHP;
    int _maxMP; 
    // Core attributes
    bool _foldoutCoreAttr;
    int _coreCount;  // number of core attributes which exist on database
    List<string> _coreAttribsIdList;  // List of core attributes id in database (excluding basic)
    List<int> _coreAttribsValueList;  // Array of core attributes (excluding basic) which user puts in IntFields
    // Class/Specialization
    bool _foldoutClassSpec;
    List<string> _classList;  // List of 'Spec.nameId' for classes (have no father)
    string[] _classArray;  // The same list in array format for the Editor
    int _selectedClass;  // Position in Popup for selected class
    List<string> _specList;  // List of 'Spec.nameId' for specializations (based on the father selected)
    string[] _specArray;  // The same list in array format for the Editor
    int _selectedSpec;  // Position in Popup for selected specialization
    //List<string> _tagMatchingModeList;  // List which says how tags match with items
    //string[] _tagMatchingModeArray;  // The same list in array format for the Editor
    //int _selectedTagMatchingMode;  // Position in Popup for selected matching mode
    // Items
    bool _foldoutItems;
    bool _foldoutItemsTags;
    bool _foldoutItemsPossible;
    int _itemSlotsCount;  // Number of available slots for items
    List<List<string>> _itemTagsList;  // Tags that filter the possible item
    List<List<string>> _possibleItemsList;  // Possible items that could be chosen
    List<string[]> _possibleItemsArray;  // The same list in array format for the Editor
    List<int> _selectedItemInEachOptionList;  // Position in Popup for selected Item in each option  
    int[] _selectedItemInEachOptionArray;  // Position in Popup for selected Item in each option
    List<bool> _equipedItemsList;
    // Passives
    bool _foldoutPassives;
    bool _foldoutPassivesTags;
    bool _foldoutPassivesPossible;
    int _passiveSlotsCount;  // Number of available slots for passives
    List<List<string>> _passiveTagsList;  // Tags "When" and "To Whom" that filter the possible passive together
    List<List<string>> _possiblePassivesList;  // Possible passives that could be chosen
    List<string[]> _possiblePassivesArray;  // The same list in array format for the Editor 
    List<int> _selectedPassiveInEachOptionList;  // Position in Popup for selected Passive in each option 
    int[] _selectedPassiveInEachOptionArray;  // Position in Popup for selected Passive in each option
    #endregion

    #region Constructor
    public void OnEnable()
        { 
        cm=CharacterManager.Instance;       
        d=Database.Instance;
        loadInfoFromDatabase();
        createEmptyCharacter();
        }
    #endregion

    #region Methods
    #region Load info
    void loadInfoFromDatabase()
        // Called when you open the Editor, when you push NEW or after you ADD a Character
        {
        loadCoreAttribs();
        loadInfoForClass();
        loadInfoForSpec();
        loadInfoForItems();
        loadInfoForPassives(); 
        loadInfoForFoldouts(); 
        }

    void loadCoreAttribs()
        {
        _coreCount=0;
        _coreAttribsIdList=new List<string>();
        _coreAttribsValueList=new List<int>();
        Dictionary<string,AttributeTRPG> attrList=new Dictionary<string,AttributeTRPG>(d.Attributes);
        foreach (KeyValuePair<string,AttributeTRPG> result in attrList) 
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
        _itemSlotsCount=0;
        _itemTagsList=new List<List<string>>();
        _possibleItemsList=new List<List<string>>();
        _possibleItemsArray=new List<string[]>();
        _selectedItemInEachOptionList=new List<int>();
        _selectedItemInEachOptionArray=_selectedItemInEachOptionList.ToArray();
        _equipedItemsList=new List<bool>();
        }

    void loadInfoForPassives()
        {
        _passiveSlotsCount=0;
        _passiveTagsList=new List<List<string>>();
        _possiblePassivesList=new List<List<string>>();
        _possiblePassivesArray=new List<string[]>();
        _selectedPassiveInEachOptionList=new List<int>();
        _selectedPassiveInEachOptionArray=_selectedPassiveInEachOptionList.ToArray();
        }

    void loadInfoForFoldouts()
        {
        // General foldouts
        _foldoutBasicInfo=true;
        _foldoutBasicAttr=true;
        _foldoutCoreAttr=true;
        _foldoutClassSpec=true;
        _foldoutItems=true;
        _foldoutPassives=true;
        // Item foldouts
        _foldoutItemsTags=true;
        _foldoutItemsPossible=true;
        // Passive foldouts
        _foldoutPassivesTags=true;
        _foldoutPassivesPossible=true;
        }
    #endregion
   
    #region Update info
    void updateInfoForSpec()
        // Called when you select a class in the Class Popup in the Class/Specialization zone
        {
        _selectedSpec=0;
        _specList=new List<string>();
        _specList.Insert(0,"Choose Specialization...");
        foreach (SpecConfig result in d.Specs[_classList[_selectedClass]].AllowedSlots.SpecCfg)
            {
            for (int i=0; i<result.specIds.Count; i++) 
                {
                _specList.Add(result.specIds[i]);
                }
            }
        _specArray=_specList.ToArray();  // List of 'Spec.nameId' for classes (have no father)
        _itemTagsList=new List<List<string>>();
        _passiveTagsList=new List<List<string>>();  
        }
    
    void updateInfoForItems()
        // Called when you select a specialization in the Spec Popup in the Class/Specialization zone
        {
        // The tags which define what item could fit in every slot
        _itemTagsList=new List<List<string>>();
        foreach (ItemConfig result in d.Specs[_classList[_selectedClass]].AllowedSlots.ItemCfg)  // Items in Class
            {
            _itemTagsList.Add(result.itemIds);
            _itemSlotsCount++;
            }
        foreach (ItemConfig result in d.Specs[_specList[_selectedSpec]].AllowedSlots.ItemCfg)  // Items in Specialization
            {
            _itemTagsList.Add(result.itemIds);
            _itemSlotsCount++;
            }
        // The items which could fit in every slot
        foreach (List<string> result in _itemTagsList)
            {
            List<string> tagsSlot=new List<string>(result);
            tagsSlot.Sort();
            List<string> matchedItems=new List<string>();
            foreach (ItemTemplate result2 in d.Items.Values)
                {
                List<string> tagsItemDB=new List<string>(result2.Tags);
                tagsItemDB.Sort();
                tagMatching("Exact",result2,tagsSlot,tagsItemDB,ref matchedItems);
                /*
                int i=0;
                bool isMatching=true;
                while (isMatching && i<tagsSlot.Count)
                    {
                    if (tagsItemDB[i]!=tagsSlot[i])
                        isMatching=false;
                    i++;
                    }
                if (isMatching)
                    matchedItems.Add(result2.NameId);
                */
                }
            matchedItems.Insert(0,"<EMPTY>");
            _possibleItemsList.Add(matchedItems); 
            }
        foreach (List<string> result in _possibleItemsList)
            {
            _selectedItemInEachOptionList.Add(0);
            _possibleItemsArray.Add(result.ToArray());
            }
        _selectedItemInEachOptionArray=_selectedItemInEachOptionList.ToArray();
        for (int i=0; i<_itemSlotsCount; i++)
            {
            _equipedItemsList.Add(false);  
            }
        }

    void updateInfoForPassives()
        // Called when you select a specialization in the Spec Popup in the Class/Specialization zone
        {
        // The tags which define what passive could fit in every slot
        _passiveTagsList=new List<List<string>>();  
        foreach (Dictionary<string,string> result in d.Specs[_classList[_selectedClass]].AllowedSlots.PassiveCfg)  // Passives in Class
            {
            List<string> tagPair=new List<string>(); 
            tagPair.Add(result["When"]); 
            tagPair.Add(result["To whom"]);
            _passiveTagsList.Add(tagPair);
            _passiveSlotsCount++;
            }
        foreach (Dictionary<string,string> result in d.Specs[_specList[_selectedSpec]].AllowedSlots.PassiveCfg)  // Passives in Spec
            {
            List<string> tagPair=new List<string>();
            tagPair.Add(result["When"]);
            tagPair.Add(result["To whom"]);
            _passiveTagsList.Add(tagPair);
            _passiveSlotsCount++;
            }
        /*
        // GUARDAR EN UN DICCIONARIO DE ITEMS LOS ITEMS SELECCIONADOS
        foreach (Dictionary<string,string> result in ESE DICCIONARIO CREADO LOS QUE TENGAN PASIVA)  // Passives in Items
            {
            List<string> tagPair=new List<string>();
            tagPair.Add(result["When"]);
            tagPair.Add(result["To Whom"]);
            _passiveTagsList.Add(tagPair);
            _passiveSlotsCount++;
            }
        */
        // The passives which could fit in every slot
        foreach (List<string> result in _passiveTagsList)
            {
            List<string> tagsSlot=new List<string>(result);
            tagsSlot.Sort();
            List<string> matchedPassives=new List<string>();
            foreach (PassiveTemplate result2 in d.Passives.Values)
                {
                List<string> tagsPassiveDB=new List<string>(result2.Tags);
                tagsPassiveDB.Sort();
                // TO DO: create a Popup to select "Exact", "OR"... and call like 'tagMatching(_tagMatchingModeArray[_selectedTagMatchingMode],...)'
                tagMatching("Exact",result2,tagsSlot,tagsPassiveDB,ref matchedPassives);
                }
            matchedPassives.Insert(0,"<EMPTY>");
            _possiblePassivesList.Add(matchedPassives); 
            }
        foreach (List<string> result in _possiblePassivesList)
            {
            _selectedPassiveInEachOptionList.Add(0);
            _possiblePassivesArray.Add(result.ToArray());
            }
        _selectedPassiveInEachOptionArray=_selectedPassiveInEachOptionList.ToArray();
        /*
        for (int i=0; i<_passiveSlotsCount; i++)
            {
            _equipedPassivesList.Add(false);  
            }
        */
        }

    void tagMatching(string selectedTagMatchingMode, Template thing, List<string> tagsSlot, List<string> tagsThingDB, ref List<string> matchedThings)
        {
        if (selectedTagMatchingMode=="Exact")
            // Matching items with the exact same tags
            {
            int i=0;
            bool isMatching=true;
            while (isMatching && i<tagsSlot.Count)
                {
                if (tagsThingDB[i]!=tagsSlot[i])
                    isMatching=false;
                i++;
                }
            if (isMatching)
                matchedThings.Add(thing.NameId);
            }
        else if (selectedTagMatchingMode=="OR")
            // Matching items with at least one similar tag
            {
            }
        }
    #endregion

    #region Character: createEmptyCharacter(), loadSelectedCharacter(), constructCurrentCharacter()
    private void createEmptyCharacter()
        {
        _nameId="Enter the name of your character (will be identified by it)";
        _description="Enter the description of your character";
        }

    private void constructCurrentCharacter()
        // Constructs a new CharacterSheet according to whatever is shown in the fields and lets the CharacterManager
        // store it
        {
        Dictionary<string,AttributeTRPG> actualAttributes=new Dictionary<string, AttributeTRPG>(d.Attributes); 
        // Resetting all attributes to 0
        foreach (KeyValuePair<string, AttributeTRPG> attr in actualAttributes)
            {
            attr.Value.value=0;
            }
        // Filling basic attributes
        actualAttributes["EXP"].value=_exp;
        actualAttributes["HPS"].value=_maxHP;
        actualAttributes["MPS"].value=_maxMP; 
        // Filling core attributes
        for (int i=0; i<_coreAttribsIdList.Count; i++)
            {
            actualAttributes[_coreAttribsIdList[i]].value=_coreAttribsValueList[i]; 
            }
        // Filling the class and specialization     
        SpecTemplate actualClass=d.Specs[_classArray[_selectedClass]];
        // SpecTemplate actualClass=ScriptableObject.CreateInstance<SpecTemplate>().Init("Human","Humans are flesh and bones",true,humanFormulas,humanSlotsConfig);
        SpecTemplate actualSpec=d.Specs[_specArray[_selectedSpec]];
        // Filling the items
        List<ItemTemplate> actualItems=new List<ItemTemplate>();
        for (int i=0; i<_itemSlotsCount; i++)
            {
            List<string> auxList=_possibleItemsList[i];
            if (_equipedItemsList[i] && auxList[_selectedItemInEachOptionArray[i]]!="<EMPTY>")
                actualItems.Add(d.Items[auxList[_selectedItemInEachOptionArray[i]]]); 
            }
        // Filling the passives
        List<PassiveTemplate> actualPassives=new List<PassiveTemplate>();
        for (int i=0; i<_passiveSlotsCount; i++)
            {
            List<string> auxList=_possiblePassivesList[i]; 
            if (auxList[_selectedPassiveInEachOptionArray[i]]!="<EMPTY>") 
                actualPassives.Add(d.Passives[auxList[_selectedPassiveInEachOptionArray[i]]]); 
            }
        CharacterSheet actualSheet=ScriptableObject.CreateInstance<CharacterSheet>().Init(_nameId,_description,actualAttributes,actualClass,actualSpec,actualItems,actualPassives);
        cm.addCharacter(actualSheet); 
        }

    private void constructDemoCharactersInCharacterManager()
        // Constructs four CharacterSheet to make a demo. Previously, Main Demo Database should be created
        {
        cm.addDemoCharacters(); 
        }
    #endregion

    #region GUI: CreateWindow(), OnGUI()
    [MenuItem("TRPG/Characters",false,7)] 
    static void CreateWindow()
        {
        CharacterEditorWindow window=(CharacterEditorWindow)EditorWindow.GetWindow(typeof(CharacterEditorWindow));
        window.Show();
        }

    void OnGUI()
        {
        EditorGUILayout.BeginVertical("Button");
        onGUIBasicInfo();
        onGUIBasicAttributes();
        onGUICoreAttributes();
        onGUIClassSpecialization();
        onGUIItems();
        onGUIPassives();
        EditorGUILayout.EndVertical();
        onGUIButtons();
        }
    
    void onGUIBasicInfo()
        {  
        EditorGUILayout.BeginVertical("Box");
        _foldoutBasicInfo=EditorGUILayout.Foldout(_foldoutBasicInfo, "Basic info");
        if (_foldoutBasicInfo)
            {
            _nameId=EditorGUILayout.TextField("Name (id)",_nameId);
            _description=EditorGUILayout.TextField("Description",_description,GUILayout.Height(100));
                //_avatar=...
                //_associatedEntity=...
            }
        EditorGUILayout.EndVertical();
        }

    void onGUIBasicAttributes()
        {
        EditorGUILayout.BeginVertical("Box");
        _foldoutBasicAttr=EditorGUILayout.Foldout(_foldoutBasicAttr, "Basic attributes");
        if (_foldoutBasicAttr)
            {
            _exp=EditorGUILayout.IntField("Experience",_exp);
            _maxHP=EditorGUILayout.IntField("Max HP",_maxHP);
            _maxMP=EditorGUILayout.IntField("Max MP",_maxMP);
            }
        EditorGUILayout.EndVertical();
        }

    void onGUICoreAttributes()
        {
        EditorGUILayout.BeginVertical("Box");
        _foldoutCoreAttr=EditorGUILayout.Foldout(_foldoutCoreAttr, "Core attributes ("+_coreCount+")");
        if (_foldoutCoreAttr)
            {
            for (int i=0; i<_coreCount; i++)
                {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(_coreAttribsIdList[i]+":");
                _coreAttribsValueList[i]=EditorGUILayout.IntField(_coreAttribsValueList[i]);
                EditorGUILayout.EndHorizontal();  
                }
            }
        EditorGUILayout.EndVertical();
        }

    void onGUIClassSpecialization()
        {
        EditorGUILayout.BeginVertical("Box");
        _foldoutClassSpec=EditorGUILayout.Foldout(_foldoutClassSpec, "Class / Specialization");
        if (_foldoutClassSpec)
            {
            EditorGUI.BeginChangeCheck();
            _selectedClass=EditorGUILayout.Popup(_selectedClass,_classArray,GUILayout.Width(150));
            if(EditorGUI.EndChangeCheck())
                { 
                if (_selectedClass==0)
                    {
                    loadInfoForSpec();
                    loadInfoForItems();
                    loadInfoForPassives();
                    }
                else if (_selectedClass!=0)
                    {
                    loadInfoForItems();
                    loadInfoForPassives();
                    updateInfoForSpec();
                    }
                }
            EditorGUI.BeginChangeCheck();
            _selectedSpec=EditorGUILayout.Popup(_selectedSpec,_specArray,GUILayout.Width(150));
            if(EditorGUI.EndChangeCheck())
                { 
                if (_selectedSpec==0)
                    {
                    loadInfoForItems();
                    loadInfoForPassives();
                    }
                else if (_selectedSpec!=0)
                    {
                    loadInfoForItems();
                    loadInfoForPassives();
                    updateInfoForItems();
                    updateInfoForPassives();
                    }
                }
            }
        EditorGUILayout.EndVertical(); 
        }

    void onGUIItems()
        {
        EditorGUILayout.BeginVertical("Box");
        _foldoutItems=EditorGUILayout.Foldout(_foldoutItems, "Items");
        if (_foldoutItems)
            // External foldout: shows the Items box
            {
            /*
            _foldoutItemsTags=EditorGUILayout.Foldout(_foldoutItemsTags, "Tags for filtering");
            if (_foldoutItemsTags)
                // Internal foldout: shows in TextField elements the tags used to match Items 
                {
                foreach (List<string> result in _itemTagsList)
                    {
                    EditorGUILayout.BeginVertical();
                    string printable="";
                    foreach (string result2 in result)
                        {
                        printable=printable+" —"+result2;
                        }
                    EditorGUILayout.TextField(printable);
                    EditorGUILayout.EndVertical();
                    }
                }
            _foldoutItemsPossible=EditorGUILayout.Foldout(_foldoutItemsPossible, "Possible items");
            if (_foldoutItemsPossible)
                // Internal foldout: shows in TextField elements all the Items that match the tags 
                {
                foreach (List<string> result in _possibleItemsList)
                    {
                    EditorGUILayout.BeginVertical();
                    string printable="";
                    foreach (string result2 in result)
                        {
                        printable=printable+" —"+result2;
                        }
                    EditorGUILayout.TextField(printable);
                    EditorGUILayout.EndVertical();
                    }
                }
            */
            // Shows in Popup and Toggle elements all the Items suitable to select
            EditorGUILayout.BeginHorizontal();
            for (int i=0; i<_itemSlotsCount; i++)
                {
                _selectedItemInEachOptionArray[i]=EditorGUILayout.Popup(_selectedItemInEachOptionArray[i],_possibleItemsArray[i],GUILayout.Width(70));
                _equipedItemsList[i]=EditorGUILayout.Toggle(_equipedItemsList[i]);
                if ((i%6==5))
                    {
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();  
                    }
                }
            EditorGUILayout.EndHorizontal(); 
            }
        EditorGUILayout.EndVertical();
        }

    void onGUIPassives()
        {
        EditorGUILayout.BeginVertical("Box");
        _foldoutPassives=EditorGUILayout.Foldout(_foldoutPassives, "Passives");
        if (_foldoutPassives)
            // External foldout: shows the Passives box
            {
            /*
            _foldoutPassivesTags=EditorGUILayout.Foldout(_foldoutPassivesTags, "Tags for filtering");
            if (_foldoutPassivesTags)
                // Internal foldout: shows in TextField elements the tags used to match Passives
                {
                foreach (List<string> result in _passiveTagsList)
                    {
                    EditorGUILayout.BeginVertical();
                    string printable="";
                    foreach (string result2 in result)
                        {
                        printable=printable+" —"+result2;
                        }
                    EditorGUILayout.TextField(printable);
                    EditorGUILayout.EndVertical();
                    }
                }
            _foldoutPassivesPossible=EditorGUILayout.Foldout(_foldoutPassivesPossible, "Possible passives");
            if (_foldoutPassivesPossible)
                // Internal foldout: shows in TextField elements all the Passives that match the tags
                {
                foreach (List<string> result in _possiblePassivesList)
                    {
                    EditorGUILayout.BeginVertical();
                    string printable="";
                    foreach (string result2 in result)
                        {
                        printable=printable+" —"+result2;
                        }
                    EditorGUILayout.TextField(printable);
                    EditorGUILayout.EndVertical();
                    }
                }
            */
            // Shows in Popup and Toggle elements all the Items suitable to select
            EditorGUILayout.BeginVertical();
            for (int i=0; i<_passiveSlotsCount; i++) 
                {
                _selectedPassiveInEachOptionArray[i]=EditorGUILayout.Popup(_selectedPassiveInEachOptionArray[i],_possiblePassivesArray[i],GUILayout.Width(70));
                }
            EditorGUILayout.EndVertical(); 
            }
        EditorGUILayout.EndVertical();
        }

    void onGUIButtons()
        {
        EditorGUILayout.BeginHorizontal("Box");
        if (GUILayout.Button("NEW",GUILayout.Width(80),GUILayout.Height(80)))
            {
            loadInfoFromDatabase();
            createEmptyCharacter();
            }
        else if (GUILayout.Button("ADD",GUILayout.Width(80),GUILayout.Height(80)))
            {
            constructCurrentCharacter();
            loadInfoFromDatabase();
            createEmptyCharacter();
            }
        else if (GUILayout.Button("DEMO",GUILayout.Width(80),GUILayout.Height(80)))
            {
            constructDemoCharactersInCharacterManager();
            loadInfoFromDatabase();
            createEmptyCharacter();
            }
        }
    #endregion
    #endregion
    }
