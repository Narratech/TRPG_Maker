using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class ItemEditorWindow: EditorWindow
    {
    #region Attributes
    // Database instance
    Database d;
    // Constants
    private static int MAX_FORMULAS=10;
    private static int MAX_SLOTS=10;
    // Current item values and container for the values
    string itemNameId;  // The name of the item identifies it
    string itemDescription;  // Description of the item
    List<Formula> itemFormulas;  // Every formula that modifies attributes for this Template
    List<Template> itemSlots;  // Templates which this Template has
    ItemTemplate currentItemTemplate;  // Temporal container for the ItemTemplate we are editing
    // Item related
    List<string> itemsInDatabaseList;  // List of item 'Item.nameId' in database
    string[] itemsInDatabaseArray;  // The same list in array format for the Editor
    int selectedItem;  // Position in Popup for selected item to add/modify/delete 
    // Formula|Item related
    List<string> attribsInDatabaseList;  // List of existing 'Attribute.id' in database
    string[] attribsInDatabaseArray;  // The same list in array format for the Editor
    List<int> formulasCountForEachItemList;  // Number of formulas each item in database has
    int[] formulasCountForEachItemArray;  // The same list in array format for the Editor
    int formulaCount;  // Number of formulas the item being added/modified/deleted currently has
    int[] selectedAttribInEachFormula;  // Position in Popup for selected Attribute in each formula
    string[] formulasArray;  // Formulas themselves
    // Slot|Item related
    List<string> slotTypesAllowedList;  // List of slot types the item can add to himself 
    string[] slotTypesAllowedArray;  // Array of slot types the item can add to himself 
    int slotsCount;  // Number of slots the item being added/modified/deleted currently has
    int[] selectedTemplateTypeInEachSlot;  // Position in Popup for selected Template type in each slot
    //int[] selectedTemplateInEachSlot;  // Slot type for each slot
    #endregion

    #region Constructor
    // Constructor
    public ItemEditorWindow()
        {
        d=Database.Instance;
        loadInfoFromDatabase();
        createEmptyItemTemplate();
        }
    #endregion

    #region Methods
    // Methods
    #region GUI: CreateWindow(), OnGUI()
    // Unity menu
    [MenuItem("TRPG/Database/Item Editor")]
    static void CreateWindow()
        {
        ItemEditorWindow window=(ItemEditorWindow)EditorWindow.GetWindow(typeof(ItemEditorWindow));
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
        #region Item selection zone
        ///////////////////////////
        EditorGUILayout.BeginVertical("Box");
        itemsInDatabaseArray=itemsInDatabaseList.ToArray();
        EditorGUI.BeginChangeCheck();
        selectedItem=EditorGUILayout.Popup(selectedItem,itemsInDatabaseArray,GUILayout.Width(150));
        if(EditorGUI.EndChangeCheck())
            {
            if(selectedItem==0)  // if 'selectedItem' is '<NEW>' then reset the fields
                {
                loadInfoFromDatabase();
                createEmptyItemTemplate();
                }
            else  // else if 'selectedItem' exists then load it from database to manage it
                {
                loadSelectedItemTemplate();
                updateFormulasZone(selectedItem);
                updateSlotsZone(selectedItem);
                }
            }
        EditorGUILayout.EndVertical();
        #endregion
        #region Item basics zone
        ////////////////////////
        EditorGUILayout.BeginVertical("Box");
        if(selectedItem==0)  // if 'selectedItem' is '<NEW>' then allow to create a name id
            itemNameId=EditorGUILayout.TextField("Item name id",itemNameId);
        else  // else if 'selectedAttrib' exists then do not allow to create a existing name id
            EditorGUILayout.LabelField("Item name id                  "+itemNameId);
        itemDescription=EditorGUILayout.TextField("Item description",itemDescription,GUILayout.Height(50));
        EditorGUILayout.EndVertical();
        #endregion
        #region EJEMPLO DE LLAMAR GENERICO
        //EditableModifierEditor.target = itemTemplate;
        //EditableTemplateEditor.OnGUI();
        #endregion
        #region Formulas zone
        /////////////////////
        EditorGUILayout.BeginVertical("Box");
        formulaCount=EditorGUILayout.IntField("Formula count",formulaCount);
        if (formulaCount<0)
            formulaCount=0;
        else if (formulaCount>MAX_FORMULAS)
            formulaCount=MAX_FORMULAS;
        for (int i=0; i<formulaCount; i++)
            {
            EditorGUILayout.BeginHorizontal();
            selectedAttribInEachFormula[i]=EditorGUILayout.Popup(selectedAttribInEachFormula[i],attribsInDatabaseArray,GUILayout.Width(50));
            formulasArray[i]=EditorGUILayout.TextField("=",formulasArray[i]); 
            EditorGUILayout.EndHorizontal();  
            }
        EditorGUILayout.EndVertical();
        #endregion
        #region Slots zone
        //////////////////
        EditorGUILayout.BeginVertical("Box");
        slotsCount=EditorGUILayout.IntField("Slots count",slotsCount);
        if (slotsCount<0)
            slotsCount=0;
        else if (slotsCount>MAX_SLOTS)
            slotsCount=MAX_SLOTS;
        for (int i=0; i<slotsCount; i++)
            {
            EditorGUILayout.BeginHorizontal();
            selectedTemplateTypeInEachSlot[i]=EditorGUILayout.Popup(selectedTemplateTypeInEachSlot[i],slotTypesAllowedArray,GUILayout.Width(70),GUILayout.Height(40));
            //formulasArray[i]=EditorGUILayout.TextField("=",formulasArray[i]); 
            EditorGUILayout.EndHorizontal();  
            }
        EditorGUILayout.EndVertical();
        #endregion
        #region Buttons zone
        ////////////////////
        EditorGUILayout.BeginHorizontal("Box");
        if(selectedItem==0)
            {       
            if (GUILayout.Button("ADD",GUILayout.Width(80),GUILayout.Height(80)))
                {
                constructCurrentItemTemplate();
                if (d.addItemTemplate(currentItemTemplate))
                    {
                    selectedItem=0;
                    loadInfoFromDatabase();
                    createEmptyItemTemplate();
                    }
                }
            }
        else
            {
            if (GUILayout.Button("MODIFY",GUILayout.Width(80),GUILayout.Height(80)))
                {
                constructCurrentItemTemplate();
                if (d.modifyItemTemplate(currentItemTemplate))
                    {
                    selectedItem=0;
                    loadInfoFromDatabase();
                    createEmptyItemTemplate();
                    }
                }
            if (GUILayout.Button("DELETE",GUILayout.Width(80),GUILayout.Height(80)))
                {
                d.deleteItemTemplate(itemNameId);
                selectedItem=0;
                loadInfoFromDatabase();
                createEmptyItemTemplate();
                }
            }
        EditorGUILayout.EndHorizontal();
        #endregion
        }
    #endregion

    #region ItemTemplate: createEmptyItemTemplate(), loadSelectedItemTemplate(), constructCurrentItemTemplate()
    private void createEmptyItemTemplate()
        // Sets the fields to default in order to build a new ItemTemplate (<NEW> in selection Popup)
        {
        // Item related
        itemNameId="Enter your item name id here";  // Info for textField
        itemDescription="Enter your item description here";  // Info for textField
        // Formula|Item related
        List<Formula> itemFormulas=new List<Formula>();  // Empty list of formulas
        formulaCount=0;
        selectedAttribInEachFormula=new int[MAX_FORMULAS];
        formulasArray=new string[MAX_FORMULAS];
        // Slot|Item related
        List<Template> itemSlots=new List<Template>();  // Empty list of slots
        slotsCount=0;
        selectedTemplateTypeInEachSlot=new int[MAX_SLOTS];
        }

    private void loadSelectedItemTemplate()
        // Sets the fields according to the ItemTemplate chosen from database in the selection Popup
        {
        itemNameId=d.items[itemsInDatabaseArray[selectedItem]].nameId; 
        itemDescription=d.items[itemsInDatabaseArray[selectedItem]].description;
        itemFormulas=d.items[itemsInDatabaseArray[selectedItem]].formulas;
        itemSlots=d.items[itemsInDatabaseArray[selectedItem]].slots;
        }

    private void constructCurrentItemTemplate()
        // Constructs a new ItemTemplate according to whatever is shown in the fields. This could really
        // mean constructing a new ItemTemplate or just modifying one that already exists in database 
        // (in this case destroys the old ItemTemplate in database and adds the new one)
        {
        itemFormulas=new List<Formula>(); 
        for (int i=0; i<formulaCount; i++)
            {
            Formula formula=new Formula(attribsInDatabaseArray[selectedAttribInEachFormula[i]],formulasArray[i],1);
            itemFormulas.Add(formula);
            }
        itemSlots=new List<Template>();
        for (int i=0; i<slotsCount; i++)
            {
            // TO DO
            }
        currentItemTemplate=new ItemTemplate(itemNameId,itemDescription,itemFormulas,itemSlots);
        }
    #endregion

    #region Database: loadInfoFromDatabase(), auxiliary methods
    private void loadInfoFromDatabase()
        {
        loadItemsFromDatabase();
        loadAttribsFromDatabase();
        loadSlotsFromDatabase();
        }

    private void loadItemsFromDatabase()
        {
        itemsInDatabaseList=new List<string>(d.getItemNames());
        itemsInDatabaseList.Insert(0,"<NEW>");
        }

    private void loadAttribsFromDatabase()
        {
        attribsInDatabaseList=new List<string>(d.getAttribIdentifiers());
        attribsInDatabaseArray=attribsInDatabaseList.ToArray();
        formulasCountForEachItemList=getFormulasCountForEachItem();
        formulasCountForEachItemArray=formulasCountForEachItemList.ToArray();
        }

    private List<int> getFormulasCountForEachItem()
        {
        List<int> formulasCountForEachItem=new List<int>();
        foreach (KeyValuePair<string,ItemTemplate> result in d.items)
            {
            formulasCountForEachItem.Add(result.Value.formulas.Count);
            }
        return formulasCountForEachItem;
        }

    private void loadSlotsFromDatabase()
        {
        Template t=new ItemTemplate();
        slotTypesAllowedList=new List<string>(d.getAllowedSlots(t));
        slotTypesAllowedArray=slotTypesAllowedList.ToArray();
        //formulasCountForEachItemList=getFormulasCountForEachItem();
        //formulasCountForEachItemArray=formulasCountForEachItemList.ToArray();
        }
    #endregion

    #region Update GUI zones: updateFormulasZone(), updateSlotsZone(), auxiliary methods
    private void updateFormulasZone(int selectedItem)
        {
        formulaCount=formulasCountForEachItemArray[selectedItem-1]; 
        for (int i=0; i<formulaCount; i++)
            {
            selectedAttribInEachFormula[i]=bind(d.items[itemsInDatabaseList[selectedItem]].formulas[i].label);  // Attribute selected in each formula
            formulasArray[i]=d.items[itemsInDatabaseList[selectedItem]].formulas[i].formula;  // Formulas themselves
            }
        }

    private int bind(string attrStr)
        // Binds the 'attrStr' which represents an 'Attribute.id' (LVL, HPS, MPS...) with its position in the local 
        // list (used for the Editor) 'List<string> attribsInDatabaseList' returning the position itself
        {
        Dictionary<string,int> bind=new Dictionary<string,int>();
        int position=0;
        int i=0;
        bool found=false;
        while(!found && i<attribsInDatabaseList.Count)
            {
            if (attribsInDatabaseList[i]==attrStr)
                {
                position=i;
                found=true;
                }
            i++;
            }
        return position;
        }

    private void updateSlotsZone(int selectedItem)
        {
        // TO DO
        }
    #endregion
    #endregion
    }