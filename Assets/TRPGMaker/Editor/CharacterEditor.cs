using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Linq;

[CustomEditor(typeof(Character))]
public class CharacterEditor : Editor {

    private ReorderableList listSpecializedClass;
    private Character character;
    private ReorderableList listSlots;
    private ReorderableList listAttributes;

    private Vector2 scrollPosition;

    // Get attributes SpecialClass
    List<Attribute> attributes;
    List<SpecializedClass> specializedClasses;
    private string[] attributesStringArray;
    private int formulaAttributeSelected = -1;
    private int slotTypeSelected = -1;
    private int slotItemSelected = -1;


    private void reloadAttributes()
    {
        character = (Character) target;
        character.refreshAttributes();
        attributes = character.attributes;

        // String array of class atributes
        attributesStringArray = new string[0];
        attributesStringArray = attributes.Select(I => I.name).ToArray();
    }

    private void OnEnable()
    {
        reloadAttributes();

        // Get Slots
        listSlots = new ReorderableList(serializedObject,
                serializedObject.FindProperty("Slots"),
                true, true, true, true);

        // Get Attributes
        listAttributes = new ReorderableList(serializedObject,
                serializedObject.FindProperty("attributes"),
                false, true, true, true);   

        //Get List SpecializedClass 
        listSpecializedClass = new ReorderableList(serializedObject,
                serializedObject.FindProperty("specializedClass"),
                false, true, true, true);

        // Draw Slots
        listSlots.drawElementCallback =
            (Rect rect, int index, bool isActive, bool isFocused) => {
                rect.y += 2;

                slotTypeSelected = Database.Instance.slotTypes.IndexOf(character.Slots[index].slotType);
                slotItemSelected = Database.Instance.items.IndexOf(character.Slots[index].modifier);

                EditorGUI.BeginChangeCheck();
                EditorGUI.LabelField(new Rect(rect.x, rect.y, 60, EditorGUIUtility.singleLineHeight), "Slot type:");
                slotTypeSelected =  EditorGUI.Popup(new Rect(rect.x + 63, rect.y, 100, EditorGUIUtility.singleLineHeight), slotTypeSelected, Database.Instance.slotTypes.ToArray());
                EditorGUI.LabelField(new Rect(rect.x + 170, rect.y, 35, EditorGUIUtility.singleLineHeight), "Item:");
                slotItemSelected = EditorGUI.Popup(new Rect(rect.x + 208, rect.y, rect.width - 208, EditorGUIUtility.singleLineHeight), slotItemSelected, Database.Instance.items.Select(s => (string)s.name).ToArray());
                if (EditorGUI.EndChangeCheck())
                {
                    if(slotTypeSelected != -1)
                        character.Slots[index].slotType = Database.Instance.slotTypes[slotTypeSelected];
                    if(slotItemSelected != -1)
                        character.Slots[index].modifier = Database.Instance.items[slotItemSelected];
                }
            };

        //Draw specialzed classes
        listSpecializedClass.drawElementCallback =
            (Rect rect, int index, bool isActive, bool isFocused) => {
                rect.y += 2;
                EditorGUI.LabelField(
                    new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    specializedClasses[index].name);
            };       

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

        // Slots header
        listSlots.drawHeaderCallback = (Rect rect) => {
        	EditorGUI.LabelField(rect, "Slots");
       	};

        // listAttributes header
        listAttributes.drawHeaderCallback = (Rect rect) => {
            EditorGUI.LabelField(rect, "Attributes");
        };

        //listSpecializedClass header
        listSpecializedClass.drawHeaderCallback = (Rect rect) => {
            EditorGUI.LabelField(rect, "Specialized Classes");
        };

        // Add slot
        listSlots.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) => {
            character.Slots.Add(new Slot());
        };

        // Add attribute
        listAttributes.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) => {
            var menu = new GenericMenu();
            foreach (Attribute attrib in Database.Instance.attributes)
            {
                if (!attrib.isCore && !attributes.Contains(attrib)) {
                    menu.AddItem(new GUIContent(attrib.name),
						false, clickHandlerAttributes,
                    attrib);
                }
            }
            menu.ShowAsContext();
        };

        // add specialiced class
        listSpecializedClass.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) => {
            var menu = new GenericMenu();
            foreach (SpecializedClass spec in Database.Instance.specializedClasses)
            {
                    menu.AddItem(new GUIContent(spec.name), false, clickHandlerSpecializedClass, spec);
            }
            menu.ShowAsContext();
        };

        // Eliminar atributo
        listAttributes.onCanRemoveCallback = (ReorderableList l) => {
            if (attributes[l.index].isCore)
                return false;
            else
                return true;
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        //EditorGUILayout.LabelField();


        serializedObject.Update();

        var customStyle = new GUIStyle();
        customStyle.alignment = TextAnchor.UpperCenter;
        customStyle.fontSize = 17;
        GUI.Label(new Rect(EditorGUILayout.GetControlRect().x, EditorGUILayout.GetControlRect().y, EditorGUILayout.GetControlRect().width, 30), "Editing \"" + character.name +  "\" character:", customStyle);

        EditorGUILayout.BeginVertical();
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("name"), new GUIContent("Name: "), GUILayout.MinWidth(100));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("distance"), new GUIContent("Distance: "), GUILayout.MinWidth(100));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("height"), new GUIContent("Height: "), GUILayout.MinWidth(100));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("attackRange"), new GUIContent("Attack Range: "), GUILayout.MinWidth(100));

        if (EditorGUI.EndChangeCheck())
        {
            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath((Character)target), character.name + ".asset");
        }

		listSlots.DoLayoutList();
        listAttributes.DoLayoutList();
        //listSpecializedClass.DoLayoutList();
        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

	private void clickHandlerAttributes(object target)
    {
        var data = (Attribute) target;
        attributes.Add(data);
        serializedObject.ApplyModifiedProperties();
    }

    private void clickHandlerSpecializedClass(object target)
    {
        var data = (SpecializedClass) target;
        specializedClasses.Add(data);
        serializedObject.ApplyModifiedProperties();
    }
}