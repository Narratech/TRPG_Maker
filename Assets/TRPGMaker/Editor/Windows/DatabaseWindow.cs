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
        EditorWindow.GetWindow(typeof(DatabaseWindow));        
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

        itemWindow = (ItemWindow)ScriptableObject.CreateInstance(typeof(ItemWindow));
        itemWindow.Init();

        specilizedWindow = (SpecializedClassWindow)ScriptableObject.CreateInstance(typeof(SpecializedClassWindow));
        specilizedWindow.Init();
    }

        void OnGUI()
    {
        EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
        var leftMenuRect = EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(250), GUILayout.ExpandHeight(true));

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
            case MenuOptions.ITEMS:
                rightWindow = itemWindow;
                break;
            case MenuOptions.SPECIALIZED_CLASS:
                rightWindow = (SpecializedClassWindow)ScriptableObject.CreateInstance(typeof(SpecializedClassWindow));
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
        var specializedTexture = (Texture2D)Resources.Load("Menu/Buttons/witch_hat", typeof(Texture2D));

        if (attributesWindow.Button(windowArea))
        {
            if(menuOption != MenuOptions.ATTRIBUTES)
                rightWindow.selected = false;
            menuOption = MenuOptions.ATTRIBUTES;
        }

        if (GUILayout.Button(new GUIContent("Tags"), "Button"))
        {
            menuOption = MenuOptions.TAGS;
        }
        if (itemWindow.Button(windowArea))
        {
            if (menuOption != MenuOptions.ITEMS)
                rightWindow.selected = false;
            menuOption = MenuOptions.ITEMS;
        }
        if (GUILayout.Button(new GUIContent("Specialized Class", specializedTexture), "Button"))
        {
            menuOption = MenuOptions.SPECIALIZED_CLASS;
        }
        else if (GUILayout.Button(new GUIContent("Characters"), "Button"))
        {
            menuOption = MenuOptions.CHARACTERS;
        }
        
    }
}