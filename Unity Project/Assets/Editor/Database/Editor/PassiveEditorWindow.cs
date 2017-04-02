using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class PassiveEditorWindow : EditorWindow
    {
    // Constructor
    public PassiveEditorWindow()
        {
        }

    // Methods
    [MenuItem("TRPG/Database/Passive Editor")]
    static void CreateWindow()
        {
        PassiveEditorWindow window = (PassiveEditorWindow)EditorWindow.GetWindow(typeof(PassiveEditorWindow));
        window.Show();
        }

    void OnGUI()
        {
        }
    }
