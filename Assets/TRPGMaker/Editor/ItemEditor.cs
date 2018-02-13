using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Item))]
public class ItemEditor : Editor
{
    string temp = "";
    bool changed = false;

    public override void OnInspectorGUI()
    {
        Item item = (Item)target;
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Name"), new GUIContent("Name: "), GUILayout.MinWidth(100));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Description"), new GUIContent("Description: "), GUILayout.MinWidth(100));
        Rect rect = EditorGUILayout.GetControlRect();
        EditorGUI.LabelField(new Rect(rect.x, rect.y, 180, EditorGUIUtility.singleLineHeight), "Tag:");
        item.tag = TextAutocomplete.TextFieldAutoComplete(new Rect(rect.x + 180, rect.y, rect.width - 180, EditorGUIUtility.singleLineHeight), item.tag, Database.Instance.tags.ToArray(), maxShownCount: 10, levenshteinDistance: 0.5f);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("SlotType"), true);
        EditorGUILayout.HelpBox("Each element of the slot defines the types of slots where the item could be placed, multiple 'Slots Occuped' in the same position are used for items who needs multiple slots.", MessageType.Info);
        
        // Detect if text changed
        if (EditorGUI.EndChangeCheck())
        {
            changed = true;
        }

        // Text in tag changed
        if (changed && GUI.GetNameOfFocusedControl() != "AutoCompleteField" && !Database.Instance.tags.Contains(item.tag))
        {
            EditorUtility.DisplayDialog("Warning!",
               "The tag \"" + item.tag  + "\" doesn't exists in Database, default tag will be used!", "Ok");
           item.tag = "";
           changed = false;
        }
    }
}