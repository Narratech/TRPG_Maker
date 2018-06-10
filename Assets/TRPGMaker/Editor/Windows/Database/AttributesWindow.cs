using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class AttributesWindow : LayoutWindow
{
    private ReorderableList listAttributes;
    private Vector2 scrollPosition;
    private Texture2D removeTexture;
    private GUIStyle removeStyle;
    List<bool> foldout;

    public override void Init()
    {
        createReorderableList();
        //Database.Instance.attributes = new List<Attribute>();
        // Remove button
        removeTexture = (Texture2D)Resources.Load("Buttons/remove", typeof(Texture2D));

        foldout = Enumerable.Repeat(false, Database.Instance.attributes.Count).ToList();
    }

    public override void Draw(Rect rect)
    {
        removeStyle = new GUIStyle("Button");
        removeStyle.padding = new RectOffset(2, 2, 2, 2);

        editor.serializedObject.Update();

        GUILayout.BeginVertical();
        EditorGUILayout.LabelField("Attributes", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("This is the attributes list editor. You can:" +
            "\n - Add a new attribute clicking on the \"+\" symbol." +
            "\n - Edit an attribute expanding with the arrow and changing any value." +
            "\n - Remove any attribute selecting it and clicking on the \"-\" symbol or right-click on it and select \"Delete array element.\"" +
            "\n NOTE: If you edit or remove an attribute that already exists in a specialized class it would be deleted from it!", MessageType.Info);

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
                Attribute attribute = element.objectReferenceValue as Attribute;
                if (attribute != null)
                {
                    var textDimensions = GUI.skin.label.CalcSize(new GUIContent(attribute.name));
                    rectL.y += 2;

                    foldout[index] = EditorGUI.Foldout(new Rect(rectL.x, rectL.y, textDimensions.x + 5, rectL.height), foldout[index], attribute.name);
                    if (GUI.Button(new Rect(rectL.width, rectL.y, 16, 16), new GUIContent("", removeTexture), removeStyle))
                    {
                        Database.Instance.attributes.RemoveAt(index);
                    }
                    if (foldout[index])
                    {
                        SerializedObject attr = new SerializedObject(attribute);
                        rectL.height = EditorGUIUtility.singleLineHeight;
                        rectL.x += 15;
                        rectL.width -= 15;
                        rectL.y += EditorGUIUtility.singleLineHeight;
                        GUI.SetNextControlName("Name");
                        EditorGUI.PropertyField(rectL, attr.FindProperty("name"));
                        rectL.y += EditorGUIUtility.singleLineHeight;
                        EditorGUI.PropertyField(rectL, attr.FindProperty("id"));
                        rectL.y += EditorGUIUtility.singleLineHeight;
                        EditorGUI.PropertyField(rectL, attr.FindProperty("description"));
                        rectL.y += EditorGUIUtility.singleLineHeight;
                        EditorGUI.PropertyField(rectL, attr.FindProperty("isCore"));
                        if (GUI.changed)
                        {                            
                            attr.ApplyModifiedProperties();
                            if (GUI.GetNameOfFocusedControl() != "Name")
                                AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(attribute), attribute.name + ".asset");
                        }
                        listAttributes.elementHeight = EditorGUIUtility.singleLineHeight * 5.0f + 4.0f;
                    }
                    else
                    {

                        listAttributes.elementHeight = EditorGUIUtility.singleLineHeight + 4.0f;
                    }                    
                }
            };

        listAttributes.elementHeightCallback += (idx) => {
            if (foldout[idx]) return EditorGUIUtility.singleLineHeight * 5.0f + 4.0f;
            else return EditorGUIUtility.singleLineHeight + 4.0f;
        };

        // listAttributes header
        listAttributes.drawHeaderCallback = (Rect rectH) => {
            EditorGUI.LabelField(rectH, "Attributes List");
        };

        // Add attributes
        listAttributes.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) => {
            Attribute attrb = CreateInstance<Attribute>();
            attrb.name = "New Attribute " + Database.Instance.attributes.Count.ToString("D3");
            attrb.id = "X" + Database.Instance.attributes.Count.ToString("D2");
            Database.Instance.attributes.Add(attrb);
            foldout = Enumerable.Repeat(false, Database.Instance.attributes.Count).ToList();

            var specializedClassesPath = "Assets/TRPGMaker/Database/Attributes";
            var _exists = AssetDatabase.LoadAssetAtPath(specializedClassesPath + "/" + attrb.name + ".asset", typeof(SpecializedClass));
            if (_exists == null)
            {
                //Create the folder if doesn't exist
                if (!System.IO.Directory.Exists(specializedClassesPath))
                {
                    System.IO.Directory.CreateDirectory(specializedClassesPath);
                }
                AssetDatabase.CreateAsset(attrb, specializedClassesPath + "/" + attrb.name + ".asset");
            }

        };

        // Detect attribute changed
        listAttributes.onChangedCallback = (ReorderableList l) => {
            foldout = Enumerable.Repeat(false, Database.Instance.attributes.Count).ToList();
        };

        // On remove attribute
        listAttributes.onRemoveCallback = (ReorderableList l) => {
            var attribute = l.serializedProperty.GetArrayElementAtIndex(l.index).objectReferenceValue as Attribute;
            Database.Instance.attributes.Remove(attribute);
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(attribute));
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
