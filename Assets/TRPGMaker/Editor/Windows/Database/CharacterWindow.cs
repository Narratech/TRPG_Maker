using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class CharacterWindow : LayoutWindow
{
    private ReorderableList listCharacters;
    private Editor buttonEditor;
    private Vector2 scrollPosition;

    public override void Init()
    {
        createButtonReorderableList();
    }

    public override void Draw(Rect rect)
    {
        if (editor == null)
            DrawMainView();
        else
            editor.OnInspectorGUI();
    }

    public void DrawMainView()
    {
        var customStyle = new GUIStyle();
        customStyle.alignment = TextAnchor.UpperCenter;
        customStyle.fontSize = 17;
        GUI.Label(new Rect(EditorGUILayout.GetControlRect().x, EditorGUILayout.GetControlRect().y, EditorGUILayout.GetControlRect().width, 30), "List of characters:", customStyle);

        // Create color for each line
        GUIStyle gsLinePair = new GUIStyle();
        gsLinePair.normal.background = MakeTextureColor.MakeTexture(600, 1, new Color(0.5f, 0.5f, 0.5f, 0.5f));
        GUIStyle gsLineOdd = new GUIStyle();
        gsLineOdd.normal.background = MakeTextureColor.MakeTexture(600, 1, new Color(0.5f, 0.5f, 0.5f, 0.0f));

        EditorGUILayout.BeginVertical();
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        for (int i = 0; i < Database.Instance.characters.Count; i++)
        {
            Character character = Database.Instance.characters[i];

            // Changing line color
            if (i % 2 == 0)
                GUILayout.BeginHorizontal(gsLinePair);
            else
                GUILayout.BeginHorizontal(gsLineOdd);
            Rect rect = EditorGUILayout.GetControlRect();            
            GUI.Label(rect, character.name);
            if (Event.current.type == EventType.MouseUp && rect.Contains(Event.current.mousePosition))
            {
                editor = Editor.CreateEditor(character);
                listCharacters.index = Database.Instance.characters.IndexOf(character);
            }
            if (GUILayout.Button(new GUIContent("Edit"), GUILayout.Width(50)))
            {
                editor = Editor.CreateEditor(character);
                listCharacters.index = Database.Instance.characters.IndexOf(character);
            }
            else if (GUILayout.Button(new GUIContent("Remove"), GUILayout.Width(90)))
            {
                removeCharacter(character);
            }
            GUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    public override bool Button(Rect rect)
    {
        var characterTexture = (Texture2D)Resources.Load("Menu/Buttons/characters", typeof(Texture2D));
        GUIStyle myStyle = new GUIStyle("Button");
        myStyle.padding = new RectOffset(0, 0, 10, 10);

        if (GUILayout.Button(new GUIContent("Characters", characterTexture), myStyle))
        {
             editor = null;
            Draw(rect);
            selected = true;
        }

         if (selected)
            DrawButton();
      
        return selected;
    }

    private void DrawButton()
    {
        buttonEditor.serializedObject.Update();

        listCharacters.DoLayoutList();

        buttonEditor.serializedObject.ApplyModifiedProperties();
    }

    private void createButtonReorderableList()
    {
        buttonEditor = Editor.CreateEditor(Database.Instance);

        // Get character
        listCharacters = new ReorderableList(buttonEditor.serializedObject,
                buttonEditor.serializedObject.FindProperty("characters"),
                true, false, true, true);

        // Draw character
        listCharacters.drawElementCallback =
            (Rect rect, int index, bool isActive, bool isFocused) => {
                var element = listCharacters.serializedProperty.GetArrayElementAtIndex(index);
                var character = element.objectReferenceValue as Character;
                rect.y += 2;
                EditorGUI.LabelField(rect, character.name);
            };

        // On select character
        listCharacters.onSelectCallback = (ReorderableList l) => {
            var character = l.serializedProperty.GetArrayElementAtIndex(l.index).objectReferenceValue as Character;
            editor = Editor.CreateEditor(character);
        };

        // On new character
        listCharacters.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) => {
            Character character = (Character)ScriptableObject.CreateInstance(typeof(Character));            
            character.init();

            var _exists = AssetDatabase.LoadAssetAtPath("Assets/TRPGMaker/Database/Characters/NewCharacter.asset", typeof(Character));
            if (_exists == null)
            {
                character.name = "NewCharacter";
                AssetDatabase.CreateAsset(character, "Assets/TRPGMaker/Database/Characters/NewCharacter.asset");
            }
            else
            {
                string[] existAssets = AssetDatabase.FindAssets("NewCharacter");
                bool seted = false;
                int i = 0;
                while (i <= existAssets.Length && !seted)
                {
                    var _existsNumber = AssetDatabase.LoadAssetAtPath("Assets/TRPGMaker/Database/Characters/NewCharacter(" + (i + 1) + ").asset", typeof(Character));
                    if (_existsNumber == null)
                    {
                        character.name = "NewCharacter(" + (i + 1) + ")";
                        AssetDatabase.CreateAsset(character, "Assets/TRPGMaker/Database/Characters/NewCharacter(" + (i + 1) + ").asset");
                        seted = true;
                    }
                    i++;
                }
            }
            
            editor = Editor.CreateEditor(character);
            Database.Instance.characters.Add(character);
            listCharacters.index = Database.Instance.characters.Count - 1;
            DrawButton();
        };

        // On remove character
        listCharacters.onRemoveCallback = (ReorderableList l) => {
            var character = l.serializedProperty.GetArrayElementAtIndex(l.index).objectReferenceValue as Character;
            removeCharacter(character);
            editor = null;
            DrawMainView();
            DrawButton();
        };

        // No header
        listCharacters.headerHeight = 0;
    }

    private void removeCharacter(Character character)
    {
        Database.Instance.characters.Remove(character);
        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(character));
    }
}
