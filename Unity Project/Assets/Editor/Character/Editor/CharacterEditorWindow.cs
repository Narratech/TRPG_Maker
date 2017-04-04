using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CharacterEditorWindow: EditorWindow
    {
    // Constructor
    public CharacterEditorWindow()
        {
        }

    // Methods
    [MenuItem("TRPG/Game/Create Character")]
    static void CreateWindow()
        {
        CharacterEditorWindow window=(CharacterEditorWindow)EditorWindow.GetWindow(typeof(CharacterEditorWindow));
        window.Show();
        }

    void OnGUI()
        {
        }
    }
