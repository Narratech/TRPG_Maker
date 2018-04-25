using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Skills))]
public class SkillsEditor : Editor {

    private static readonly string[] _dontIncludeMe = new string[] { "m_Script" };

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        var customStyle = new GUIStyle();
        customStyle.alignment = TextAnchor.UpperCenter;
        customStyle.fontSize = 17;
        GUI.Label(new Rect(EditorGUILayout.GetControlRect().x, EditorGUILayout.GetControlRect().y, EditorGUILayout.GetControlRect().width, 30), "Editing \"" + ((Skills)target).name + "\" skill:", customStyle);

        DrawPropertiesExcluding(serializedObject, _dontIncludeMe);

        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(target);
    }
}
