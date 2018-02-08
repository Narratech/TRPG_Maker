using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Linq;

[CustomEditor(typeof(SpecializedClass))]
public class SpecializedClassEditor : Editor {

    private SerializedProperty className;
    private ReorderableList listSlots;
    private ReorderableList listAttributes;
    private ReorderableList listFormulas;

    // Get attributes SpecialClass
    List<Attribute> attributes;
    private string[] attriburesStringArray;
    int selected = 0;

    // Get all attributes SpecialClass
    List<Attribute> allAttributes;
    private string[] allAttriburesStringArray;
    int selectedAll = 0;

    private void reloadAttributes()
    {
        SpecializedClass specializedClass = (SpecializedClass)target;
        specializedClass.refreshAttributes();
        attributes = specializedClass.attributes;
        attriburesStringArray = new string[0];
        attriburesStringArray = attributes.Select(I => I.name).ToArray();
    }

    private void OnEnable()
    {

        className = serializedObject.FindProperty("className");

        // Get Slots
        listSlots = new ReorderableList(serializedObject,
                serializedObject.FindProperty("slots"),
                true, true, true, true);

        // Get Attributes
        listAttributes = new ReorderableList(serializedObject,
                serializedObject.FindProperty("attributes"),
                false, true, true, true);

        // Get Formulas
        listFormulas = new ReorderableList(serializedObject,
                serializedObject.FindProperty("formulas"),
                true, true, true, true);

        // Draw Slots
        listSlots.drawElementCallback =
            (Rect rect, int index, bool isActive, bool isFocused) => {
                var element = listSlots.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;
                EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y, 140, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("slotType"), GUIContent.none);
                EditorGUI.PropertyField(
                    new Rect(rect.x + 150, rect.y, rect.width - 150, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("modifier"), GUIContent.none);
            };

        // Draw attributes
        listAttributes.drawElementCallback =
            (Rect rect, int index, bool isActive, bool isFocused) => {
                var element = listAttributes.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;
                EditorGUI.LabelField(
                    new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    attributes[index].name);
            };

        // Draw formulas
        reloadAttributes();
        listFormulas.drawElementCallback =
            (Rect rect, int index, bool isActive, bool isFocused) => {
                var element = listFormulas.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;
                // EditorGUI.DropdownButton(new Rect(rect.x, rect.y, 60, EditorGUIUtility.singleLineHeight), new GUIContent("asd", "asdasd"), FocusType.Keyboard);
                selected = EditorGUI.Popup(new Rect(rect.x, rect.y, 60, EditorGUIUtility.singleLineHeight),
                    selected, attriburesStringArray);
                EditorGUI.PropertyField(
                    new Rect(rect.x + 70, rect.y, 60, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("operation"), GUIContent.none);
                EditorGUI.PropertyField(
                    new Rect(rect.x + 140, rect.y, 90, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("value"), GUIContent.none);
            };

        // Slots header
        listSlots.drawHeaderCallback = (Rect rect) => {
            EditorGUI.LabelField(rect, "Slots");
        };

        // listFormulas header
        listFormulas.drawHeaderCallback = (Rect rect) => {
            EditorGUI.LabelField(rect, "Formulas");
        };

        // listAttributes header
        listAttributes.drawHeaderCallback = (Rect rect) => {
            EditorGUI.LabelField(rect, "Attributes");
        };

        // Añadir atributo
        listAttributes.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) => {
            var menu = new GenericMenu();
            foreach (Attribute attrib in Database.Instance.attributes)
            {
                if (!attrib.isCore && !attributes.Contains(attrib)) {
                    menu.AddItem(new GUIContent(attrib.name),
                    false, clickHandler,
                    attrib);
                }
            }
            menu.ShowAsContext();
        };

        // Eliminar atributo
        listAttributes.onCanRemoveCallback = (ReorderableList l) => {
            if (attributes[l.index].isCore)
                return false;
            else
                return true;
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(className, new GUIContent("Name: "), GUILayout.MinWidth(100));
        listSlots.DoLayoutList();
        listAttributes.DoLayoutList();
        listFormulas.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }

    private void clickHandler(object target)
    {
        var data = (Attribute) target;
        attributes.Add(data);
        serializedObject.ApplyModifiedProperties();
    }
}
