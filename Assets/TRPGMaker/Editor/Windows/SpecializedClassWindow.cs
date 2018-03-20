using UnityEngine;
using UnityEditor;
using System.Collections;
using System;
using UnityEditorInternal;

class SpecializedClassWindow : LayoutWindow
{
    private ReorderableList listSpecializedClass;
    private Editor buttonEditor;

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
        GUI.Label(new Rect(EditorGUILayout.GetControlRect().x, EditorGUILayout.GetControlRect().y, EditorGUILayout.GetControlRect().width, 30), "List of specialized classes:", customStyle);

        // Create color for each line
        GUIStyle gsLinePair = new GUIStyle();
        gsLinePair.normal.background = MakeTextureColor.MakeTexture(600, 1, new Color(0.5f, 0.5f, 0.5f, 0.5f));
        GUIStyle gsLineOdd = new GUIStyle();
        gsLineOdd.normal.background = MakeTextureColor.MakeTexture(600, 1, new Color(0.5f, 0.5f, 0.5f, 0.0f));

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
            if (GUILayout.Button(new GUIContent("Edit"), GUILayout.Width(50)))
            {
                editor = Editor.CreateEditor(specializedClass);
            }
            else if (GUILayout.Button(new GUIContent("Remove"), GUILayout.Width(90)))
            {
                removeSpecializedClass(specializedClass);
            }
            GUILayout.EndHorizontal();
        }

    }

    public override bool Button(Rect rect)
    {
        var specializedTexture = (Texture2D)Resources.Load("Menu/Buttons/specialized", typeof(Texture2D));
        GUIStyle myStyle = new GUIStyle("Button");
        myStyle.padding = new RectOffset(0, 0, 10, 10);

        if (GUILayout.Button(new GUIContent("Specialized Class", specializedTexture), myStyle))
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

        listSpecializedClass.DoLayoutList();

        buttonEditor.serializedObject.ApplyModifiedProperties();
    }

    private void createButtonReorderableList()
    {
        buttonEditor = Editor.CreateEditor(Database.Instance);

        // Get specialized classes
        listSpecializedClass = new ReorderableList(buttonEditor.serializedObject,
                buttonEditor.serializedObject.FindProperty("specializedClasses"),
                true, false, true, true);

        // Draw specialized class
        listSpecializedClass.drawElementCallback =
            (Rect rect, int index, bool isActive, bool isFocused) => {
                var element = listSpecializedClass.serializedProperty.GetArrayElementAtIndex(index);
                var specializedClass = element.objectReferenceValue as SpecializedClass;
                rect.y += 2;
                EditorGUI.LabelField(rect, specializedClass.name);
            };

        // On select specialized class
        listSpecializedClass.onSelectCallback = (ReorderableList l) => {
            var specializedClass = l.serializedProperty.GetArrayElementAtIndex(l.index).objectReferenceValue as SpecializedClass;
            editor = Editor.CreateEditor(specializedClass);
        };

        // On new specialized class
        listSpecializedClass.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) => {
            SpecializedClass specializedClass = (SpecializedClass)ScriptableObject.CreateInstance(typeof(SpecializedClass));            

            var _exists = AssetDatabase.LoadAssetAtPath("Assets/TRPGMaker/Database/SpecializedClasses/NewSpecializedClass.asset", typeof(SpecializedClass));
            if (_exists == null)
            {
                specializedClass.name = "New Specialized Class";
                AssetDatabase.CreateAsset(specializedClass, "Assets/TRPGMaker/Database/SpecializedClasses/NewSpecializedClass.asset");                
            }
            else
            {
                string[] existAssets = AssetDatabase.FindAssets("NewSpecializedClass");
                bool seted = false;
                int i = 0;
                while(i <= existAssets.Length && !seted)
                {
                    var _existsNumber = AssetDatabase.LoadAssetAtPath("Assets/TRPGMaker/Database/SpecializedClasses/NewSpecializedClass(" + (i + 1) + ").asset", typeof(SpecializedClass));
                    if(_existsNumber == null)
                    {
                        specializedClass.name = "New Specialized Class (" + (i+1) + ")";
                        AssetDatabase.CreateAsset(specializedClass, "Assets/TRPGMaker/Database/SpecializedClasses/NewSpecializedClass(" + (i+1) + ").asset");
                        seted = true;
                    }
                    i++;
                }                
            }

            editor = Editor.CreateEditor(specializedClass);
            Database.Instance.specializedClasses.Add(specializedClass);
            listSpecializedClass.index = Database.Instance.specializedClasses.Count - 1;
            DrawButton();
        };

        // On remove specialized class
        listSpecializedClass.onRemoveCallback = (ReorderableList l) => {
            var specializedClass = l.serializedProperty.GetArrayElementAtIndex(l.index).objectReferenceValue as SpecializedClass;
            removeSpecializedClass(specializedClass);
            editor = null;
            DrawMainView();
            DrawButton();
        };

        // No header
        listSpecializedClass.headerHeight = 0;
    }

    private void removeSpecializedClass(SpecializedClass specializedClass)
    {
        Database.Instance.specializedClasses.Remove(specializedClass);
        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(specializedClass));
    }
}