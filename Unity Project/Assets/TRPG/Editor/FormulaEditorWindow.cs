using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FormulaEditorWindow: EditorWindow
    {
    // Methods
    [MenuItem("TRPG/Formulas",false,2)]
    static void CreateWindow()
        {
        var d=Database.Instance;
        FormulaEditorWindow window = (FormulaEditorWindow)EditorWindow.GetWindow(typeof(FormulaEditorWindow));
        window.Show();
        }
	}
