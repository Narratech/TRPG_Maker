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

        // All possible equip combinations
        GUILayout.BeginHorizontal();
        GUILayout.Label("Number of combinations for this item:");
        EditorGUILayout.IntField(item.SlotType.Count);
        if(GUILayout.Button("Add combination"))
        {
            item.SlotType.Add(new Modifier.SlotsOcupped());
        }
        GUILayout.EndHorizontal();

        var centeredStyle = new GUIStyle();
        centeredStyle.alignment = TextAnchor.UpperCenter;
        for (int i = 0; i < item.SlotType.Count; i++)
        {
            GUILayout.BeginVertical("Box");
            GUILayout.BeginHorizontal();
            GUILayout.Label("Comination size:");
            EditorGUILayout.IntField(item.SlotType[i].slotsOcupped.Count);
            if (GUILayout.Button("Add slot type also needed"))
            {
                item.SlotType[i].slotsOcupped.Add("");
                serializedObject.Update();
            }
            GUILayout.EndHorizontal();

            for (int j = 0; j < item.SlotType[i].slotsOcupped.Count; j++)
            {
                SerializedProperty property = serializedObject.FindProperty("SlotType").GetArrayElementAtIndex(i).FindPropertyRelative("slotsOcupped").GetArrayElementAtIndex(j);

                EditorGUI.BeginChangeCheck();
                GUILayout.BeginHorizontal();
                int selectedIndex = Database.Instance.slotTypes.IndexOf(property.stringValue);
                Rect rectPopup = EditorGUILayout.GetControlRect();
                selectedIndex = EditorGUI.Popup(rectPopup, selectedIndex, Database.Instance.slotTypes.ToArray());
                if (GUILayout.Button("Remove"))
                {
                    item.SlotType[i].slotsOcupped.RemoveAt(j);
                }
                GUILayout.EndHorizontal();
                if (EditorGUI.EndChangeCheck())
                {
                    property.stringValue = Database.Instance.slotTypes[selectedIndex];
                }
                if (j != item.SlotType[i].slotsOcupped.Count - 1)
                    GUILayout.Label("AND", centeredStyle);
            }
            if (GUILayout.Button("Remove this combination"))
            {
                item.SlotType.RemoveAt(i);
            }
            GUILayout.EndVertical();
            if(i != item.SlotType.Count - 1)
                GUILayout.Label("OR", centeredStyle);
        }
        
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