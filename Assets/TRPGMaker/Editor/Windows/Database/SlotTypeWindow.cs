using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class SlotTypeWindow : LayoutWindow
{
	private ReorderableList listSlotTypes;
    private Vector2 scrollPosition;
    private Texture2D removeTexture;
    private GUIStyle removeStyle;

    public override void Init()
    {
        createReorderableList();

        // Remove button
        removeTexture = (Texture2D)Resources.Load("Buttons/remove", typeof(Texture2D));
    }

    public override void Draw(Rect rect)
    {
        removeStyle = new GUIStyle("Button");
        removeStyle.padding = new RectOffset(2, 2, 2, 2);

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
        EditorUtility.SetDirty(Database.Instance);
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
                //var element = listSlotTypes.serializedProperty.GetArrayElementAtIndex(index);
                SlotType slotType = Database.Instance.slotTypes[index];
                var textDimensions = GUI.skin.label.CalcSize(new GUIContent(slotType.slotName));
                if (isActive) Database.Instance.slotTypes[index].slotName = EditorGUI.TextField(new Rect(rect.x, rect.y, textDimensions.x + 5, rect.height), slotType.slotName);
                else EditorGUI.LabelField(rect, slotType.slotName);

                if (GUI.Button(new Rect(rect.width, rect.y, 16, 16), new GUIContent("", removeTexture), removeStyle))
                {
                    Database.Instance.slotTypes.RemoveAt(index);
                }
            };

        // On new slot type
        listSlotTypes.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) => {
            Database.Instance.slotTypes.Add(new SlotType("New slot type"));
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
