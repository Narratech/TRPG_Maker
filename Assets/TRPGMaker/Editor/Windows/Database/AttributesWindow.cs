using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class AttributesWindow : LayoutWindow
{
    private ReorderableList listAttributes;
    private Vector2 scrollPosition;

    public override void Init()
    {
        createReorderableList();
    }

    public override void Draw(Rect rect)
    {        
        editor.serializedObject.Update();

        GUILayout.BeginVertical();
        EditorGUILayout.LabelField("Attributes", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("This is the attributes list editor. You can:" +
            "\n - Add a new attribute clicking on the \"+\" symbol." +
            "\n - Edit an attribute expanding with the arrow and changing any value." +
            "\n - Remove any attribute selecting it and clicking on the \"-\" symbol or right-click on it and select \"Delete array element.\"", MessageType.Info);

        EditorGUILayout.BeginVertical();
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        listAttributes.DoLayoutList();

        GUILayout.EndVertical();

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();

        editor.serializedObject.ApplyModifiedProperties();
    }

    private void createReorderableList()
    {
        editor = Editor.CreateEditor(Database.Instance);

        // Get Attributes
        listAttributes = new ReorderableList(editor.serializedObject,
                editor.serializedObject.FindProperty("attributes"),
                true, true, true, true);

        // Draw attributes
        listAttributes.drawElementCallback =
            (Rect rectL, int index, bool isActive, bool isFocused) => {
                var element = listAttributes.serializedProperty.GetArrayElementAtIndex(index);
                rectL.y += 2;

                if (element.propertyType == SerializedPropertyType.Generic)
                {
                    EditorGUI.LabelField(new Rect(rectL.x + 15, rectL.y, rectL.width, rectL.height), element.displayName);
                }
                rectL.height = EditorGUI.GetPropertyHeight(element, GUIContent.none, true);
                rectL.y += 1;
                EditorGUI.PropertyField(rectL, element, GUIContent.none, true);
                listAttributes.elementHeight = rectL.height + 4.0f;


            };

        listAttributes.elementHeightCallback += (idx) => { return Mathf.Max(EditorGUIUtility.singleLineHeight, EditorGUI.GetPropertyHeight(listAttributes.serializedProperty.GetArrayElementAtIndex(idx), GUIContent.none, true)) + 4.0f; };

        // listAttributes header
        listAttributes.drawHeaderCallback = (Rect rectH) => {
            EditorGUI.LabelField(rectH, "Attributes List");
        };

        // Add attributes
        listAttributes.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) => {
            Database.Instance.attributes.Add(new Attribute("", "", "", 0, 0, 0, false));
        };
    }

    public override bool Button(Rect rect)
    {
        var attributeTexture = (Texture2D)Resources.Load("Menu/Buttons/attributes", typeof(Texture2D));
        GUIStyle myStyle = new GUIStyle("Button");
        myStyle.padding = new RectOffset(0, 0, 10, 10);

        if (GUILayout.Button(new GUIContent("Attributes", attributeTexture), myStyle))
        {
            Draw(rect);
            selected = true;
        }
        
        return selected;
    }
}
