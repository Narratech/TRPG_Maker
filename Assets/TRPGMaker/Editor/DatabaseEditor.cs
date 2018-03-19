using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using UnityEditor.AnimatedValues;

[CustomEditor(typeof(Database))]
[CanEditMultipleObjects]
public class CustomEditorBase : Editor{

    public override void OnInspectorGUI()
    {
        // Create color for each line
        GUIStyle gsLinePair = new GUIStyle();
        gsLinePair.normal.background = MakeTextureColor.MakeTexture(600, 1, new Color(0.5f, 0.5f, 0.5f, 0.5f));
        GUIStyle gsLineOdd = new GUIStyle();
        gsLineOdd.normal.background = MakeTextureColor.MakeTexture(600, 1, new Color(0.5f, 0.5f, 0.5f, 0.0f));


        // Atributes
        GUILayout.BeginVertical("Box");
        var titleStyle = new GUIStyle();
        titleStyle.alignment = TextAnchor.UpperCenter;
        titleStyle.fontSize = 15;
        titleStyle.fontStyle = FontStyle.Bold;
        GUI.Label(new Rect(EditorGUILayout.GetControlRect().x, EditorGUILayout.GetControlRect().y, EditorGUILayout.GetControlRect().width, 30), "Attributes:", titleStyle);
        for(int i = 0; i < Database.Instance.attributes.Count; i++)
        {
            Attribute attribute = Database.Instance.attributes[i];
            // Changing line color
            if (i % 2 == 0)
                GUILayout.BeginHorizontal(gsLinePair);
            else
                GUILayout.BeginHorizontal(gsLineOdd);
            Rect rect = EditorGUILayout.GetControlRect();
            GUI.Label(rect, attribute.name);
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();

        // Tags
        GUILayout.BeginVertical("Box");
        GUI.Label(new Rect(EditorGUILayout.GetControlRect().x, EditorGUILayout.GetControlRect().y, EditorGUILayout.GetControlRect().width, 30), "Tags:", titleStyle);
        for (int i = 0; i < Database.Instance.tags.Count; i++)
        {
            string tag = Database.Instance.tags[i];
            // Changing line color
            if (i % 2 == 0)
                GUILayout.BeginHorizontal(gsLinePair);
            else
                GUILayout.BeginHorizontal(gsLineOdd);
            Rect rect = EditorGUILayout.GetControlRect();
            GUI.Label(rect, tag);
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();

        // Slot types
        GUILayout.BeginVertical("Box");
        GUI.Label(new Rect(EditorGUILayout.GetControlRect().x, EditorGUILayout.GetControlRect().y, EditorGUILayout.GetControlRect().width, 30), "Slot types:", titleStyle);
        for (int i = 0; i < Database.Instance.slotTypes.Count; i++)
        {
            string slotType = Database.Instance.slotTypes[i];
            // Changing line color
            if (i % 2 == 0)
                GUILayout.BeginHorizontal(gsLinePair);
            else
                GUILayout.BeginHorizontal(gsLineOdd);
            Rect rect = EditorGUILayout.GetControlRect();
            GUI.Label(rect, slotType);
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();

        // Items
        GUILayout.BeginVertical("Box");
        GUI.Label(new Rect(EditorGUILayout.GetControlRect().x, EditorGUILayout.GetControlRect().y, EditorGUILayout.GetControlRect().width, 30), "Items:", titleStyle);
        for (int i = 0; i < Database.Instance.items.Count; i++)
        {
            Modifier item = Database.Instance.items[i];
            // Changing line color
            if (i % 2 == 0)
                GUILayout.BeginHorizontal(gsLinePair);
            else
                GUILayout.BeginHorizontal(gsLineOdd);
            Rect rect = EditorGUILayout.GetControlRect();
            GUI.Label(rect, item.name);
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();

        // Specialized classes
        GUILayout.BeginVertical("Box");
        GUI.Label(new Rect(EditorGUILayout.GetControlRect().x, EditorGUILayout.GetControlRect().y, EditorGUILayout.GetControlRect().width, 30), "Specialized classes:", titleStyle);
        for (int i = 0; i < Database.Instance.specializedClasses.Count; i++)
        {
            SpecializedClass specializedClass = Database.Instance.specializedClasses[i];
            // Changing line color
            if (i % 2 == 0)
                GUILayout.BeginHorizontal(gsLinePair);
            else
                GUILayout.BeginHorizontal(gsLineOdd);
            Rect rect = EditorGUILayout.GetControlRect();
            GUI.Label(rect, specializedClass.name);
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();

        // Characters
        GUILayout.BeginVertical("Box");
        GUI.Label(new Rect(EditorGUILayout.GetControlRect().x, EditorGUILayout.GetControlRect().y, EditorGUILayout.GetControlRect().width, 30), "Characters:", titleStyle);
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
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
    }
}