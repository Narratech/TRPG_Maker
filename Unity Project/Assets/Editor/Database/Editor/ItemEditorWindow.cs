using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class ItemEditorWindow: EditorWindow
    {
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
    ItemTemplate temporal;  // Temporal container for the ItemTemplate
    // Attribute related
    List<string> attribsInDatabaseList;  // List of 'Attribute.id' loaded from database
    string[] attribsInDatabaseArray;  // The same list in array format for the Editor
    // Item related
    List<string> itemsInDatabaseList;  // List of item 'Item.nameId' loaded from database
    string[] itemsInDatabaseArray;  // The same list in array format for the Editor
    int selectedItem;  // Selected item to edit/delete
    // Formula|Item related
    List<int> formulasCountForEachItemList;  // Number of formulas each item in database has
    int[] formulasCountForEachItemArray;  // The same list in array format for the Editor
    int formulaCount;  // Number of formulas the item (as container) has
    int[] selectedAttribInEachFormulaInt;  // Attribute selected in each formula in int format
    string[] selectedAttribInEachFormulaStr;  // Attribute selected in each formula in string format
    string[] formulasArray;  // Formulas themselves
    // Slot|Item related
    string[] slotTypesArray;  // Array of slot types (item, passive, spec) the item can add to himself 
    int slotsCount;  // Number of slots the item (as container) has
    int dbSlotsCount;  // Number of slots the item (in database) has
    int[] selectedSlotTypeInEachSlot;  // Slot type for each slot

    // Constructor
    public ItemEditorWindow()
        {
        d=Database.Instance;
        resetFields();
        }

    // Methods
    [MenuItem("Window/Item Editor")]
    static void CreateWindow()
        {
        ItemEditorWindow window=(ItemEditorWindow)EditorWindow.GetWindow(typeof(ItemEditorWindow));
        window.Show();
        }

    void OnGUI()
        {
        // Events
        // Left mouse click on ADD/DELETE button
        if(Event.current.isMouse && Event.current.type == EventType.mouseDown && Event.current.button == 0)
            loadItemsFromDatabase();
        // Zones
        // Selection zone
        EditorGUILayout.BeginVertical("Box");
        itemsInDatabaseArray=itemsInDatabaseList.ToArray();
        EditorGUI.BeginChangeCheck();
        selectedItem=EditorGUILayout.Popup(selectedItem,itemsInDatabaseArray,GUILayout.Width(150));
        if(EditorGUI.EndChangeCheck())
            {
            if(selectedItem==0)  // if 'selectedItem' is '<NEW>' then reset the fields
                {
                resetFields();
                }
            else  // else if 'selectedItem' exists then manage it directly from database through 'temporal' container
                {
                temporal=d.items[itemsInDatabaseArray[selectedItem]];
                updateFormulasZone(selectedItem);
                updateSlotsZone(selectedItem);
                }
            }
        EditorGUILayout.EndVertical();
        // Basics zone
        EditorGUILayout.BeginVertical("Box");
        if(selectedItem==0)  // if 'selectedItem' is '<NEW>' then allow to create a name id
            //temporal.nameId=EditorGUILayout.TextField("Item name id",temporal.nameId);  // CON ESTO NO FUNCIONA
            itemNameId=EditorGUILayout.TextField("Item name id",itemNameId);
        else  // else if 'selectedAttrib' exists then do not allow to create a existing name id
            EditorGUILayout.LabelField("Item name id                    " + temporal.nameId);
        itemDescription=EditorGUILayout.TextField("Item description",itemDescription);
        EditorGUILayout.EndVertical();
        // Formulas zone
        EditorGUILayout.BeginVertical("Box");
        formulaCount=EditorGUILayout.IntField("Formula count",formulaCount);
        if (formulaCount<0)
            formulaCount=0;
        else if (formulaCount>MAX_FORMULAS)
            formulaCount=MAX_FORMULAS;
        for (int i=0; i<formulaCount; i++)
            {
            EditorGUILayout.BeginHorizontal();
            selectedAttribInEachFormulaInt[i]=EditorGUILayout.Popup(selectedAttribInEachFormulaInt[i],attribsInDatabaseArray,GUILayout.Width(50));
            formulasArray[i]=EditorGUILayout.TextField("=",formulasArray[i]); 
            EditorGUILayout.EndHorizontal();  
            }
        EditorGUILayout.EndVertical();
        // Slots zone
        EditorGUILayout.BeginVertical("Box");
        slotsCount=EditorGUILayout.IntField("Slots count",slotsCount);
        if (slotsCount<0)
            slotsCount=0;
        else if (slotsCount>MAX_SLOTS)
            slotsCount=MAX_SLOTS;
        EditorGUILayout.EndVertical();
        for (int i=0; i<slotsCount; i++)
            {
            EditorGUILayout.BeginHorizontal();
            //selectedAttribInEachFormula[i]=EditorGUILayout.Popup(selectedAttribInEachFormula[i],attribsInDatabaseArray,GUILayout.Width(50));
            //formulasArray[i]=EditorGUILayout.TextField("=",formulasArray[i]);
            EditorGUILayout.EndHorizontal();  
            }
        // Buttons zone
        EditorGUILayout.BeginHorizontal("Box");
        if(selectedItem==0)  // if 'selectedItem' is '<NEW>' then the button adds
            {            
            if (GUILayout.Button("ADD",GUILayout.Width(80),GUILayout.Height(80)))
                {
                createTemporal();
                if (d.addItem(temporal))
                    {
                    resetFields();
                    Debug.Log(">> Item added to database!");
                    }
                else
                    Debug.Log("X Error adding item to database!");
                }
            }
        else
            {
            if (GUILayout.Button("MODIFY",GUILayout.Width(80),GUILayout.Height(80)))
                {/*
                if (Database.Instance.deleteItem(itemsInDatabaseArray[selectedItem]))
                    {
                    resetFields();
                    Debug.Log("<< Item deleted from database!");
                    }
                else
                    Debug.Log("X Error deleting item from database!");
                */}
            if (GUILayout.Button("DELETE",GUILayout.Width(80),GUILayout.Height(80)))
                {

                }
            }
        EditorGUILayout.EndHorizontal();
        }

    private void resetFields()
        {       
        // Current item values and container
        itemNameId="Enter your item name id here";
        itemDescription="Enter your item description here";
        List<Formula> itemFormulas=new List<Formula>();  // Every formula that modifies attributes for this Template
        /*
        Formula f=new Formula("zas","formula de zas",1);
        Debug.Log(f.label);
        Debug.Log(f.formula);
        Debug.Log(f.priority);
        */
        List<Template> itemSlots=new List<Template>();  // Templates which this Template has
        ItemTemplate temporal;
        temporal=new ItemTemplate(itemNameId,itemDescription,itemFormulas,itemSlots);
        // Items from database
        loadItemsFromDatabase();
        selectedItem=0;
        // Formulas from current item
        loadAttribsFromDatabase();
        formulaCount=0;
        selectedAttribInEachFormulaInt=new int[MAX_FORMULAS];
        formulasArray=new string[MAX_FORMULAS];
        // Slots from current item         
        slotsCount=0;  
        slotTypesArray=new string[] {"Item","Passive","Specialization"};
        selectedAttribInEachFormulaInt=new int[MAX_SLOTS];
        }

    private void loadAttribsFromDatabase()
        {
        attribsInDatabaseList=new List<string>(d.getAttribIdentifiers());
        attribsInDatabaseArray=attribsInDatabaseList.ToArray();
        formulasCountForEachItemList=getFormulasCountForEachItem();
        //formulasForEachItemList=new List<string>(Database.Instance.getFormulasForEachItem());
        formulasCountForEachItemArray=formulasCountForEachItemList.ToArray();
        }

    private void loadItemsFromDatabase()
        {
        itemsInDatabaseList=new List<string>(d.getItemNames());
        itemsInDatabaseList.Insert(0,"<NEW>");
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
    
    private void updateFormulasZone(int selectedItem)
        {
        formulaCount=formulasCountForEachItemArray[selectedItem-1];
        for (int i=0; i<formulaCount; i++)
            {
            selectedAttribInEachFormulaInt[i]=bindInLoad(d.items[itemsInDatabaseList[selectedItem]].formulas[i].label);  // Attribute selected in each formula
            formulasArray[i]=d.items[itemsInDatabaseList[selectedItem]].formulas[i].formula;  // Formulas themselves
            }
        }
 
    private int bindInLoad(string attrStr)
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

    private void createTemporal()
        {
        for (int i=0; i<formulaCount; i++)
            {
            Formula formula=new Formula(attribsInDatabaseArray[selectedAttribInEachFormulaInt[i]],formulasArray[i],1);
            itemFormulas=new List<Formula>(); 
            itemFormulas.Add(formula);

            Debug.Log(formula.label);
            Debug.Log(formula.formula);
            Debug.Log(formula.priority);

            }
        for (int i=0; i<slotsCount; i++)
            {
            }
        temporal=new ItemTemplate(itemNameId,itemDescription,itemFormulas,itemSlots);
        } 

    private void updateSlotsZone(int selectedItem)
        {
        }             
    }
