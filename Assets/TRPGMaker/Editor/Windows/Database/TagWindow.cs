using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class TagWindow : LayoutWindow
{
	private ReorderableList listTags;
    private Vector2 scrollPosition;
    private Texture2D removeTexture;
    private GUIStyle removeStyle;

    public override void Init()
    {
        createReorderableList();

        // Remove button
        removeTexture = (Texture2D)Resources.Load("Buttons/remove", typeof(Texture2D));
        removeStyle = new GUIStyle("Button");
        removeStyle.padding = new RectOffset(2, 2, 2, 2);
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

        EditorGUILayout.BeginVertical();
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        listTags.DoLayoutList();

        GUILayout.EndVertical();

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();

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

                if (GUI.Button(new Rect(rect.width, rect.y, 16, 16), new GUIContent("", removeTexture), removeStyle))
                {
                    Database.Instance.tags.RemoveAt(index);
                }
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
