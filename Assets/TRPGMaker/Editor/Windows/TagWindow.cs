using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class TagWindow : LayoutWindow
{
	private ReorderableList listTags;

    public override void Init()
    {
        createReorderableList();
    }

    public override void Draw(Rect rect)
    {        
        editor.serializedObject.Update();

        GUILayout.BeginVertical();
        EditorGUILayout.LabelField("Tag", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("This is the tag list editor. You can:" +
            "\n - Add a new tag clicking on the \"+\" symbol." +
            "\n - Edit a tag name." +
			"\n - Remove any tag selecting it and clicking on the \"-\" symbol.", MessageType.Info);

        listTags.DoLayoutList();

        GUILayout.EndVertical();

        editor.serializedObject.ApplyModifiedProperties();
    }

   private void createReorderableList()
    {
        editor = Editor.CreateEditor(Database.Instance);

        // Get tags
        listTags = new ReorderableList(editor.serializedObject,
                editor.serializedObject.FindProperty("tags"),
                true, true, true, true);

        // Draw tags
        listTags.drawElementCallback =
            (Rect rect, int index, bool isActive, bool isFocused) => {
                var element = listTags.serializedProperty.GetArrayElementAtIndex(index);
                var textDimensions = GUI.skin.label.CalcSize(new GUIContent(element.stringValue));
                if (isActive) EditorGUI.PropertyField(new Rect(rect.x, rect.y, textDimensions.x + 5, rect.height), element, GUIContent.none, true);
                else EditorGUI.LabelField(rect, element.stringValue);
            };

        // On new tag
        listTags.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) => {
            Database.Instance.tags.Add("New tag");
        };

        // listTags header
        listTags.drawHeaderCallback = (Rect rectH) => {
            EditorGUI.LabelField(rectH, "Tags List");
        };
    }

    public override bool Button(Rect rect)
    {
        var tagTexture = (Texture2D)Resources.Load("Menu/Buttons/tags", typeof(Texture2D));
        GUIStyle myStyle = new GUIStyle("Button");
        myStyle.padding = new RectOffset(0, 0, 10, 10);


        if (GUILayout.Button(new GUIContent("Tags", tagTexture), myStyle))
        {
            Draw(rect);
            selected = true;
        }

        if(selected)
            DrawButton();        

        return selected;
    }

    private void DrawButton()
    {
        editor.serializedObject.Update();
    }
}
