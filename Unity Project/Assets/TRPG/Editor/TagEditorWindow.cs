using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TagEditorWindow: EditorWindow
    {
    // Methods
    [MenuItem("TRPG/Tags",false,1)]
    static void CreateWindow()
        {
        var d=Database.Instance;
        TagEditorWindow window = (TagEditorWindow)EditorWindow.GetWindow(typeof(TagEditorWindow));
        window.Show();
        }
    }
