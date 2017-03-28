using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class ItemEditorWindow : EditorWindow
    {
    // Constructor
    public ItemEditorWindow()
        {
        }

    // Methods
    [MenuItem("Window/Item Editor")]
    static void CreateWindow()
        {
        ItemEditorWindow window = (ItemEditorWindow)EditorWindow.GetWindow(typeof(ItemEditorWindow));
        window.Show();
        }

    void OnGUI()
        {
        }
    }
