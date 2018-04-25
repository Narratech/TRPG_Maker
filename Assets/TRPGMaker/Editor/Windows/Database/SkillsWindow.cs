using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class SkillsWindow : LayoutWindow
{
    private ReorderableList listSkills;
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
        GUI.Label(new Rect(EditorGUILayout.GetControlRect().x, EditorGUILayout.GetControlRect().y, EditorGUILayout.GetControlRect().width, 30), "List of skills:", customStyle);

        // Create color for each line
        GUIStyle gsLinePair = new GUIStyle();
        gsLinePair.normal.background = MakeTextureColor.MakeTexture(600, 1, new Color(0.5f, 0.5f, 0.5f, 0.5f));
        GUIStyle gsLineOdd = new GUIStyle();
        gsLineOdd.normal.background = MakeTextureColor.MakeTexture(600, 1, new Color(0.5f, 0.5f, 0.5f, 0.0f));

        EditorGUILayout.BeginVertical();
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        for (int i = 0; i < Database.Instance.skills.Count; i++)
        {
            Skills skill = Database.Instance.skills[i];

            // Changing line color
            if (i % 2 == 0)
                GUILayout.BeginHorizontal(gsLinePair);
            else
                GUILayout.BeginHorizontal(gsLineOdd);
            Rect rect = EditorGUILayout.GetControlRect();
            GUI.Label(rect, skill.name);
            if (GUILayout.Button(new GUIContent("Edit"), GUILayout.Width(50)))
            {
                editor = Editor.CreateEditor(skill);
            }
            else if (GUILayout.Button(new GUIContent("Remove"), GUILayout.Width(90)))
            {
                removeSkill(skill);
            }
            GUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    public override bool Button(Rect rect)
    {
        var skillTexture = (Texture2D)Resources.Load("Menu/Buttons/skills", typeof(Texture2D));
        GUIStyle myStyle = new GUIStyle("Button");
        myStyle.padding = new RectOffset(0, 0, 10, 10);

        if (GUILayout.Button(new GUIContent("Skills", skillTexture), myStyle))
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

        listSkills.DoLayoutList();

        buttonEditor.serializedObject.ApplyModifiedProperties();
    }

    private void createButtonReorderableList()
    {
        buttonEditor = Editor.CreateEditor(Database.Instance);

        // Get skills
        listSkills = new ReorderableList(buttonEditor.serializedObject,
                buttonEditor.serializedObject.FindProperty("skills"),
                true, false, true, true);

        // Draw skills
        listSkills.drawElementCallback =
            (Rect rect, int index, bool isActive, bool isFocused) => {
                var element = listSkills.serializedProperty.GetArrayElementAtIndex(index);
                var skill = element.objectReferenceValue as Skills;
                rect.y += 2;
                EditorGUI.LabelField(rect, skill.name);
            };

        // On select skill
        listSkills.onSelectCallback = (ReorderableList l) => {
            var skill = l.serializedProperty.GetArrayElementAtIndex(l.index).objectReferenceValue as Skills;
            editor = Editor.CreateEditor(skill);
        };

        // On new skill
        listSkills.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) => {
            Skills skill = (Skills)ScriptableObject.CreateInstance(typeof(Skills));

            var _exists = AssetDatabase.LoadAssetAtPath("Assets/TRPGMaker/Database/Skills/NewSkill.asset", typeof(Skills));
            if (_exists == null)
            {
                skill.name = "NewSkill";
                AssetDatabase.CreateAsset(skill, "Assets/TRPGMaker/Database/Skills/NewSkill.asset");
            }
            else
            {
                string[] existAssets = AssetDatabase.FindAssets("NewSkill");
                bool seted = false;
                int i = 0;
                while (i <= existAssets.Length && !seted)
                {
                    var _existsNumber = AssetDatabase.LoadAssetAtPath("Assets/TRPGMaker/Database/Skills/NewSkill(" + (i + 1) + ").asset", typeof(Skills));
                    if (_existsNumber == null)
                    {
                        skill.name = "NewSkill(" + (i + 1) + ")";
                        AssetDatabase.CreateAsset(skill, "Assets/TRPGMaker/Database/Skills/NewSkill(" + (i + 1) + ").asset");
                        seted = true;
                    }
                    i++;
                }
            }

            editor = Editor.CreateEditor(skill);
            Database.Instance.skills.Add(skill);
            listSkills.index = Database.Instance.skills.Count - 1;
            DrawButton();
        };

        // On remove skill
        listSkills.onRemoveCallback = (ReorderableList l) => {
            var skill = l.serializedProperty.GetArrayElementAtIndex(l.index).objectReferenceValue as Skills;
            removeSkill(skill);
            editor = null;
            DrawMainView();
            DrawButton();
        };

        // No header
        listSkills.headerHeight = 0;
    }

    private void removeSkill(Skills skill)
    {
        Database.Instance.skills.Remove(skill);
        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(skill));
        EditorUtility.SetDirty(Database.Instance);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}