using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(Team))]
public class TeamEditor : Editor {

    private ReorderableList listCharacters;
    private Vector2 scrollPosition;

    private void OnEnable()
    {

        // Get characters
        listCharacters = new ReorderableList(serializedObject,
            serializedObject.FindProperty("characters"),
            true, true, true, true);

        // Draw characters
        listCharacters.drawElementCallback =
            (Rect rect, int index, bool isActive, bool isFocused) => {
                var element = listCharacters.serializedProperty.GetArrayElementAtIndex(index);
                Character character = element.objectReferenceValue as Character;

                rect.y += 2;
                EditorGUI.LabelField(
                    new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    character.name);
            };

        // Characters header
        listCharacters.drawHeaderCallback = (Rect rect) => {
            EditorGUI.LabelField(rect, "Characters");
        };

        // Add character
        listCharacters.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) => {
            var menu = new GenericMenu();
            foreach (Character character in Database.Instance.characters)
            {
                menu.AddItem(new GUIContent(character.name),
                        false, clickHandlerCharacters,
                    character);
            }
            menu.ShowAsContext();
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        var customStyle = new GUIStyle();
        customStyle.alignment = TextAnchor.UpperCenter;
        customStyle.fontSize = 17;
        GUI.Label(new Rect(EditorGUILayout.GetControlRect().x, EditorGUILayout.GetControlRect().y, EditorGUILayout.GetControlRect().width, 30), "Editing \"" + ((Team)target).name + "\" team:", customStyle);

        EditorGUILayout.BeginVertical();
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("id"), new GUIContent("Id: "), GUILayout.MinWidth(100));
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("name"), new GUIContent("Name: "), GUILayout.MinWidth(100));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("playable"), new GUIContent("Playable: "), GUILayout.MinWidth(100));
        if (EditorGUI.EndChangeCheck())
        {
            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath((Team)target), ((Team)target).name + ".asset");
        }
        listCharacters.DoLayoutList();
        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    private void clickHandlerCharacters(object target)
    {
        var data = (Character)target;
        ((Team)this.target).characters.Add(data);
        serializedObject.ApplyModifiedProperties();
    }
}
