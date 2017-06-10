using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class PassiveEditorWindow: EditorWindow
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
    // Current passive values and container for the values
    string passiveNameId;  // The name and unique identifier of the SpecTemplate
    string passiveDescription;  // The description of the SpecTemplate
    List<string> passiveTags;  // Every tag that this SpecTemplate has
    List<Formula> passiveFormulas;  // Every formula that modifies attributes for this SpecTemplate
    SlotsConfig passiveAllowedSlots;  // Every slot allowed in this SpecTemplate
    Template currentPassiveTemplate;  // Temporal container for the SpecTemplate we are editing
    // Passive related
    List<string> passivesInDatabaseList;  // List of spec 'Spec.nameId' in database
    string[] passivesInDatabaseArray;  // The same list in array format for the Editor
    int selectedPassive;  // Position in Popup for selected spec to add/modify/delete 
    // Tags|Passive related
    List<string> passiveTagsInDatabaseList;  // List of tags for SpecTemplate in database
    string[] passiveTagsInDatabaseArray;  // The same list in array format for the Editor
    int[] selectedPassiveTag;  // Position in Popup for selected type tag in each tag
    int passiveTagsCount;  // Number of tags the item being added/modified/deleted currently has
    // Formula|Passive related
    List<string> attribsInDatabaseList;  // List of existing 'Attribute.id' in database
    string[] attribsInDatabaseArray;  // The same list in array format for the Editor
    List<int> formulasCountForEachPassiveList;  // Number of formulas each spec in database has
    int[] formulasCountForEachPassiveArray;  // The same list in array format for the Editor
    int formulaCount;  // Number of formulas the spec being added/modified/deleted currently has
    int[] selectedAttribInEachFormula;  // Position in Popup for selected Attribute in each formula
    string[] formulasArray;  // Formulas themselves
    // Slot|Passive related
    List<string> slotTypesAllowedList;  // List of slot types the spec can add to himself 
    string[] slotTypesAllowedArray;  // The same list in array format for the Editor
    List<int> slotsCountForEachPassiveList;  // Number of slots each spec in database has
    int[] slotsCountForEachPassiveArray;  // The same list in array format for the Editor
    int slotsCount;  // Number of slots the spec being added/modified/deleted currently has
    int[] selectedTemplateTypeInEachSlot;  // Position in Popup for selected Template type in each slot
    // PassiveSlot|Slot|Passive related
    List<string> passiveWhenTagsInDatabaseList;  // List of tags for PassiveTemplate (When) in database
    string[] passiveWhenTagsInDatabaseArray;  // The same list in array format for the Editor
    int[] selectedWhenPassive;  // Position in Popup for selected 'when' option in each slot
    List<string> passiveToWhomTagsInDatabaseList;  // List of tags for PassiveTemplate (To whom) in database
    string[] passiveToWhomTagsInDatabaseArray;  // The same list in array format for the Editor
    int[] selectedToWhomPassive;  // Position in Popup for selected 'to whom' option in each slot
    // ItemSlot|Slot|Spec related
    // No Item slots in PassiveTemplate
    // SpecSlot|Slot|Spec related
    // No Spec slots in PassiveTemplate
    #endregion

    #region Constructor
    public void OnEnable()
        {
        d=Database.Instance;
        loadInfoFromDatabase();
        createEmptyPassiveTemplate();
        }
    #endregion

    #region Methods 
    #region Database: loadInfoFromDatabase(), auxiliary methods
    private void loadInfoFromDatabase()
        // Called when you open the Editor, when you select <NEW> on Spec Popup, or when you finish an 
        // ADD/MODIFY/DELETE operation. Then you have in local variables and structures the updated and 
        // necessary info from database to create a new SpecTemplate with the Editor
        {
        loadPassivesFromDatabase();
        loadTagsFromDatabase();
        loadAttribsFromDatabase();
        loadSlotsFromDatabase();
        }

    private void loadPassivesFromDatabase()
        {
        // For Passive Popup
        passivesInDatabaseList=new List<string>(d.getPassiveNames());
        passivesInDatabaseList.Insert(0,"<NEW>");
        passivesInDatabaseArray=passivesInDatabaseList.ToArray();
        }

    private void loadTagsFromDatabase()
        {
        // Tags for Passive tags zone and Passive slots zone
        passiveTagsInDatabaseList=new List<string>();
        passiveWhenTagsInDatabaseList=new List<string>();
        List<string> auxPassiveWhenTags=d.Tags["PassiveWhen"];
        foreach (var result in auxPassiveWhenTags)
            {
            passiveWhenTagsInDatabaseList.Add(result);
            passiveTagsInDatabaseList.Add(result);
            }
        passiveWhenTagsInDatabaseArray=passiveWhenTagsInDatabaseList.ToArray();
        passiveToWhomTagsInDatabaseList=new List<string>();
        List<string> auxPassiveToWhomTags=d.Tags["PassiveToWhom"];
        foreach (var result in auxPassiveToWhomTags)
            {
            passiveToWhomTagsInDatabaseList.Add(result);
            passiveTagsInDatabaseList.Add(result);
            }
        passiveToWhomTagsInDatabaseArray=passiveToWhomTagsInDatabaseList.ToArray();
        passiveTagsInDatabaseArray=passiveTagsInDatabaseList.ToArray();
        }

    private void loadAttribsFromDatabase()
        {
        // Getting the lists of attributes in database
        attribsInDatabaseList=new List<string>(d.getAttribIdentifiers());
        attribsInDatabaseArray=attribsInDatabaseList.ToArray();
        // Getting the number of formulas every Passive has
        formulasCountForEachPassiveList=getFormulasCountForEachPassive();
        formulasCountForEachPassiveArray=formulasCountForEachPassiveList.ToArray();
        }

    private List<int> getFormulasCountForEachPassive()
        {
        List<int> formulasCountForEachPassive=new List<int>();
        foreach (KeyValuePair<string,PassiveTemplate> result in d.Passives)
            {
            formulasCountForEachPassive.Add(result.Value.Formulas.Count);
            }
        return formulasCountForEachPassive;
        }

    private void loadSlotsFromDatabase()
        {        
        // Getting the list of slot types allowed (Passive) for this PassiveTemplate
        Template t=new PassiveTemplate();
        slotTypesAllowedList=new List<string>(d.getAllowedSlots(t));
        slotTypesAllowedList.Insert(0,"Choose...");
        slotTypesAllowedArray=slotTypesAllowedList.ToArray();
        // Getting the number of slots every PassiveTemplate has
        slotsCountForEachPassiveList=getSlotsCountForEachPassive();
        slotsCountForEachPassiveArray=slotsCountForEachPassiveList.ToArray();
        }

    private List<int> getSlotsCountForEachPassive()
        {
        List<int> slotsCountForEachPassive=new List<int>();
        foreach (KeyValuePair<string,PassiveTemplate> result in d.Passives)
            {
            int passiveSlotsCount=result.Value.AllowedSlots.PassiveCfg.Count;
            int slotsCount=passiveSlotsCount;
            slotsCountForEachPassive.Add(slotsCount);
            }
        return slotsCountForEachPassive;
        }
    #endregion
    
    #region PassiveTemplate: createEmptyPassiveTemplate(), loadselectedPassiveTemplate(), constructcurrentPassiveTemplate()
    private void createEmptyPassiveTemplate()
        // Called when you open the Editor, when you select <NEW> on Passive Popup, or when you finish an 
        // ADD/MODIFY/DELETE operation. Then fields and structures are set to be ready to be filled 
        // with data for a new PassiveTemplate
        {
        // Passive related
        passiveNameId="Enter your passive name id here";  // Info for textField
        passiveDescription="Enter your passive description here";  // Info for textField
        // Tags|Passive related
        passiveTagsCount=0;  // Number of tags the PassiveTemplate is managing
        selectedPassiveTag=new int[MAX_TAGS];  // For those 'passiveTagsCount' tags the ones selected
        // Formula|Passive related
        List<Formula> passiveFormulas=new List<Formula>();  // Empty list of formulas
        formulaCount=0;  // Number of formulas the PassiveTemplate is managing
        selectedAttribInEachFormula=new int[MAX_FORMULAS];  // For those 'formulaCount' formulas the attribute they're modifying
        formulasArray=new string[MAX_FORMULAS];  // For those 'formulaCount' formulas, the formulas themselves
        // Slot|Passive related
        List<Template> passiveSlots=new List<Template>();  // Empty list of slots
        slotsCount=0;  // Number of slots the PassiveTemplate is managing
        selectedTemplateTypeInEachSlot=new int[MAX_SLOTS];  // For those 'slotsCount' slots, the kind of every one of them
        // PassiveSlot|Slot|Passive Related       
        selectedWhenPassive=new int[MAX_SLOTS];  // For those 'slotsCount' slots, if the slot is PassiveTemplate, when the passive could be executed
        selectedToWhomPassive=new int[MAX_SLOTS];  // For those 'slotsCount' slots, if the slot is PassiveTemplate, to whom the passive could be executed
        // ItemSlot|Slot|Passive Related
        // No Item slots in PassiveTemplate
        // SpecSlot|Slot|Passive Related
        // No Spec slots in PassiveTemplate
        }

    private void loadselectedPassiveTemplate()
        // Called when you select a stored Passive on Passive Popup. Then loads every Passive related info from 
        // database into local structures so GUI can show the info for selected Passive
        {
        passiveNameId=d.Passives[passivesInDatabaseArray[selectedPassive]].NameId; 
        passiveDescription=d.Passives[passivesInDatabaseArray[selectedPassive]].Description;
        passiveTags=d.Passives[passivesInDatabaseArray[selectedPassive]].Tags; 
        passiveFormulas=d.Passives[passivesInDatabaseArray[selectedPassive]].Formulas;
        passiveAllowedSlots=d.Passives[passivesInDatabaseArray[selectedPassive]].AllowedSlots;
        }

    private void constructcurrentPassiveTemplate()
        // Constructs a new PassiveTemplate according to whatever is shown in the fields. After this
        // method is finished this PassiveTemplate could be added to database as a new PassiveTemplate or just 
        // modify one that already exists (in this case destroys the old PassiveTemplate in database 
        // and adds the new one)
        {
        passiveTags=new List<string>();
        for (int i=0; i<passiveTagsCount; i++)
            {
            string tag=passiveTagsInDatabaseArray[selectedPassiveTag[i]];
            passiveTags.Add(tag); 
            }
        passiveFormulas=new List<Formula>(); 
        for (int i=0; i<formulaCount; i++)
            {
            Formula formula=new Formula(attribsInDatabaseArray[selectedAttribInEachFormula[i]],formulasArray[i],1);
            passiveFormulas.Add(formula);
            }
        passiveAllowedSlots=new SlotsConfig();
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
            }
        passiveAllowedSlots.PassiveCfg=passiveCfg;
        passiveAllowedSlots.ItemCfg=itemCfg;
        passiveAllowedSlots.SpecCfg=specCfg;
        currentPassiveTemplate=ScriptableObject.CreateInstance<PassiveTemplate>().Init(passiveNameId,passiveDescription,passiveTags,passiveFormulas,passiveAllowedSlots);
        }
    #endregion

    #region GUI: CreateWindow(), OnGUI()
    [MenuItem("TRPG/Passives",false,3)]
    static void CreateWindow()
        {
        var d=Database.Instance;
        PassiveEditorWindow window=(PassiveEditorWindow)EditorWindow.GetWindow(typeof(PassiveEditorWindow));
        window.Show();
        }

    void OnGUI()
        {
        #region Passive selection zone
        //////////////////////////////
        EditorGUILayout.BeginVertical("Box");
        EditorGUI.BeginChangeCheck();
        selectedPassive=EditorGUILayout.Popup(selectedPassive,passivesInDatabaseArray,GUILayout.Width(150));
        if(EditorGUI.EndChangeCheck())
            {
            if(selectedPassive==0)  // if 'selectedPassive' is '<NEW>' then reset the fields
                {
                loadInfoFromDatabase();
                createEmptyPassiveTemplate();
                }
            else  // else if 'selectedPassive' exists then load it from database to manage it
                {
                loadselectedPassiveTemplate();
                updateGUIZones(selectedPassive);
                }
            }
        EditorGUILayout.EndVertical();
        #endregion
        #region Passive basics zone
        ///////////////////////////
        EditorGUILayout.BeginHorizontal("Button");
        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.LabelField("Basics:",EditorStyles.boldLabel);
        if(selectedPassive==0)  // if 'selectedPassive' is '<NEW>' then allow to create a name id
            passiveNameId=EditorGUILayout.TextField("Passive name id",passiveNameId);
        else  // else if 'selectedAttrib' exists then do not allow to create a existing name id
            EditorGUILayout.LabelField("Passive name id              "+passiveNameId);
        passiveDescription=EditorGUILayout.TextField("Passive description",passiveDescription,GUILayout.Height(50));
        EditorGUILayout.EndVertical();
        #endregion
        #region Tags zone
        /////////////////
        EditorGUILayout.BeginVertical("Box");        
        EditorGUILayout.LabelField("Tags:",EditorStyles.boldLabel);
        passiveTagsCount=EditorGUILayout.IntField("Count",passiveTagsCount);
        if (passiveTagsCount<0)
            passiveTagsCount=0;
        else if (passiveTagsCount>MAX_TAGS)
            passiveTagsCount=MAX_TAGS;
        EditorGUILayout.BeginHorizontal();
        for (int i=0; i<passiveTagsCount; i++)
            {
            selectedPassiveTag[i]=EditorGUILayout.Popup(selectedPassiveTag[i],passiveTagsInDatabaseArray,GUILayout.Width(70));
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
            if (slotTypesAllowedArray[selectedTemplateTypeInEachSlot[i]]=="Passive") 
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
        if(selectedPassive==0)
            {       
            if (GUILayout.Button("ADD",GUILayout.Width(80),GUILayout.Height(80)))
                {
                constructcurrentPassiveTemplate();
                if (d.addModDelTemplate(currentPassiveTemplate,"add"))
                    {
                    selectedPassive=0;
                    loadInfoFromDatabase();
                    createEmptyPassiveTemplate();
                    }
                }
            }
        else
            {
            if (GUILayout.Button("MODIFY",GUILayout.Width(80),GUILayout.Height(80)))
                {
                constructcurrentPassiveTemplate();
                if (d.addModDelTemplate(currentPassiveTemplate,"mod"))
                    {
                    selectedPassive=0;
                    loadInfoFromDatabase();
                    createEmptyPassiveTemplate();
                    }
                }
            if (GUILayout.Button("DELETE",GUILayout.Width(80),GUILayout.Height(80)))
                {
                constructcurrentPassiveTemplate();
                if (d.addModDelTemplate(currentPassiveTemplate,"del"))
                    {
                    selectedPassive=0;
                    loadInfoFromDatabase();
                    createEmptyPassiveTemplate();
                    }
                }
            }
        EditorGUILayout.EndHorizontal();
        #endregion
        }
    #endregion

    #region GUI zones: updateGUIZones(), auxiliary methods
    private void updateGUIZones(int selectedPassive)
        // Updates the fields in the Editor according to the passive from database selected in the Passive Popup.
        // This passive is identified by the 'selectedPassive' integer
        {
        updateTagsZone(selectedPassive);
        updateFormulasZone(selectedPassive);
        updateSlotsZone(selectedPassive);
        }

    private void updateTagsZone(int selectedPassive)
        // Updates the Popup tags according to the passive from database selected in the Passive Popup. This passive
        // is identified by the 'selectedPassive' integer
        {
        passiveTagsCount=passiveTags.Count;
        for (int i=0; i<passiveTagsCount; i++)
            {
            selectedPassiveTag[i]=bindTags(passiveTags[i]);
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
        while(!found && i<passiveTagsInDatabaseList.Count)
            {
            if (passiveTagsInDatabaseList[i]==tagStr)
                {
                position=i;
                found=true;
                }
            i++;
            }
        return position;
        }

    private void updateFormulasZone(int selectedPassive)
        // Uptates the Popup formulas and field formulas according to the passive from database selected in the 
        // Passive Popup. This passive is identified by the 'selectedPassive' integer
        {
        formulaCount=formulasCountForEachPassiveArray[selectedPassive-1]; 
        for (int i=0; i<formulaCount; i++)
            {
            selectedAttribInEachFormula[i]=bindFormulas(d.Passives[passivesInDatabaseList[selectedPassive]].Formulas[i].label);  // Attribute selected in each formula
            formulasArray[i]=d.Passives[passivesInDatabaseList[selectedPassive]].Formulas[i].formula;  // Formulas themselves
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

    private void updateSlotsZone(int selectedPassive)
        // Updates the Popup slots and config slots according to the passive from database selected in the 
        // Passive Popup. This passive is identified by the 'selectedPassive' integer
        {
        slotsCount=slotsCountForEachPassiveArray[selectedPassive-1];
        int i=0;            
        foreach (Dictionary<string,string> result in d.Passives[passivesInDatabaseList[selectedPassive]].AllowedSlots.PassiveCfg)
            {
            selectedTemplateTypeInEachSlot[i]=bindSlots("Passive");
            selectedWhenPassive[i]=bindPassives(result["When"],"When");
            selectedToWhomPassive[i]=bindPassives(result["To whom"],"To whom");
            i++;
            }
        foreach (ItemConfig result in d.Passives[passivesInDatabaseList[selectedPassive]].AllowedSlots.ItemCfg)
            {
            }
        foreach (SpecConfig result in d.Passives[passivesInDatabaseList[selectedPassive]].AllowedSlots.SpecCfg)
            {
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