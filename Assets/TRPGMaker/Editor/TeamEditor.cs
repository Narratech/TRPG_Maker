using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(Team))]
public class TeamEditor : Editor {

    private ReorderableList listCharacters;
    private Vector2 scrollPosition;
    private Texture2D removeTexture;
    private GUIStyle removeStyle;

    private void OnEnable()
    {
        // Remove button
        removeTexture = (Texture2D)Resources.Load("Buttons/remove", typeof(Texture2D));

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

                if (GUI.Button(new Rect(rect.width, rect.y, 16, 16), new GUIContent("", removeTexture), removeStyle))
                {
                    ((Team)target).characters.RemoveAt(index);
                }
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

        // Remove character
        listCharacters.onRemoveCallback = (ReorderableList l) =>
        {
            Team team = (Team)target;
            team.characters.Remove(listCharacters.serializedProperty.GetArrayElementAtIndex(l.index).objectReferenceValue as Character);
        };
    }

    public override void OnInspectorGUI()
    {
        removeStyle = new GUIStyle("Button");
        removeStyle.padding = new RectOffset(2, 2, 2, 2);

        Team team = (Team)target;
        // Clean array if there are null objects
        for (int i = 0; i < team.characters.Count; i++)
        {
            if (team.characters[i] == null)
                team.characters.RemoveAt(i);
        }

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
        EditorUtility.SetDirty(this.target);

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    private void clickHandlerCharacters(object target)
    {
        var data = (Character)target;
        ((Team)this.target).characters.Add(data);
        EditorUtility.SetDirty(this.target);
    }
}
