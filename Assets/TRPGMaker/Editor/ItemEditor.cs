using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(Item))]
public class ItemEditor : Editor
{
    bool changed = false;

    public override void OnInspectorGUI()
    {
        Item item = (Item)target;

        serializedObject.Update();

        var customStyle = new GUIStyle();
        customStyle.alignment = TextAnchor.UpperCenter;
        customStyle.fontSize = 17;
        GUI.Label(new Rect(EditorGUILayout.GetControlRect().x, EditorGUILayout.GetControlRect().y, EditorGUILayout.GetControlRect().width, 30), "Editing \"" + item.name + "\" item:", customStyle);

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("name"), new GUIContent("Name: "), GUILayout.MinWidth(100));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("description"), new GUIContent("Description: "), GUILayout.MinWidth(100));
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
            if (item.tag != "" && item.tag != "Enter a Tag (Empty could be used by all characters)")
            {
                EditorUtility.DisplayDialog("Warning!",
                   "The tag \"" + item.tag + "\" doesn't exists in Database, default tag will be used!", "Ok");
                item.tag = "";
                changed = false;
            }
        }

        // Save changes
        if (GUI.changed)
        {
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());

            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath((Item)target), item.name + ".asset");
        }
    }
}