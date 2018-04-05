using UnityEngine;
using UnityEditor;
using System.Collections;

class DatabaseWindow : EditorWindow
{
    private Vector2 scrollPosition;
    private Rect windowArea;
    private Rect windowRect;
    private Rect rectAct;
    private Editor editor;
    private LayoutWindow rightWindow;
    private MenuOptions menuOption;

    private LayoutWindow welcomeWindow;
    private LayoutWindow attributesWindow;
    private LayoutWindow tagWindow;
    private LayoutWindow slotTypeWindow;
    private LayoutWindow itemWindow;
    private LayoutWindow specilizedWindow;
    private LayoutWindow characterWindow;
    private LayoutWindow teamsWindow;

    [MenuItem("TRPGMaker/Database editor", false, 0)]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(DatabaseWindow), false, "Database Editor");        
    }

    private void OnEnable()
    {
        createMenuLayouts();
    }

    public enum MenuOptions
    {
        WELCOME,
        ATTRIBUTES,
        TAGS,
        SLOT_TYPE,
        ITEMS,
        SPECIALIZED_CLASS,
        CHARACTERS,
        TEAMS
    };

    public void createMenuLayouts()
    {
        welcomeWindow = (Welcomewindow)ScriptableObject.CreateInstance(typeof(Welcomewindow));
        menuOption = MenuOptions.WELCOME;

        attributesWindow = (AttributesWindow)ScriptableObject.CreateInstance(typeof(AttributesWindow));
        attributesWindow.Init();

        tagWindow = (TagWindow)ScriptableObject.CreateInstance(typeof(TagWindow));
        tagWindow.Init();

        slotTypeWindow = (SlotTypeWindow)ScriptableObject.CreateInstance(typeof(SlotTypeWindow));
        slotTypeWindow.Init();

        itemWindow = (ItemWindow)ScriptableObject.CreateInstance(typeof(ItemWindow));
        itemWindow.Init();
        
        specilizedWindow = (SpecializedClassWindow)ScriptableObject.CreateInstance(typeof(SpecializedClassWindow));
        specilizedWindow.Init();

        characterWindow = (CharacterWindow)ScriptableObject.CreateInstance(typeof(CharacterWindow));
        characterWindow.Init();

        teamsWindow = (TeamsWindow)ScriptableObject.CreateInstance(typeof(TeamsWindow));
        teamsWindow.Init();
    }

        void OnGUI()
    {
        EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
        EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(250), GUILayout.ExpandHeight(true));

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        DrawLeftMenu();

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();

        windowArea = EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));        

        BeginWindows();

        switch(menuOption)
        {
            case MenuOptions.WELCOME:
                rightWindow = welcomeWindow;
                break;
            case MenuOptions.ATTRIBUTES:
                rightWindow = attributesWindow;
                break;
            case MenuOptions.TAGS:
                rightWindow = tagWindow;
                break;
            case MenuOptions.SLOT_TYPE:
                rightWindow= slotTypeWindow;
                break;
            case MenuOptions.ITEMS:
                rightWindow = itemWindow;
                break;
            case MenuOptions.SPECIALIZED_CLASS:
                rightWindow = specilizedWindow;
                break;
            case MenuOptions.CHARACTERS:
                rightWindow = characterWindow;
                break;
            case MenuOptions.TEAMS:
                rightWindow = teamsWindow;
                break;
            default:
                break;
        }
    
        rightWindow.Draw(windowArea);

        EndWindows();

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
    }

    void DrawLeftMenu()
    {
        if (attributesWindow.Button(windowArea))
        {
            if(menuOption != MenuOptions.ATTRIBUTES)
                rightWindow.selected = false;
            menuOption = MenuOptions.ATTRIBUTES;
        }

        if (tagWindow.Button(windowArea))
        {
            if(menuOption != MenuOptions.TAGS)
                rightWindow.selected = false;
            menuOption = MenuOptions.TAGS;
        }
        if (slotTypeWindow.Button(windowArea))
        {
            if (menuOption != MenuOptions.SLOT_TYPE)
                rightWindow.selected = false;
            menuOption = MenuOptions.SLOT_TYPE;
        }  
        if (itemWindow.Button(windowArea))
        {
            if (menuOption != MenuOptions.ITEMS)
                rightWindow.selected = false;
            menuOption = MenuOptions.ITEMS;
        }
        if (specilizedWindow.Button(windowArea))
        {
            if (menuOption != MenuOptions.SPECIALIZED_CLASS)
                rightWindow.selected = false;
            menuOption = MenuOptions.SPECIALIZED_CLASS;
        }
        if (characterWindow.Button(windowArea))
        {
            if (menuOption != MenuOptions.CHARACTERS)
                rightWindow.selected = false;
            menuOption = MenuOptions.CHARACTERS;
        }
        if (teamsWindow.Button(windowArea))
        {
            if (menuOption != MenuOptions.TEAMS)
                rightWindow.selected = false;
            menuOption = MenuOptions.TEAMS;
        }
    }

    private void OnDestroy()
    {        
        EditorUtility.SetDirty(Database.Instance);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}