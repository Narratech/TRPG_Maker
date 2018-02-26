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
			"\n - Remove any tag selecting it and clicking on the \"-\" symbol or right-click on it and select \"Delete array element.\"", MessageType.Info);

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
            (Rect rectL, int index, bool isActive, bool isFocused) => {
                var element = listTags.serializedProperty.GetArrayElementAtIndex(index);
                rectL.y += 2;
                rectL.height = EditorGUI.GetPropertyHeight(element, GUIContent.none, true);
                rectL.y += 1;
                /* To be unable to change the name, but you can not change the new label name */
                //EditorGUI.LabelField(rectL, element.stringValue);
                EditorGUI.PropertyField(rectL, element, GUIContent.none, true);
                listTags.elementHeight = rectL.height + 4.0f;


            };

        listTags.elementHeightCallback += (idx) => { return Mathf.Max(EditorGUIUtility.singleLineHeight, EditorGUI.GetPropertyHeight(listTags.serializedProperty.GetArrayElementAtIndex(idx), GUIContent.none, true)) + 4.0f; };

        // listTags header
        listTags.drawHeaderCallback = (Rect rectH) => {
            EditorGUI.LabelField(rectH, "Tags List");
        };
    }

    public override bool Button(Rect rect)
    {
        var tagTexture = (Texture2D)Resources.Load("Menu/Buttons/tags", typeof(Texture2D));

        if (GUILayout.Button(new GUIContent("Tags", tagTexture), "Button"))
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
