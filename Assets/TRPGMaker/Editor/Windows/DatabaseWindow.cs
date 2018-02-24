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
    private int menuOption;

    [MenuItem("TRPG/Database")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(DatabaseWindow));
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
            case 0:
                rightWindow = (ItemWindow)ScriptableObject.CreateInstance(typeof(ItemWindow));
                rightWindow.OnGUI();
                break;
            case 1:
                rightWindow = (SpecializedClassWindow)ScriptableObject.CreateInstance(typeof(SpecializedClassWindow));
                rightWindow.OnGUI();
                break;
        }

        EndWindows();

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
    }

    void DrawLeftMenu()
    {
        var itemTexture = (Texture2D)Resources.Load("Menu/Buttons/sword", typeof(Texture2D));
        var specializedTexture = (Texture2D)Resources.Load("Menu/Buttons/witch_hat", typeof(Texture2D));
        if (GUILayout.Button(new GUIContent("Items", itemTexture), "Button"))
        {
            menuOption = 0;
        }
        else if (GUILayout.Button(new GUIContent("Specialized Class", specializedTexture), "Button"))
        {
            menuOption = 1;
        }
    }
}