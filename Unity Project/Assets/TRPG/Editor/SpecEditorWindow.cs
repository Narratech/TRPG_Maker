using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class SpecEditorWindow: EditorWindow
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
    // Current spec values and container for the values
    string specNameId;  // The name and unique identifier of the SpecTemplate
    string specDescription;  // The description of the SpecTemplate
    bool specIsBasicClass;  // Says if this is a basic class (not a derived spec)
    //List<string> specTags;  // Every tag that this SpecTemplate has
    List<Formula> specFormulas;  // Every formula that modifies attributes for this SpecTemplate
    SlotsConfig specAllowedSlots;  // Every slot allowed in this SpecTemplate
    SpecTemplate currentSpecTemplate;  // Temporal container for the SpecTemplate we are editing
    // Spec related
    List<string> specsInDatabaseList;  // List of spec 'Spec.nameId' in database
    string[] specsInDatabaseArray;  // The same list in array format for the Editor
    int selectedSpec;  // Position in Popup for selected spec to add/modify/delete
    // Formula|Spec related
    List<string> attribsInDatabaseList;  // List of existing 'Attribute.id' in database
    string[] attribsInDatabaseArray;  // The same list in array format for the Editor
    List<int> formulasCountForEachSpecList;  // Number of formulas each spec in database has
    int[] formulasCountForEachSpecArray;  // The same list in array format for the Editor
    int formulaCount;  // Number of formulas the spec being added/modified/deleted currently has
    int[] selectedAttribInEachFormula;  // Position in Popup for selected Attribute in each formula
    string[] formulasArray;  // Formulas themselves
    // Slot|Spec related
    List<string> slotTypesAllowedList;  // List of slot types the spec can add to himself 
    string[] slotTypesAllowedArray;  // The same list in array format for the Editor
    List<int> slotsCountForEachSpecList;  // Number of slots each spec in database has
    int[] slotsCountForEachSpecArray;  // The same list in array format for the Editor
    int slotsCount;  // Number of slots the spec being added/modified/deleted currently has
    int[] selectedTemplateTypeInEachSlot;  // Position in Popup for selected Template type in each slot
    // PassiveSlot|Slot|Spec related
    List<string> passiveWhenTagsInDatabaseList;  // List of tags for PassiveTemplate (When) in database
    string[] passiveWhenTagsInDatabaseArray;  // The same list in array format for the Editor
    int[] selectedWhenPassive;  // Position in Popup for selected 'when' option in each slot
    List<string> passiveToWhomTagsInDatabaseList;  // List of tags for PassiveTemplate (To whom) in database
    string[] passiveToWhomTagsInDatabaseArray;  // The same list in array format for the Editor
    int[] selectedToWhomPassive;  // Position in Popup for selected 'to whom' option in each slot
    // ItemSlot|Slot|Spec related
    List<string> itemTagsInDatabaseList;  // List of tags for ItemTemplate in database
    string[] itemTagsInDatabaseArray;  // The same list in array format for the Editor
    int[] itemTagMasks;  // List of tag masks for every item slot selected
    // SpecSlot|Slot|Spec related
    int[] specTagMasks;  // List of tag masks for every spec slot selected
    List<string> specsInDatabaseForSlotsList;  // List of spec 'Spec.nameId' in database
    string[] specsInDatabaseForSlotsArray;  // The same list in array format for the SpecSlot
    #endregion

    #region Constructor
    public void OnEnable()
        {
        d=Database.Instance;
        loadInfoFromDatabase();
        createEmptySpecTemplate();
        }
    #endregion

    #region Methods 
    #region Database: loadInfoFromDatabase(), auxiliary methods
    private void loadInfoFromDatabase()
        // Called when you open the Editor, when you select <NEW> on Spec Popup, or when you finish an 
        // ADD/MODIFY/DELETE operation. Then you have in local variables and structures the updated and 
        // necessary info from database to create a new SpecTemplate with the Editor
        {
        loadSpecsFromDatabase();
        loadTagsFromDatabase();
        loadAttribsFromDatabase();
        loadSlotsFromDatabase();
        }

    private void loadSpecsFromDatabase()
        {
        // For Spec Popup
        specsInDatabaseList=new List<string>(d.getSpecNames());
        specsInDatabaseList.Insert(0,"<NEW>");
        specsInDatabaseArray=specsInDatabaseList.ToArray();
        // For Spec Slots
        specsInDatabaseForSlotsList=new List<string>(d.getSpecNames());
        specsInDatabaseForSlotsArray=specsInDatabaseForSlotsList.ToArray();
        }

    private void loadTagsFromDatabase()
        {
        // Tags for Item slots zone
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
        // Getting the number of formulas every Spec has
        formulasCountForEachSpecList=getFormulasCountForEachSpec();
        formulasCountForEachSpecArray=formulasCountForEachSpecList.ToArray();
        }

    private List<int> getFormulasCountForEachSpec()
        {
        List<int> formulasCountForEachSpec=new List<int>();
        foreach (KeyValuePair<string,SpecTemplate> result in d.Specs)
            {
            formulasCountForEachSpec.Add(result.Value.Formulas.Count);
            }
        return formulasCountForEachSpec;
        }

    private void loadSlotsFromDatabase()
        {        
        // Getting the list of slot types allowed (Passive, Item) for this ItemTemplate
        Template t=new SpecTemplate();
        slotTypesAllowedList=new List<string>(d.getAllowedSlots(t));
        slotTypesAllowedList.Insert(0,"Choose...");
        slotTypesAllowedArray=slotTypesAllowedList.ToArray();
        // Getting the number of slots every SpecTemplate has
        slotsCountForEachSpecList=getSlotsCountForEachSpec();
        slotsCountForEachSpecArray=slotsCountForEachSpecList.ToArray();
        }

    private List<int> getSlotsCountForEachSpec()
        {
        List<int> slotsCountForEachSpec=new List<int>();
        foreach (KeyValuePair<string,SpecTemplate> result in d.Specs)
            {
            int passiveSlotsCount=result.Value.AllowedSlots.PassiveCfg.Count; 
            int itemSlotsCount=result.Value.AllowedSlots.ItemCfg.Count;
            int specSlotsCount=result.Value.AllowedSlots.SpecCfg.Count;
            int slotsCount=passiveSlotsCount+itemSlotsCount+specSlotsCount;
            slotsCountForEachSpec.Add(slotsCount);
            }
        return slotsCountForEachSpec;
        }
    #endregion
    
    #region SpecTemplate: createEmptySpecTemplate(), loadSelectedSpecTemplate(), constructCurrentSpecTemplate()
    private void createEmptySpecTemplate()
        // Called when you open the Editor, when you select <NEW> on Spec Popup, or when you finish an 
        // ADD/MODIFY/DELETE operation. Then fields and structures are set to be ready to be filled 
        // with data for a new SpecTemplate
        {
        // Spec related
        specNameId="Enter your specialization name id here";  // Info for textField
        specDescription="Enter your specialization description here";  // Info for textField
        // Formula|Spec related
        List<Formula> specFormulas=new List<Formula>();  // Empty list of formulas
        formulaCount=0;  // Number of formulas the SpecTemplate is managing
        selectedAttribInEachFormula=new int[MAX_FORMULAS];  // For those 'formulaCount' formulas the attribute they're modifying
        formulasArray=new string[MAX_FORMULAS];  // For those 'formulaCount' formulas, the formulas themselves
        // Slot|Spec related
        List<Template> specSlots=new List<Template>();  // Empty list of slots
        slotsCount=0;  // Number of slots the SpecTemplate is managing
        selectedTemplateTypeInEachSlot=new int[MAX_SLOTS];  // For those 'slotsCount' slots, the kind of every one of them
        // PassiveSlot|Slot|Spec Related       
        selectedWhenPassive=new int[MAX_SLOTS];  // For those 'slotsCount' slots, if the slot is PassiveTemplate, when the passive could be executed
        selectedToWhomPassive=new int[MAX_SLOTS];  // For those 'slotsCount' slots, if the slot is PassiveTemplate, to whom the passive could be executed
        // ItemSlot|Slot|Spec Related
        itemTagMasks=new int[MAX_SLOTS];  // For those 'slotsCount' slots, if the slot is ItemTemplate, the tag mask they could have
        // SpecSlot|Slot|Spec Related
        specTagMasks=new int[MAX_SLOTS];  // For those 'slotsCount' slots, if the slot is SpecTemplate, the tag mask they could have
        }

    private void loadSelectedSpecTemplate()
        // Called when you select a stored Spec on Spec Popup. Then loads every Spec related info from 
        // database into local structures so GUI can show the info for selected Spec
        {
        specNameId=d.Specs[specsInDatabaseArray[selectedSpec]].NameId; 
        specDescription=d.Specs[specsInDatabaseArray[selectedSpec]].Description;
        specIsBasicClass=d.Specs[specsInDatabaseArray[selectedSpec]].IsBasicClass;
        specFormulas=d.Specs[specsInDatabaseArray[selectedSpec]].Formulas;
        specAllowedSlots=d.Specs[specsInDatabaseArray[selectedSpec]].AllowedSlots;
        }

    private void constructCurrentSpecTemplate()
        // Constructs a new SpecTemplate according to whatever is shown in the fields. After this
        // method is finished this SpecTemplate could be added to database as a new SpecTemplate or just 
        // modify one that already exists (in this case destroys the old SpecTemplate in database 
        // and adds the new one)
        {
        specFormulas=new List<Formula>(); 
        for (int i=0; i<formulaCount; i++)
            {
            Formula formula=new Formula(attribsInDatabaseArray[selectedAttribInEachFormula[i]],formulasArray[i],1);
            specFormulas.Add(formula);
            }
        specAllowedSlots=new SlotsConfig();
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
                SpecConfig sc;
                sc.specMask=specTagMasks[i];
                List<string> auxSpecIds=new List<string>();
                for (int j=0; j<specsInDatabaseForSlotsList.Count; j++)
                    {
                    int layer = 1 << j;
                    if ((sc.specMask & layer) != 0)
                        auxSpecIds.Add(specsInDatabaseForSlotsList[j]);
                    }
                sc.specIds=auxSpecIds;
                specCfg.Add(sc);         
                }
            }
        specAllowedSlots.PassiveCfg=passiveCfg;
        specAllowedSlots.ItemCfg=itemCfg;
        specAllowedSlots.SpecCfg=specCfg;
        currentSpecTemplate=ScriptableObject.CreateInstance<SpecTemplate>().Init(specNameId,specDescription,specIsBasicClass,specFormulas,specAllowedSlots);
        }
    #endregion

    #region GUI: CreateWindow(), OnGUI()
    [MenuItem("TRPG/Classes and specializations",false,5)]
    static void CreateWindow()
        {
        var d=Database.Instance;
        SpecEditorWindow window=(SpecEditorWindow)EditorWindow.GetWindow(typeof(SpecEditorWindow));
        window.Show();
        }

    void OnGUI()
        {
        #region Spec selection zone
        ///////////////////////////
        EditorGUILayout.BeginVertical("Box");
        EditorGUI.BeginChangeCheck();
        selectedSpec=EditorGUILayout.Popup(selectedSpec,specsInDatabaseArray,GUILayout.Width(150));
        if(EditorGUI.EndChangeCheck())
            {
            if(selectedSpec==0)  // if 'selectedSpec' is '<NEW>' then reset the fields
                {
                loadInfoFromDatabase();
                createEmptySpecTemplate();
                }
            else  // else if 'selectedSpec' exists then load it from database to manage it
                {
                loadSelectedSpecTemplate();
                updateGUIZones(selectedSpec);
                }
            }
        EditorGUILayout.EndVertical();
        #endregion
        #region Spec basics zone
        ////////////////////////
        EditorGUILayout.BeginHorizontal("Button");
        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.LabelField("Basics:",EditorStyles.boldLabel);
        if(selectedSpec==0)  // if 'selectedSpec' is '<NEW>' then allow to create a name id
            specNameId=EditorGUILayout.TextField("Spec name id",specNameId);
        else  // else if 'selectedAttrib' exists then do not allow to create a existing name id
            EditorGUILayout.LabelField("Spec name id                  "+specNameId);
        specDescription=EditorGUILayout.TextField("Spec description",specDescription,GUILayout.Height(50));
        specIsBasicClass=EditorGUILayout.Toggle("Is basic class?",specIsBasicClass);
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
            else if (slotTypesAllowedArray[selectedTemplateTypeInEachSlot[i]]=="Specialization") 
                {
                specTagMasks[i]=EditorGUILayout.MaskField(specTagMasks[i],specsInDatabaseForSlotsArray,GUILayout.Width(244)); 
                }         
            EditorGUILayout.EndHorizontal();  
            }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
        #endregion
        #region Buttons zone
        ////////////////////
        EditorGUILayout.BeginHorizontal("Box");
        if(selectedSpec==0)
            {       
            if (GUILayout.Button("ADD",GUILayout.Width(80),GUILayout.Height(80)))
                {
                constructCurrentSpecTemplate();
                if (d.addModDelTemplate(currentSpecTemplate,"add"))
                    {
                    selectedSpec=0;
                    loadInfoFromDatabase();
                    createEmptySpecTemplate();
                    }
                }
            }
        else
            {
            if (GUILayout.Button("MODIFY",GUILayout.Width(80),GUILayout.Height(80)))
                {
                constructCurrentSpecTemplate();
                if (d.addModDelTemplate(currentSpecTemplate,"mod"))
                    {
                    selectedSpec=0;
                    loadInfoFromDatabase();
                    createEmptySpecTemplate();
                    }
                }
            if (GUILayout.Button("DELETE",GUILayout.Width(80),GUILayout.Height(80)))
                {
                constructCurrentSpecTemplate();
                if (d.addModDelTemplate(currentSpecTemplate,"del"))
                    {
                    selectedSpec=0;
                    loadInfoFromDatabase();
                    createEmptySpecTemplate();
                    }
                }
            }
        EditorGUILayout.EndHorizontal();
        #endregion
        }
    #endregion

    #region GUI zones: updateGUIZones(), auxiliary methods
    private void updateGUIZones(int selectedSpec)
        // Updates the fields in the Editor according to the spec from database selected in the Spec Popup.
        // This spec is identified by the 'selectedSpec' integer
        {
        updateTagsZone(selectedSpec);
        updateFormulasZone(selectedSpec);
        updateSlotsZone(selectedSpec);
        }

    private void updateTagsZone(int selectedSpec)
        // Updates the Popup tags according to the spec from database selected in the Spec Popup. This spec
        // is identified by the 'selectedSpec' integer
        {
        /*
        specTagsCount=specTags.Count; 
        for (int i=0; i<specTagsCount; i++)
            {
            selectedSpecTag[i]=bindTags(specTags[i]);  // Attribute selected in each formula
            }
        */
        }

    private int bindTags(string tagStr)
        // Binds the 'tagStr' which represents a tag Dictionary<tagStr,tagInt> with its position in the local 
        // list (used for the Editor) 'List<string> specTagsInDatabaseList' returning the position itself
        {
        /*
        Dictionary<string,int> bind=new Dictionary<string,int>();
        int position=0;
        int i=0;
        bool found=false;
        while(!found && i<specTagsInDatabaseList.Count)
            {
            if (specTagsInDatabaseList[i]==tagStr)
                {
                position=i;
                found=true;
                }
            i++;
            }
        return position;
        */
        return 0;
        }

    private void updateFormulasZone(int selectedSpec)
        // Uptates the Popup formulas and field formulas according to the spec from database selected in the 
        // Spec Popup. This spec is identified by the 'selectedSpec' integer
        {
        formulaCount=formulasCountForEachSpecArray[selectedSpec-1];
        for (int i=0; i<formulaCount; i++)
            {
            selectedAttribInEachFormula[i]=bindFormulas(d.Specs[specsInDatabaseList[selectedSpec]].Formulas[i].label);  // Attribute selected in each formula
            formulasArray[i]=d.Specs[specsInDatabaseList[selectedSpec]].Formulas[i].formula;  // Formulas themselves
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

    private void updateSlotsZone(int selectedSpec)
        // Uptates the Popup slots and config slots according to the spec from database selected in the 
        // Spec Popup. This spec is identified by the 'selectedSpec' integer
        {
        slotsCount=slotsCountForEachSpecArray[selectedSpec-1];
        int i=0;            
        foreach (Dictionary<string,string> result in d.Specs[specsInDatabaseList[selectedSpec]].AllowedSlots.PassiveCfg)
            {
            selectedTemplateTypeInEachSlot[i]=bindSlots("Passive");
            selectedWhenPassive[i]=bindPassives(result["When"],"When");
            selectedToWhomPassive[i]=bindPassives(result["To whom"],"To whom");
            i++;
            }
        foreach (ItemConfig result in d.Specs[specsInDatabaseList[selectedSpec]].AllowedSlots.ItemCfg)
            {
            selectedTemplateTypeInEachSlot[i]=bindSlots("Item");
            itemTagMasks[i]=result.itemMask;
            i++;
            }
        foreach (SpecConfig result in d.Specs[specsInDatabaseList[selectedSpec]].AllowedSlots.SpecCfg)
            {
            selectedTemplateTypeInEachSlot[i]=bindSlots("Specialization");
            specTagMasks[i]=result.specMask;
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
        // position in the local lists (used for the Editor) 'List<string> passiveWhenTagsInDatabaseList' and 'List<string> passiveToWhomTagsInDatabaseList'
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