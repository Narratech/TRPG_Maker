using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(Attribute))]
public class AttributeEditor : Editor {
    
    public bool foldout;

    public override void OnInspectorGUI()
    {
        Rect rectL = EditorGUILayout.GetControlRect();
        Attribute attribute = (Attribute) target;
        var textDimensions = GUI.skin.label.CalcSize(new GUIContent(attribute.name));

        foldout = EditorGUI.Foldout(new Rect(rectL.x, rectL.y, textDimensions.x + 5, rectL.height), foldout, attribute.name);
        if (foldout)
        {
            rectL.height = EditorGUIUtility.singleLineHeight;
            rectL.x += 15;
            rectL.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rectL, serializedObject.FindProperty("name"));
            rectL.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rectL, serializedObject.FindProperty("id"));
            rectL.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rectL, serializedObject.FindProperty("description"));
            rectL.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rectL, serializedObject.FindProperty("isCore"));
        }
    }

}