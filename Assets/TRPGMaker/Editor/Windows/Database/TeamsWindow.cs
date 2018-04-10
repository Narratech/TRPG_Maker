using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class TeamsWindow : LayoutWindow {
    private ReorderableList listTeams;
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
        GUI.Label(new Rect(EditorGUILayout.GetControlRect().x, EditorGUILayout.GetControlRect().y, EditorGUILayout.GetControlRect().width, 30), "List of teams:", customStyle);

        // Create color for each line
        GUIStyle gsLinePair = new GUIStyle();
        gsLinePair.normal.background = MakeTextureColor.MakeTexture(600, 1, new Color(0.5f, 0.5f, 0.5f, 0.5f));
        GUIStyle gsLineOdd = new GUIStyle();
        gsLineOdd.normal.background = MakeTextureColor.MakeTexture(600, 1, new Color(0.5f, 0.5f, 0.5f, 0.0f));

        EditorGUILayout.BeginVertical();
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        for (int i = 0; i < Database.Instance.teams.Count; i++)
        {
            Team team = Database.Instance.teams[i];

            // Changing line color
            if (i % 2 == 0)
                GUILayout.BeginHorizontal(gsLinePair);
            else
                GUILayout.BeginHorizontal(gsLineOdd);
            Rect rect = EditorGUILayout.GetControlRect();
            GUI.Label(rect, team.id + ": " + team.name);
            if (GUILayout.Button(new GUIContent("Edit"), GUILayout.Width(50)))
            {
                editor = Editor.CreateEditor(team);
            }
            else if (GUILayout.Button(new GUIContent("Remove"), GUILayout.Width(90)))
            {
                removeTeam(team);
            }
            GUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    public override bool Button(Rect rect)
    {
        var teamTexture = (Texture2D)Resources.Load("Menu/Buttons/teams", typeof(Texture2D));
        GUIStyle myStyle = new GUIStyle("Button");
        myStyle.padding = new RectOffset(0, 0, 10, 10);

        if (GUILayout.Button(new GUIContent("Teams", teamTexture), myStyle))
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

        listTeams.DoLayoutList();

        buttonEditor.serializedObject.ApplyModifiedProperties();
    }

    private void createButtonReorderableList()
    {
        buttonEditor = Editor.CreateEditor(Database.Instance);

        // Get teams
        listTeams = new ReorderableList(buttonEditor.serializedObject,
                buttonEditor.serializedObject.FindProperty("teams"),
                true, false, true, true);

        // Draw teams
        listTeams.drawElementCallback =
            (Rect rect, int index, bool isActive, bool isFocused) => {
                var element = listTeams.serializedProperty.GetArrayElementAtIndex(index);
                var team = element.objectReferenceValue as Team;
                rect.y += 2;
                EditorGUI.LabelField(rect, team.id + ": " + team.name);
            };

        // On select team
        listTeams.onSelectCallback = (ReorderableList l) => {
            var team = l.serializedProperty.GetArrayElementAtIndex(l.index).objectReferenceValue as Team;
            editor = Editor.CreateEditor(team);
        };

        // On new team
        listTeams.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) => {
            Team team = (Team)ScriptableObject.CreateInstance(typeof(Team));

            var _exists = AssetDatabase.LoadAssetAtPath("Assets/TRPGMaker/Database/Teams/NewTeam.asset", typeof(Team));
            if (_exists == null)
            {
                team.name = "NewTeam";
                AssetDatabase.CreateAsset(team, "Assets/TRPGMaker/Database/Teams/NewTeam.asset");
            }
            else
            {
                string[] existAssets = AssetDatabase.FindAssets("NewTeam");
                bool seted = false;
                int i = 0;
                while (i <= existAssets.Length && !seted)
                {
                    var _existsNumber = AssetDatabase.LoadAssetAtPath("Assets/TRPGMaker/Database/Teams/NewTeam(" + (i + 1) + ").asset", typeof(Team));
                    if (_existsNumber == null)
                    {
                        team.name = "NewTeam(" + (i + 1) + ")";
                        AssetDatabase.CreateAsset(team, "Assets/TRPGMaker/Database/Teams/NewTeam(" + (i + 1) + ").asset");
                        seted = true;
                    }
                    i++;
                }
            }

            editor = Editor.CreateEditor(team);
            Database.Instance.teams.Add(team);
            listTeams.index = Database.Instance.teams.Count - 1;
            DrawButton();
        };

        // On remove team
        listTeams.onRemoveCallback = (ReorderableList l) => {
            var team = l.serializedProperty.GetArrayElementAtIndex(l.index).objectReferenceValue as Team;
            removeTeam(team);
            editor = null;
            DrawMainView();
            DrawButton();
        };

        // No header
        listTeams.headerHeight = 0;
    }

    private void removeTeam(Team team)
    {
        Database.Instance.teams.Remove(team);
        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(team));
        EditorUtility.SetDirty(Database.Instance);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}