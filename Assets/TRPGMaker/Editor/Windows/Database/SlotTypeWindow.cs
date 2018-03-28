using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class SlotTypeWindow : LayoutWindow
{
	private ReorderableList listSlotTypes;
    private Vector2 scrollPosition;

    public override void Init()
    {
        createReorderableList();
    }

    public override void Draw(Rect rect)
    {        
        editor.serializedObject.Update();

        GUILayout.BeginVertical();
        EditorGUILayout.LabelField("Slot types", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("This is the slot type list editor. You can:" +
            "\n - Add a new slot type clicking on the \"+\" symbol." +
            "\n - Edit a slot type name." +
			"\n - Remove any slot type selecting it and clicking on the \"-\" symbol.", MessageType.Info);

        EditorGUILayout.BeginVertical();
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        listSlotTypes.DoLayoutList();

        GUILayout.EndVertical();

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();

        editor.serializedObject.ApplyModifiedProperties();
    }

   private void createReorderableList()
    {
        editor = Editor.CreateEditor(Database.Instance);

        // Get slot types
        listSlotTypes = new ReorderableList(editor.serializedObject,
                editor.serializedObject.FindProperty("slotTypes"),
                true, true, true, true);

        // Draw slot types
        listSlotTypes.drawElementCallback =
            (Rect rect, int index, bool isActive, bool isFocused) => {
                var element = listSlotTypes.serializedProperty.GetArrayElementAtIndex(index);
                var textDimensions = GUI.skin.label.CalcSize(new GUIContent(element.stringValue));
                if (isActive) EditorGUI.PropertyField(new Rect(rect.x, rect.y, textDimensions.x + 5, rect.height), element, GUIContent.none, true);
                else EditorGUI.LabelField(rect, element.stringValue);
            };

        // On new slot type
        listSlotTypes.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) => {
            Database.Instance.slotTypes.Add("New slot type");
        };

        // listSlotType header
        listSlotTypes.drawHeaderCallback = (Rect rectH) => {
            EditorGUI.LabelField(rectH, "Slot types List");
        };
    }

    public override bool Button(Rect rect)
    {
        var slotTypesTexture = (Texture2D)Resources.Load("Menu/Buttons/slotType", typeof(Texture2D));
        GUIStyle myStyle = new GUIStyle("Button");
        myStyle.padding = new RectOffset(0, 0, 10, 10);


        if (GUILayout.Button(new GUIContent("Slot types", slotTypesTexture), myStyle))
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
