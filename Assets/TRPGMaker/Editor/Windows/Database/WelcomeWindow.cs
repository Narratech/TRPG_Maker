using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Welcomewindow : LayoutWindow
{
    public override void Init()
    {
        
    }

    public override bool Button(Rect rect)
    {
        return false;
    }

    public override void Draw(Rect rect)
    {
        var titleStyle = new GUIStyle();
        titleStyle.alignment = TextAnchor.UpperCenter;
        titleStyle.fontSize = 15;
        titleStyle.fontStyle = FontStyle.Bold;
        GUI.Label(new Rect(EditorGUILayout.GetControlRect().x, EditorGUILayout.GetControlRect().y, EditorGUILayout.GetControlRect().width, 30), "Welcome!", titleStyle);

        var textStyle = new GUIStyle();
        textStyle.alignment = TextAnchor.UpperCenter;
        GUILayout.Label("This is the database editor, you can select a category from the left panel and start editing it to start creating your game.", textStyle);
    }
}
