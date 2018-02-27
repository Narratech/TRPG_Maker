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

    private LayoutWindow attributesWindow;
    private LayoutWindow tagWindow;
    private LayoutWindow itemWindow;
    private LayoutWindow specilizedWindow;
    private LayoutWindow characterWindow;

    [MenuItem("TRPGMaker/Database editor")]
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
        ATTRIBUTES,
        TAGS,
        ITEMS,
        SPECIALIZED_CLASS,
        CHARACTERS
    };

    public void createMenuLayouts()
    {
        attributesWindow = (AttributesWindow)ScriptableObject.CreateInstance(typeof(AttributesWindow));
        attributesWindow.Init();

        tagWindow = (TagWindow)ScriptableObject.CreateInstance(typeof(TagWindow));
        tagWindow.Init();

        itemWindow = (ItemWindow)ScriptableObject.CreateInstance(typeof(ItemWindow));
        itemWindow.Init();
        
        specilizedWindow = (SpecializedClassWindow)ScriptableObject.CreateInstance(typeof(SpecializedClassWindow));
        specilizedWindow.Init();

        characterWindow = (CharacterWindow)ScriptableObject.CreateInstance(typeof(CharacterWindow));
        characterWindow.Init();
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
            case MenuOptions.ATTRIBUTES:
                rightWindow = attributesWindow;
                break;
            case MenuOptions.TAGS:
                rightWindow = tagWindow;
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
        
    }
}