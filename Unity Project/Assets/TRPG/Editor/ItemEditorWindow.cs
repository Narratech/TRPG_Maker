using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class ItemEditorWindow: EditorWindow
    #region Description
    #endregion
    {
    #region Attributes
    // Database instance
    Database d;
    // Constants
    private static int MAX_TAGS=30;
    private static int MAX_FORMULAS=10;
    private static int MAX_SLOTS=24;
    // Current item values and container for the values
    string itemNameId;  // The name and unique identifier of the ItemTemplate
    string itemDescription;  // The description of the ItemTemplate
    List<string> itemTags;  // Every tag that this ItemTemplate has
    List<Formula> itemFormulas;  // Every formula that modifies attributes for this ItemTemplate
    SlotsConfig itemAllowedSlots;  // Every slot allowed in this ItemTemplate
    ItemTemplate currentItemTemplate;  // Temporal container for the ItemTemplate we are editing
    // Item related
    List<string> itemsInDatabaseList;  // List of item 'Item.nameId' in database
    string[] itemsInDatabaseArray;  // The same list in array format for the Editor
    int selectedItem;  // Position in Popup for selected item to add/modify/delete 
    // Tags|Item related
    List<string> itemTagsInDatabaseList;  // List of tags for ItemTemplate in database
    string[] itemTagsInDatabaseArray;  // The same list in array format for the Editor
    int[] selectedItemTag;  // Position in Popup for selected type tag in each tag
    int itemTagsCount;  // Number of tags the item being added/modified/deleted currently has
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
    string[] slotTypesAllowedArray;  // The same list in array format for the Editor
    List<int> slotsCountForEachItemList;  // Number of slots each item in database has
    int[] slotsCountForEachItemArray;  // The same list in array format for the Editor
    int slotsCount;  // Number of slots the item being added/modified/deleted currently has
    int[] selectedTemplateTypeInEachSlot;  // Position in Popup for selected Template type in each slot
    // PassiveSlot|Slot|Item related
    List<string> passiveWhenTagsInDatabaseList;  // List of tags for PassiveTemplate (When) in database
    string[] passiveWhenTagsInDatabaseArray;  // The same list in array format for the Editor
    int[] selectedWhenPassive;  // Position in Popup for selected 'when' option in each slot
    List<string> passiveToWhomTagsInDatabaseList;  // List of tags for PassiveTemplate (To whom) in database
    string[] passiveToWhomTagsInDatabaseArray;  // The same list in array format for the Editor
    int[] selectedToWhomPassive;  // Position in Popup for selected 'to whom' option in each slot
    // ItemSlot|Slot|Item related
    int[] itemTagMasks;  // List of tag masks for every item slot selected
    // SpecSlot|Slot|Item related
    // No Spec slots in ItemTemplate
    #endregion

    #region Constructor
    public void OnEnable()
        {
        d=Database.Instance;
        loadInfoFromDatabase();
        createEmptyItemTemplate();
        }
    #endregion

    #region Methods
    #region Database: loadInfoFromDatabase(), auxiliary methods
    private void loadInfoFromDatabase()
        // Called when you open the Editor, when you select <NEW> on Item Popup, or when you finish an 
        // ADD/MODIFY/DELETE operation. Then you have in local variables and structures the updated and 
        // necessary info from database to create a new ItemTemplate with the Editor
        {
        loadItemsFromDatabase();
        loadTagsFromDatabase();
        loadAttribsFromDatabase();
        loadSlotsFromDatabase();
        }

    private void loadItemsFromDatabase()
        {
        itemsInDatabaseList=new List<string>(d.getItemNames());
        itemsInDatabaseList.Insert(0,"<NEW>");
        itemsInDatabaseArray=itemsInDatabaseList.ToArray();
        }

    private void loadTagsFromDatabase()
        {
        // Tags for Item tags zone and Item slots zone
        List<string> auxItemTags=d.Tags["Item"];
        itemTagsInDatabaseList=new List<string>();
        foreach (var result in auxItemTags)
            {
            itemTagsInDatabaseList.Add(result);
            }
        itemTagsInDatabaseArray=itemTagsInDatabaseList.ToArray();
        // Tags for Passive slots zone
        List<string> auxPassiveWhenTags=d.Tags["PassiveWhen"];
        passiveWhenTagsInDatabaseList=new List<string>();
        foreach (var result in auxPassiveWhenTags)
            {
            passiveWhenTagsInDatabaseList.Add(result);
            }
        passiveWhenTagsInDatabaseArray=passiveWhenTagsInDatabaseList.ToArray();
        List<string> auxPassiveToWhomTags=d.Tags["PassiveToWhom"];
        passiveToWhomTagsInDatabaseList=new List<string>();
        foreach (var result in auxPassiveToWhomTags)
            {
            passiveToWhomTagsInDatabaseList.Add(result);
            }
        passiveToWhomTagsInDatabaseArray=passiveToWhomTagsInDatabaseList.ToArray();
        }

    private void loadAttribsFromDatabase()
        {
        // Getting the lists of attributes in database
        attribsInDatabaseList=new List<string>(d.getAttribIdentifiers());
        attribsInDatabaseArray=attribsInDatabaseList.ToArray();
        // Getting the number of formulas every Item has
        formulasCountForEachItemList=getFormulasCountForEachItem();
        formulasCountForEachItemArray=formulasCountForEachItemList.ToArray();
        }

    private List<int> getFormulasCountForEachItem()
        {
        List<int> formulasCountForEachItem=new List<int>();
        foreach (KeyValuePair<string,ItemTemplate> result in d.Items)
            {
            formulasCountForEachItem.Add(result.Value.Formulas.Count);
            }
        return formulasCountForEachItem;
        }

    private void loadSlotsFromDatabase()
        {
        // Getting the list of slot types allowed (Passive, Item) for this ItemTemplate
        Template t=new ItemTemplate();
        slotTypesAllowedList=new List<string>(d.getAllowedSlots(t));
        slotTypesAllowedList.Insert(0,"Choose...");
        slotTypesAllowedArray=slotTypesAllowedList.ToArray();
        // Getting the number of slots every ItemTemplate has
        slotsCountForEachItemList=getSlotsCountForEachItem();
        slotsCountForEachItemArray=slotsCountForEachItemList.ToArray();
        }

    private List<int> getSlotsCountForEachItem()
        {
        List<int> slotsCountForEachItem=new List<int>();
        foreach (KeyValuePair<string,ItemTemplate> result in d.Items)
            {
            int passiveSlotsCount=result.Value.AllowedSlots.PassiveCfg.Count; 
            int itemSlotsCount=result.Value.AllowedSlots.ItemCfg.Count;
            int slotsCount=passiveSlotsCount+itemSlotsCount;
            slotsCountForEachItem.Add(slotsCount);
            }
        return slotsCountForEachItem;
        }
    #endregion
    
    #region ItemTemplate: createEmptyItemTemplate(), loadSelectedItemTemplate(), constructCurrentItemTemplate()
    private void createEmptyItemTemplate()
        // Called when you open the Editor, when you select <NEW> on Item Popup, or when you finish an 
        // ADD/MODIFY/DELETE operation. Then fields and structures are set to be ready to be filled 
        // with data for a new ItemTemplate
        {
        // Item related
        itemNameId="Enter your item name id here";  // Info for textField
        itemDescription="Enter your item description here";  // Info for textField
        // Tags|Item related
        itemTagsCount=0;  // Number of tags the ItemTemplate is managing
        selectedItemTag=new int[MAX_TAGS];  // For those 'itemTagsCount' tags the ones selected
        // Formula|Item related
        List<Formula> itemFormulas=new List<Formula>();  // Empty list of formulas
        formulaCount=0;  // Number of formulas the ItemTemplate is managing
        selectedAttribInEachFormula=new int[MAX_FORMULAS];  // For those 'formulaCount' formulas the attribute they're modifying
        formulasArray=new string[MAX_FORMULAS];  // For those 'formulaCount' formulas, the formulas themselves
        // Slot|Item related
        List<Template> itemSlots=new List<Template>();  // Empty list of slots
        slotsCount=0;  // Number of slots the ItemTemplate is managing
        selectedTemplateTypeInEachSlot=new int[MAX_SLOTS];  // For those 'slotsCount' slots, the kind of every one of them
        // PassiveSlot|Slot|Item Related       
        selectedWhenPassive=new int[MAX_SLOTS];  // For those 'slotsCount' slots, if the slot is PassiveTemplate, when the passive could be executed
        selectedToWhomPassive=new int[MAX_SLOTS];  // For those 'slotsCount' slots, if the slot is PassiveTemplate, to whom the passive could be executed
        // ItemSlot|Slot|Item Related
        itemTagMasks=new int[MAX_SLOTS];  // For those 'slotsCount' slots, if the slot is ItemTemplate, the tag mask they could have
        // SpecSlot|Slot|Item Related
        // No Spec slots in ItemTemplate
        }

    private void loadSelectedItemTemplate()
        // Called when you select a stored Item on Item Popup. Then loads every Item related info from 
        // database into local structures so GUI can show the info for selected Item
        {
        itemNameId=d.Items[itemsInDatabaseArray[selectedItem]].NameId; 
        itemDescription=d.Items[itemsInDatabaseArray[selectedItem]].Description;
        itemTags=d.Items[itemsInDatabaseArray[selectedItem]].Tags;
        itemFormulas=d.Items[itemsInDatabaseArray[selectedItem]].Formulas;
        itemAllowedSlots=d.Items[itemsInDatabaseArray[selectedItem]].AllowedSlots;
        }

    private void constructCurrentItemTemplate()
        // Constructs a new ItemTemplate according to whatever is shown in the fields. After this
        // method is finished this ItemTemplate could be added to database as a new ItemTemplate or just 
        // modify one that already exists (in this case destroys the old ItemTemplate in database 
        // and adds the new one)
        {
        itemTags=new List<string>();
        for (int i=0; i<itemTagsCount; i++)
            {
            string tag=itemTagsInDatabaseArray[selectedItemTag[i]];
            itemTags.Add(tag);
            }
        itemFormulas=new List<Formula>(); 
        for (int i=0; i<formulaCount; i++)
            {
            Formula formula=new Formula(attribsInDatabaseArray[selectedAttribInEachFormula[i]],formulasArray[i],1);
            itemFormulas.Add(formula);
            }
        itemAllowedSlots=new SlotsConfig();
        List<Dictionary<string,string>> passiveCfg=new List<Dictionary<string,string>>();
        List<ItemConfig> itemCfg=new List<ItemConfig>();
        List<SpecConfig> specCfg=new List<SpecConfig>();
        for (int i=0; i<slotsCount; i++)
            {
            if (slotTypesAllowedArray[selectedTemplateTypeInEachSlot[i]]=="Passive") 
                {
                Dictionary<string,string> actual=new Dictionary<string,string>();
                actual.Add("When",passiveWhenTagsInDatabaseArray[selectedWhenPassive[i]]);
                actual.Add("To whom",passiveToWhomTagsInDatabaseArray[selectedToWhomPassive[i]]);
                passiveCfg.Add(actual);
                }
            else if (slotTypesAllowedArray[selectedTemplateTypeInEachSlot[i]]=="Item")
                {
                ItemConfig ic;
                ic.itemMask=itemTagMasks[i];
                List<string> auxItemIds=new List<string>();
                for (int j=0; j<itemTagsInDatabaseList.Count; j++)
                    {
                    int layer = 1 << j;
                    if ((ic.itemMask & layer) != 0)
                        auxItemIds.Add(itemTagsInDatabaseList[j]);
                    }
                ic.itemIds=auxItemIds;
                itemCfg.Add(ic); 
                }
            else if (slotTypesAllowedArray[selectedTemplateTypeInEachSlot[i]]=="Specialization")
                {
                }
            }
        itemAllowedSlots.PassiveCfg=passiveCfg;
        itemAllowedSlots.ItemCfg=itemCfg;
        itemAllowedSlots.SpecCfg=specCfg;
        currentItemTemplate=ScriptableObject.CreateInstance<ItemTemplate>().Init(itemNameId,itemDescription,itemTags,itemFormulas,itemAllowedSlots);
        }
    #endregion

    #region GUI: CreateWindow(), OnGUI()
    [MenuItem("TRPG/Items",false,4)]
    static void CreateWindow()
        {
        var d=Database.Instance;
        ItemEditorWindow window=(ItemEditorWindow)EditorWindow.GetWindow(typeof(ItemEditorWindow));
        window.Show();
        }

    void OnGUI()
        {
        #region Item selection zone
        ///////////////////////////
        EditorGUILayout.BeginVertical("Box");
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
                updateGUIZones(selectedItem);
                }
            }
        EditorGUILayout.EndVertical();
        #endregion
        #region Item basics zone
        ////////////////////////
        EditorGUILayout.BeginHorizontal("Button");
        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.LabelField("Basics:",EditorStyles.boldLabel);
        if(selectedItem==0)  // if 'selectedItem' is '<NEW>' then allow to create a name id
            itemNameId=EditorGUILayout.TextField("Item name id",itemNameId);
        else  // else if 'selectedAttrib' exists then do not allow to create a existing name id
            EditorGUILayout.LabelField("Item name id                  "+itemNameId);
        itemDescription=EditorGUILayout.TextField("Item description",itemDescription,GUILayout.Height(50));
        EditorGUILayout.EndVertical();
        #endregion
        #region Tags zone
        /////////////////
        EditorGUILayout.BeginVertical("Box");        
        EditorGUILayout.LabelField("Tags:",EditorStyles.boldLabel);
        itemTagsCount=EditorGUILayout.IntField("Count",itemTagsCount);
        if (itemTagsCount<0)
            itemTagsCount=0;
        else if (itemTagsCount>MAX_TAGS)
            itemTagsCount=MAX_TAGS;
        EditorGUILayout.BeginHorizontal();
        for (int i=0; i<itemTagsCount; i++)
            {
            selectedItemTag[i]=EditorGUILayout.Popup(selectedItemTag[i],itemTagsInDatabaseArray,GUILayout.Width(70));
            if ((i%5==4))
                {
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();  
                }
            }
        EditorGUILayout.EndHorizontal();  
        EditorGUILayout.EndVertical();  
        #endregion
        #region Formulas zone
        /////////////////////
        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.LabelField("Formulas:",EditorStyles.boldLabel);
        formulaCount=EditorGUILayout.IntField("Count",formulaCount);
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
        EditorGUILayout.EndVertical();
        #endregion
        #region Slots zone
        //////////////////
        EditorGUILayout.BeginVertical("Box",GUILayout.Width(310));
        EditorGUILayout.LabelField("Slots:",EditorStyles.boldLabel);
        slotsCount=EditorGUILayout.IntField("Count",slotsCount);
        if (slotsCount<0)
            slotsCount=0;
        else if (slotsCount>MAX_SLOTS)
            slotsCount=MAX_SLOTS;
        for (int i=0; i<slotsCount; i++)
            {
            EditorGUILayout.BeginHorizontal();
            selectedTemplateTypeInEachSlot[i]=EditorGUILayout.Popup(selectedTemplateTypeInEachSlot[i],slotTypesAllowedArray,GUILayout.Width(70));
            if (slotTypesAllowedArray[selectedTemplateTypeInEachSlot[i]]=="Item")
                {
                itemTagMasks[i]=EditorGUILayout.MaskField(itemTagMasks[i],itemTagsInDatabaseArray,GUILayout.Width(244)); 
                }
            else if (slotTypesAllowedArray[selectedTemplateTypeInEachSlot[i]]=="Passive") 
                {
                selectedWhenPassive[i]=EditorGUILayout.Popup(selectedWhenPassive[i],passiveWhenTagsInDatabaseArray,GUILayout.Width(120));
                selectedToWhomPassive[i]=EditorGUILayout.Popup(selectedToWhomPassive[i],passiveToWhomTagsInDatabaseArray,GUILayout.Width(120));
                }          
            EditorGUILayout.EndHorizontal();  
            }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
        #endregion
        #region Buttons zone
        ////////////////////
        EditorGUILayout.BeginHorizontal("Box");
        if(selectedItem==0)
            {       
            if (GUILayout.Button("ADD",GUILayout.Width(80),GUILayout.Height(80)))
                {
                constructCurrentItemTemplate();
                if (d.addModDelTemplate(currentItemTemplate,"add"))
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
                if (d.addModDelTemplate(currentItemTemplate,"mod"))
                    {
                    selectedItem=0;
                    loadInfoFromDatabase();
                    createEmptyItemTemplate();
                    }
                }
            if (GUILayout.Button("DELETE",GUILayout.Width(80),GUILayout.Height(80)))
                {
                constructCurrentItemTemplate();
                if (d.addModDelTemplate(currentItemTemplate,"del"))
                    {
                    selectedItem=0;
                    loadInfoFromDatabase();
                    createEmptyItemTemplate();
                    }
                }
            }
        EditorGUILayout.EndHorizontal();
        #endregion
        }
    #endregion

    #region GUI zones: updateGUIZones(), auxiliary methods
    private void updateGUIZones(int selectedItem)
        // Updates the fields in the Editor according to the item from database selected in the Item Popup.
        // This item is identified by the 'selectedItem' integer
        {
        updateTagsZone(selectedItem);
        updateFormulasZone(selectedItem);
        updateSlotsZone(selectedItem);
        }

    private void updateTagsZone(int selectedItem)
        // Updates the Popup tags according to the item from database selected in the Item Popup. This item
        // is identified by the 'selectedItem' integer
        {
        itemTagsCount=itemTags.Count; 
        for (int i=0; i<itemTagsCount; i++)
            {
            selectedItemTag[i]=bindTags(itemTags[i]);  // Attribute selected in each formula
            }
        }

    private int bindTags(string tagStr)
        // Binds the 'tagStr' which represents a tag Dictionary<tagStr,tagInt> with its position in the local 
        // list (used for the Editor) 'List<string> passiveTagsInDatabaseList' returning the position itself
        {
        Dictionary<string,int> bind=new Dictionary<string,int>();
        int position=0;
        int i=0;
        bool found=false;
        while(!found && i<itemTagsInDatabaseList.Count)
            {
            if (itemTagsInDatabaseList[i]==tagStr)
                {
                position=i;
                found=true;
                }
            i++;
            }
        return position;
        }

    private void updateFormulasZone(int selectedItem)
        // Uptates the Popup formulas and field formulas according to the item from database selected in the 
        // Item Popup. This item is identified by the 'selectedItem' integer
        {
        formulaCount=formulasCountForEachItemArray[selectedItem-1]; 
        for (int i=0; i<formulaCount; i++)
            {
            selectedAttribInEachFormula[i]=bindFormulas(d.Items[itemsInDatabaseList[selectedItem]].Formulas[i].label);  // Attribute selected in each formula
            formulasArray[i]=d.Items[itemsInDatabaseList[selectedItem]].Formulas[i].formula;  // Formulas themselves
            }
        }

    private int bindFormulas(string attrStr)
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
        // Uptates the Popup slots and config slots according to the item from database selected in the 
        // Item Popup. This item is identified by the 'selectedItem' integer
        {
        slotsCount=slotsCountForEachItemArray[selectedItem-1];
        int i=0;            
        foreach (Dictionary<string,string> result in d.Items[itemsInDatabaseList[selectedItem]].AllowedSlots.PassiveCfg)
            {
            selectedTemplateTypeInEachSlot[i]=bindSlots("Passive");
            selectedWhenPassive[i]=bindPassives(result["When"],"When");
            selectedToWhomPassive[i]=bindPassives(result["To whom"],"To whom");
            i++;
            }
        foreach (ItemConfig result in d.Items[itemsInDatabaseList[selectedItem]].AllowedSlots.ItemCfg)
            {
            selectedTemplateTypeInEachSlot[i]=bindSlots("Item");
            itemTagMasks[i]=result.itemMask;
            i++;
            }
        foreach (SpecConfig result in d.Items[itemsInDatabaseList[selectedItem]].AllowedSlots.SpecCfg)
            {
            selectedTemplateTypeInEachSlot[i]=bindSlots("Specialization");
            i++;
            }
        }

    private int bindSlots(string slotTypeStr)
        // Binds the 'slotTypeStr' which represents a type of slot with its position in the local list (used for the Editor) 
        // 'List<string> slotTypesAllowedList'
        {
        Dictionary<string,int> bind=new Dictionary<string,int>();
        int position=0;
        int i=0;
        bool found=false;
        while(!found && i<slotTypesAllowedList.Count)
            {
            if (slotTypesAllowedList[i]==slotTypeStr)
                {
                position=i;
                found=true;
                }
            i++;
            }
        return position;
        }

    private int bindPassives(string passiveStr, string option)
        // Binds the 'passiveStr' which represents a concrete passive option for "When" or "To whom" with its 
        // position in the local lists (used for the Editor) 'List<string> whenPassiveOptionsList' and 'List<string> toWhomPassiveOptionsList'
        {
        Dictionary<string,int> bind=new Dictionary<string,int>();
        List<string> optionsList=new List<string>();
        int position=0;
        int i=0;
        bool found=false;
        if (option=="When")
            optionsList=new List<string>(passiveWhenTagsInDatabaseList);
        else if (option=="To whom")
            optionsList=new List<string>(passiveToWhomTagsInDatabaseList);
        while(!found && i<optionsList.Count)
            {
            if (optionsList[i]==passiveStr)
                {
                position=i;
                found=true;
                }
            i++;
            }
        return position;
        }
    #endregion
    #endregion
    }
