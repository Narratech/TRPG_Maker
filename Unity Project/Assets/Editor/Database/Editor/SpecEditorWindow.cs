using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class SpecEditorWindow: EditorWindow
    {
    // Constants
    private static int MAX_FORMULAS=1000;
    private static int MAX_SLOTS=1000;
    // Wizard
    private int step;
    // Basics
    string specName;
    string specDescription;
    // Formulas
    private int formulaCount;
    private string[] coreOptions;
    int[] selectedCoreIndex;
    string[] formulaArray;
    // Slots
    private int slotsCount;
    private string[] slotTypes; int[] selectedSlotIndex;
    private string[] specSlots; int[] selectedSpecIndex;
    private string[] itemSlots; int[] selectedItemIndex;
    private string[] passiveSlots; int[] selectedPassiveIndex;

    // Constructor
    public SpecEditorWindow()
        {
        // Wizard
        step=0;
        // Basics
        specName="Enter your spec name here";
        specDescription="Enter your spec description here";
        // Formulas
        formulaCount=0;
        coreOptions=new string[] { "ATK", "DEX", "INT", "LVL" };
        selectedCoreIndex=new int[MAX_FORMULAS];
        formulaArray=new string[MAX_FORMULAS];
        // Slots
        slotsCount=0;
        slotTypes=new string[] {"Spec","Passive","Item","Character"};
        selectedSlotIndex=new int[MAX_SLOTS];

        /*
        specSlots=new string[] {"Humano","Orco","Elfo","Ladron"};
        selectedSpecIndex=0;
        private string[] itemSlots; int[] selectedItemIndex;
        private string[] passiveSlots; int[] selectedPassiveIndex;*/
        }

    // Methods
    [MenuItem("TRPG/Database/Specialization Editor")]
    static void CreateWindow()
        {
        SpecEditorWindow window = (SpecEditorWindow)EditorWindow.GetWindow(typeof(SpecEditorWindow));
        window.Show();
        }

    void OnGUI()
        {
        switch (step)
            {
            case 0:
                // NEW and EDIT buttons zone
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("NEW",GUILayout.Width(200),GUILayout.Height(200)))
                    step=1;
                else if (GUILayout.Button("EDIT",GUILayout.Width(200),GUILayout.Height(200)))
                    step=3;
                EditorGUILayout.EndHorizontal();
                break;
            case 1:
                // Basics zone
                EditorGUILayout.BeginVertical("Box");
                specName=EditorGUILayout.TextField("Spec name",specName);
                specDescription=EditorGUILayout.TextField("Spec description",specDescription,GUILayout.Height(50));
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
                    selectedCoreIndex[i]=EditorGUILayout.Popup(selectedCoreIndex[i],coreOptions,GUILayout.Width(50));
                    formulaArray[i]=EditorGUILayout.TextField("=",formulaArray[i]);
                    EditorGUILayout.EndHorizontal();  
                    }
                EditorGUILayout.EndVertical();
                // Restart and Next buttons zone
                EditorGUILayout.BeginHorizontal("Box");
                if (GUILayout.Button("Restart",GUILayout.Width(100),GUILayout.Height(50)))
                    {
                    setDefaults();
                    step=0;
                    }
                else if (GUILayout.Button("Next (slots) >",GUILayout.Width(100),GUILayout.Height(50)))
                    step=2;
                EditorGUILayout.EndHorizontal();
                break;
            case 2:
                // Slots zone
                EditorGUILayout.BeginVertical("Box");  
                slotsCount=EditorGUILayout.IntField("Slots count",slotsCount);
                if (slotsCount<0)
                    slotsCount=0;
                else if (slotsCount>MAX_SLOTS)
                    slotsCount=MAX_SLOTS;
                for (int i=0; i<slotsCount; i++)
                    {
                    EditorGUILayout.BeginHorizontal();
                    selectedSlotIndex[i]=EditorGUILayout.Popup(selectedSlotIndex[i],slotTypes);
                    //formulaArray[i]=EditorGUILayout.TextField("=",formulaArray[i]);
                    EditorGUILayout.EndHorizontal();  
                    }
                EditorGUILayout.BeginHorizontal("Box");
                EditorGUILayout.LabelField("Slots zone");
                EditorGUILayout.BeginHorizontal("Box");
                // Restart and Save buttons zone
                EditorGUILayout.BeginHorizontal("Box");
                if (GUILayout.Button("Restart", GUILayout.Width(100), GUILayout.Height(50)))
                    {
                    setDefaults();
                    step=0;
                    }
                else if (GUILayout.Button("Save", GUILayout.Width(100), GUILayout.Height(50)))
                    {
                    // Mensaje de salvado! -> aceptar -> step 1
                    saveSpecTemplate();
                    setDefaults();
                    step=0;
                    }
                EditorGUILayout.EndHorizontal();
                break;
            case 3:  // Slots selection
                break;
            default:
                break;
            }
        }

    void saveSpecTemplate()
        {
        // Salva el Template correspondiente a un SpecTemplate y actualiza 'Database'
        }

    void setDefaults()
        {
        // Limpia todas las variables y estructuras temporales para crear la spec y las vuelve a dejar a sus
        // defaults
        }
        /*
        EditorGUILayout.BeginHorizontal(); //BEGIN Whole Window

        //Sidebar
        EditorGUILayout.BeginVertical(GUILayout.Width(200)); // BEGIN Sidebar

        EditorGUILayout.BeginHorizontal(); // BEGIN Board Size

        currentData.SizeX = EditorGUILayout.IntField("Size X", currentData.SizeX);
        currentData.SizeY = EditorGUILayout.IntField("Size Y", currentData.SizeY);

        EditorGUILayout.EndHorizontal(); // END Board Size

        EditorGUILayout.Space();

        if (GUILayout.Button("Fill"))
        {
            currentData.Fill(selectedTile);
        }

        if (GUILayout.Button("Clear"))
        {
            currentData.Clear();
        }

        bool guiEnabled = GUI.enabled;
        GUI.enabled = selectedObjectBoard != null;

        if (GUILayout.Button("Save to Object"))
        {

            //selectedObjectBoard.Load(currentData);
        }
        if (GUILayout.Button("Load From Object"))
        {
            currentData.Load(selectedObjectBoard);
        }

        GUI.enabled = guiEnabled;

        EditorGUILayout.EndVertical(); // END Sidebar

        //Board Editor
        EditorGUILayout.BeginVertical(); // BEGIN Board Editor

        EditorGUILayout.BeginHorizontal(); // BEGIN selectedTile Menu
        foreach (GameBoardTile tile in Enum.GetValues(typeof(GameBoardTile)))
        {
            if (GUILayout.Button(tile.ToString(), (tile == selectedTile ? selectedButton : GUI.skin.button)))
            {
                selectedTile = tile;
            }
        }
        EditorGUILayout.EndHorizontal(); // END selectedTile Menu

        EditorGUILayout.BeginHorizontal(); // BEGIN Board Layout

        #region V2 Code

        EditorGUILayout.BeginVertical(); // BEGIN Row Fill Column
        GUILayout.Space(20);
        for (int y = 0; y < currentData.SizeY; y++)
        {
            if (GUILayout.Button(">", GUILayout.Width(20), GUILayout.Height(50)))
            {
                currentData.FillRow(y, selectedTile);
            }
        }
        EditorGUILayout.EndVertical(); // END Row Fill Column

        #endregion

        for (int x = 0; x < currentData.SizeX; x++)
        {

            EditorGUILayout.BeginVertical(); // BEGIN Sub-Board Layout

            #region V2 Code

            if (GUILayout.Button("v", GUILayout.Width(50), GUILayout.Height(20)))
            {
                currentData.FillColumn(x, selectedTile);
            }

            #endregion

            for (int y = 0; y < currentData.SizeY; y++)
            {
                if (GUILayout.Button(currentData.GetTileValue(x, y).ToString(), GUILayout.Width(50), GUILayout.Height(50)))
                {
                    currentData.SetTileValue(x, y, selectedTile);
                }
            }
            EditorGUILayout.EndVertical(); // END Sub-Board Layout
        }
        EditorGUILayout.EndHorizontal(); // END Board Layout

        EditorGUILayout.EndVertical(); //END Board Editor

        EditorGUILayout.EndHorizontal(); //END Whole Window
    }


*/
}

//    new List<string>(coreOptions).ToArray()
