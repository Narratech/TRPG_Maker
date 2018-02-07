using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(SpecializedClass))]
public class SpecializedClassEditor : Editor {

    private SerializedProperty className;
    private ReorderableList list;

    private void OnEnable()
    {

        className = serializedObject.FindProperty("className");

        list = new ReorderableList(serializedObject,
                serializedObject.FindProperty("slots"),
                true, true, true, true);

        list.drawElementCallback =
            (Rect rect, int index, bool isActive, bool isFocused) => {
                var element = list.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;
                EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y, 60, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("slotType"), GUIContent.none);
                EditorGUI.PropertyField(
                    new Rect(rect.x + 60, rect.y, rect.width - 60 - 30, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("modifier"), GUIContent.none);
            };

        list.drawHeaderCallback = (Rect rect) => {
            EditorGUI.LabelField(rect, "Slots");
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(className, new GUIContent("Name: "), GUILayout.MinWidth(100));
        list.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }
}
