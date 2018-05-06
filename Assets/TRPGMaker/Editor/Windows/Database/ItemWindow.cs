using UnityEngine;
using UnityEditor;
using System.Collections;
using System;
using UnityEditorInternal;

class ItemWindow : LayoutWindow
{
    private ReorderableList listItems;
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
        GUI.Label(new Rect(EditorGUILayout.GetControlRect().x, EditorGUILayout.GetControlRect().y, EditorGUILayout.GetControlRect().width, 30), "List of items:", customStyle);

        EditorGUILayout.BeginVertical();
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        // Create color for each line
        GUIStyle gsLinePair = new GUIStyle();
        gsLinePair.normal.background = MakeTextureColor.MakeTexture(600, 1, new Color(0.5f, 0.5f, 0.5f, 0.5f));
        GUIStyle gsLineOdd = new GUIStyle();
        gsLineOdd.normal.background = MakeTextureColor.MakeTexture(600, 1, new Color(0.5f, 0.5f, 0.5f, 0.0f));

        for(int i = 0; i < Database.Instance.items.Count; i++)
        {
            Modifier item = Database.Instance.items[i];

            // Changing line color
            if (i % 2 == 0)
                GUILayout.BeginHorizontal(gsLinePair);
            else
                GUILayout.BeginHorizontal(gsLineOdd);
            Rect rect = EditorGUILayout.GetControlRect();
            GUI.Label(rect, item.name);
            if (Event.current.type == EventType.MouseUp && rect.Contains(Event.current.mousePosition))
            {
                editor = Editor.CreateEditor(item);
                listItems.index = Database.Instance.items.IndexOf(item);
            }
            if (GUILayout.Button(new GUIContent("Edit"), GUILayout.Width(50))){
                editor = Editor.CreateEditor(item);
                listItems.index = Database.Instance.items.IndexOf(item);
            }
            else if (GUILayout.Button(new GUIContent("Remove"), GUILayout.Width(90))){
                removeItem(item);
            }
            GUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();

    }

    public override bool Button(Rect rect)
    {
        var itemTexture = (Texture2D)Resources.Load("Menu/Buttons/items", typeof(Texture2D));
        GUIStyle myStyle = new GUIStyle("Button");
        myStyle.padding = new RectOffset(0, 0, 10, 10);

        if (GUILayout.Button(new GUIContent("Items", itemTexture), myStyle))
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

        listItems.DoLayoutList();

        buttonEditor.serializedObject.ApplyModifiedProperties();
    }

    private void createButtonReorderableList()
    {
        buttonEditor = Editor.CreateEditor(Database.Instance);

        // Get items
        listItems = new ReorderableList(buttonEditor.serializedObject,
                buttonEditor.serializedObject.FindProperty("items"),
                true, false, true, true);

        // Draw items
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

            var _exists = AssetDatabase.LoadAssetAtPath("Assets/TRPGMaker/Database/Items/New Item.asset", typeof(Item));
            if (_exists == null)
            {
                item.name = "New Item";
                AssetDatabase.CreateAsset(item, "Assets/TRPGMaker/Database/Items/New Item.asset");
            }
            else
            {
                string[] existAssets = AssetDatabase.FindAssets("New Item");
                bool seted = false;
                int i = 0;
                while (i <= existAssets.Length && !seted)
                {
                    var _existsNumber = AssetDatabase.LoadAssetAtPath("Assets/TRPGMaker/Database/Items/New Item(" + (i + 1) + ").asset", typeof(Item));
                    if (_existsNumber == null)
                    {
                        item.name = "New Item(" + (i + 1) + ")";
                        AssetDatabase.CreateAsset(item, "Assets/TRPGMaker/Database/Items/New Item(" + (i + 1) + ").asset");
                        seted = true;
                    }
                    i++;
                }
            }

            editor = Editor.CreateEditor(item);
            Database.Instance.items.Add(item);
            listItems.index = Database.Instance.items.Count - 1;
            DrawButton();
        };

        // On remove item
        listItems.onRemoveCallback = (ReorderableList l) => {
            var item = l.serializedProperty.GetArrayElementAtIndex(l.index).objectReferenceValue as Item;
            removeItem(item);
            editor = null;
            DrawMainView();
            DrawButton();
        };

        // No header
        listItems.headerHeight = 0;        
    }

    private void removeItem(Modifier item)
    {
        Database.Instance.items.Remove(item);
        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(item));
    }
}