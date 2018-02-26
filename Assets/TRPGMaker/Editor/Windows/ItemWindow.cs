using UnityEngine;
using UnityEditor;
using System.Collections;
using System;
using UnityEditorInternal;

class ItemWindow : LayoutWindow
{
    private ReorderableList listItems;
    private Editor buttonEditor;

    public override void Init()
    {
        createButtonReorderableList();
    }

    public override void Draw(Rect rect)
    {
        if(editor == null)
            editor = Editor.CreateEditor((Item)ScriptableObject.CreateInstance(typeof(Item)));
        editor.OnInspectorGUI();
    }

    public override bool Button(Rect rect)
    {
        var itemTexture = (Texture2D)Resources.Load("Menu/Buttons/sword", typeof(Texture2D));

        if (GUILayout.Button(new GUIContent("Items", itemTexture), "Button"))
        {
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

        listItems.DoLayoutList();

        buttonEditor.serializedObject.ApplyModifiedProperties();
    }

    private void createButtonReorderableList()
    {
        buttonEditor = Editor.CreateEditor(Database.Instance);

        // Get Attributes
        listItems = new ReorderableList(buttonEditor.serializedObject,
                buttonEditor.serializedObject.FindProperty("items"),
                true, false, true, true);

        // Draw attributes
        listItems.drawElementCallback =
            (Rect rect, int index, bool isActive, bool isFocused) => {
                var element = listItems.serializedProperty.GetArrayElementAtIndex(index);
                var item = element.objectReferenceValue as Item;
                rect.y += 2;
                EditorGUI.LabelField(rect, item.name);
            };

        // On select item
        listItems.onSelectCallback = (ReorderableList l) => {
            var item = l.serializedProperty.GetArrayElementAtIndex(l.index).objectReferenceValue as Item;
            editor = Editor.CreateEditor(item);
        };

        // On new item
        listItems.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) => {
            Item item = (Item)ScriptableObject.CreateInstance(typeof(Item));
            item.name = "New Item";
            AssetDatabase.CreateAsset(item, "Assets/TRPGMaker/Source/Items/NewItem.asset");
            editor = Editor.CreateEditor(item);
            Database.Instance.items.Add(item);
            listItems.index = Database.Instance.items.Count - 1;
            DrawButton();
        };

        // On remove item
        listItems.onRemoveCallback = (ReorderableList l) => {
            var item = l.serializedProperty.GetArrayElementAtIndex(l.index).objectReferenceValue as Item;
            Database.Instance.items.Remove(item);

            editor = Editor.CreateEditor((Item)ScriptableObject.CreateInstance(typeof(Item)));
            editor.OnInspectorGUI();

            listItems.index = -1;
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(item));
            DrawButton();
        };
    }
}