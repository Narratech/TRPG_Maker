﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(Item))]
public class ItemEditor : Editor
{
    bool changed = false;
    private DropDown dropDown;
    private Item item;
    private string tag;
    private string lastTag;
    int lastCursorPos = 0;
    int indexFormula;

    private ReorderableList listFormulas;
    private Vector2 scrollPosition;

    void Init()
    {
        dropDown = new DropDown("Tags:");
        item = (Item)target;

        // Draw stored tags
        drawTags();
    }

    private void OnEnable()
    {
        // Get Formulas
        listFormulas = new ReorderableList(serializedObject,
                serializedObject.FindProperty("_formulas"),
                true, true, true, true);

        // Draw formulas
        listFormulas.drawElementCallback =
            (Rect rect, int index, bool isActive, bool isFocused) => {
                var element = listFormulas.serializedProperty.GetArrayElementAtIndex(index);
                var formula = item.formulas[index];
                rect.y += 2;

                indexFormula = Database.Instance.attributes.IndexOf(Database.Instance.attributes.Find(x => x.id == formula.attributeID));

                EditorGUI.BeginChangeCheck();
                indexFormula = EditorGUI.Popup(new Rect(rect.x, rect.y, 50, EditorGUIUtility.singleLineHeight), indexFormula, Database.Instance.attributes.Select(x => x.id).ToArray());

                EditorGUI.LabelField(new Rect(rect.x + 55, rect.y, 10, EditorGUIUtility.singleLineHeight), "=", EditorStyles.boldLabel);

                formula.formula = EditorGUI.TextField(new Rect(rect.x + 70, rect.y, rect.width - 70, EditorGUIUtility.singleLineHeight), formula.formula);

                var f = FormulaScript.Create(formula.formula);
                if (!f.FormulaParser.IsValidExpression)
                {
                    EditorGUI.LabelField(new Rect(rect.x + 70, rect.y + EditorGUIUtility.singleLineHeight + 2.0f, rect.width, EditorGUIUtility.singleLineHeight), f.FormulaParser.Error);
                }

                if (EditorGUI.EndChangeCheck())
                    formula.attributeID = Database.Instance.attributes[indexFormula].id;
            };

        listFormulas.elementHeight = (EditorGUIUtility.singleLineHeight * 2) + 4.0f;

        // listFormulas header
        listFormulas.drawHeaderCallback = (Rect rect) => {
            EditorGUI.LabelField(rect, "Formulas");
        };

        // Add formula
        listFormulas.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) => {
            item.formulas.Add(new Formula());
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
        };

        // Remove formula
        listFormulas.onRemoveCallback = (ReorderableList l) => {
            item.formulas.Remove(item.formulas[l.index]);
            serializedObject.ApplyModifiedProperties();
        };
    }

    public override void OnInspectorGUI()
    {
        if (dropDown == null)
            Init();

        serializedObject.Update();

        var customStyle = new GUIStyle();
        customStyle.alignment = TextAnchor.UpperCenter;
        customStyle.fontSize = 17;
        GUI.Label(new Rect(EditorGUILayout.GetControlRect().x, EditorGUILayout.GetControlRect().y, EditorGUILayout.GetControlRect().width, 30), "Editing \"" + item.name + "\" item:", customStyle);
        var iconStyle = new GUIStyle();
        iconStyle.alignment = TextAnchor.MiddleCenter;
        iconStyle.margin = new RectOffset(0, 0, 0, 10);
        if (item.icon != null)
            GUILayout.Label(item.icon, iconStyle, GUILayout.MaxHeight(64));

        EditorGUILayout.BeginVertical();
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("name"), new GUIContent("Name: "), GUILayout.MinWidth(100));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("description"), new GUIContent("Description: "), GUILayout.MinWidth(100));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("icon"), new GUIContent("Icon: "), GUILayout.MinWidth(100));

        // Tags dropdown
        dropDownSearch();
        tag = dropDown.LayoutBegin();

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

        // For formulas
        listFormulas.DoLayoutList();        

        // Detect if text changed
        if (EditorGUI.EndChangeCheck())
        {
            changed = true;
        }

        // Text in tag changed
        if (changed && GUI.GetNameOfFocusedControl() != "Tags:" && (lastTag == null || (tag != null && lastTag != tag))) //&& !Database.Instance.tags.Contains(item.tag))
        {
            saveTagsInDatabase();
            lastTag = dropDown.Value;
            changed = false;
        }

        // Save changes
        if (GUI.changed)
        {
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);

            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath((Item)target), item.name + ".asset");
        }

        if (dropDown.LayoutEnd())
        {
            // Save all tags
            if (lastTag == null || (tag != null && lastTag != tag))
            {
                //dropDown.LayoutBegin();
                saveTagsInDatabase();
                lastTag = dropDown.Value;
                //GUI.FocusControl("Tags:");
            }
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(target);
    }

    void dropDownSearch()
    {
        var dropDownList = new List<string>();
        foreach (var r in Database.Instance.tags)
        {
            if (tag != null)
            {
                // Detect comma separator
                if (tag.LastIndexOf(',') != -1)
                    tag = tag.Substring(tag.LastIndexOf(',') + 1, tag.Length - tag.LastIndexOf(',') - 1);
                // Search in Database
                if (tag.Trim() != "" && r.ToLower().Contains(tag.ToLower()))
                    dropDownList.Add(r);
            }
        }
        dropDown.Elements = dropDownList;
    }

    void drawTags()
    {
        dropDown.Value = "";
        for(int i = 0; i < item.tags.Count; i++)
        {
            string s = item.tags[i];
            // If tag removed from database:
            if (!Database.Instance.tags.Contains(s))
                item.tags.Remove(s);
            dropDown.Value += s + ",";
        }
        if (dropDown.Value.Length > 0)
            dropDown.Value = dropDown.Value.Substring(0, dropDown.Value.Length - 1); // Remove last ,
    }

    void saveTagsInDatabase()
    {
        string tagsTemp = dropDown.Value;
        item.tags = new List<string>();
        if (tagsTemp.Trim() != "")
        {
            if (tagsTemp.IndexOf(',') != -1)
            {
                while (tagsTemp.IndexOf(',') != -1)
                {
                    string tagName = tagsTemp.Substring(0, tagsTemp.IndexOf(','));
                    checkTagExistsInDatabaseAndStore(tagName);
                    tagsTemp = tagsTemp.Substring(tagsTemp.IndexOf(',') + 1, tagsTemp.Length - tagsTemp.IndexOf(',') - 1);
                }
            }
            checkTagExistsInDatabaseAndStore(tagsTemp);
        }
        drawTags();
    }

    void checkTagExistsInDatabaseAndStore(string tagName)
    {
        if (!Database.Instance.tags.Contains(tagName))
        {
            if (EditorUtility.DisplayDialog("Create new tag \'" + tagName + "\'?",
                    "The tag \'" + tagName + "\' doesn't exists in Databes, you want to create this tag or remove froms tags assigned?", "Create", "Remove"))
            {
                Database.Instance.tags.Add(tagName);
                item.tags.Add(tagName);
            }
        }
        else
        {
            item.tags.Add(tagName);
        }
    }
}