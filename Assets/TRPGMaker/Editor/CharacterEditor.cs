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
    private ReorderableList listSlotsSpecializedClasses;
    private ReorderableList listAttributes;
    private ReorderableList listAttributesFormulas;

    List<bool> foldout;
    private Vector2 scrollPosition;

    // Get attributes SpecialClass
    List<Attribute> attributes;
    private string[] attributesStringArray;
    private int formulaAttributeSelected = -1;
    private int slotTypeSelected = -1;
    private int slotItemSelected = -1;


    private void reloadAttributes()
    {
        character = (Character) target;
        character.refresh();
        attributes = character.attributes;

        // String array of class atributes
        attributesStringArray = new string[0];
        attributesStringArray = attributes.Select(I => I.name).ToArray();

        foldout = Enumerable.Repeat(false, attributes.Count).ToList();
    }

    private void OnEnable()
    {
        reloadAttributes();

        // Get Slots
        listSlots = new ReorderableList(serializedObject,
                serializedObject.FindProperty("Slots"),
                true, true, true, true);

        // Get slots defined in specialized classes
        listSlotsSpecializedClasses = new ReorderableList(serializedObject,
                serializedObject.FindProperty("specializedClasses"),
                false, true, false, false);

        // Get Attributes
        listAttributes = new ReorderableList(serializedObject,
                serializedObject.FindProperty("attributes"),
                false, true, true, true);

        // Get Attributes Formulas
        listAttributesFormulas = new ReorderableList(serializedObject,
                serializedObject.FindProperty("attributesWithFormulas"),
                false, true, true, true);

        //Get List SpecializedClass 
        listSpecializedClass = new ReorderableList(serializedObject,
                serializedObject.FindProperty("specializedClasses"),
                true, true, true, true);

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

        // Draw Slots defined in specialized classes
        listSlotsSpecializedClasses.drawElementCallback =
            (Rect rect, int index, bool isActive, bool isFocused) => {
                rect.y += 2;

                var element = listSlotsSpecializedClasses.serializedProperty.GetArrayElementAtIndex(index);
                SpecializedClass specializedClass = (SpecializedClass)element.objectReferenceValue;

                int i = 0;
                foreach (Slot slot in specializedClass.slots)
                {
                    if (slot.slotType != null && slot.modifier != null)
                    {
                        EditorGUI.LabelField(new Rect(rect.x, rect.y + (EditorGUIUtility.singleLineHeight * i), rect.width, EditorGUIUtility.singleLineHeight), slot.slotType + " - " + slot.modifier.name);
                        //listSlotsSpecializedClasses.elementHeight += EditorGUIUtility.singleLineHeight;
                        i++;
                    }
                }
                
            };

        // Set height of Slots defined in specialized classes
        listSlotsSpecializedClasses.elementHeightCallback = (index) =>
        {
            var element = listSlotsSpecializedClasses.serializedProperty.GetArrayElementAtIndex(index);
            SpecializedClass specializedClass = (SpecializedClass)element.objectReferenceValue;

            int i = 0;
            foreach (Slot slot in specializedClass.slots)
            {
                if (slot.slotType != null && slot.modifier != null)
                {
                    i++;
                }
            }
            return EditorGUIUtility.singleLineHeight * i;
        };

        //Draw specialzed classes
        listSpecializedClass.drawElementCallback =
            (Rect rect, int index, bool isActive, bool isFocused) => {
                rect.y += 2;
                if(character.specializedClasses[index] != null)
                    EditorGUI.LabelField(
                        new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                        character.specializedClasses[index].name);
            };

        // Draw attributes
        listAttributes.drawElementCallback =
             (Rect rectL, int index, bool isActive, bool isFocused) => {
                 var element = listAttributes.serializedProperty.GetArrayElementAtIndex(index);
                 var textDimensions = GUI.skin.label.CalcSize(new GUIContent(element.displayName));
                 rectL.y += 2;

                 foldout[index] = EditorGUI.Foldout(new Rect(rectL.x, rectL.y, textDimensions.x + 5, rectL.height), foldout[index], element.displayName);
                 if (foldout[index])
                 {
                     rectL.height = EditorGUIUtility.singleLineHeight;
                     rectL.x += 15;
                     rectL.y += EditorGUIUtility.singleLineHeight;
                     EditorGUI.PropertyField(rectL, element.FindPropertyRelative("value"));
                     rectL.y += EditorGUIUtility.singleLineHeight;
                     EditorGUI.PropertyField(rectL, element.FindPropertyRelative("minValue"));
                     rectL.y += EditorGUIUtility.singleLineHeight;
                     EditorGUI.PropertyField(rectL, element.FindPropertyRelative("maxValue"));
                     listAttributes.elementHeight = EditorGUIUtility.singleLineHeight * 4.0f + 4.0f;
                 }
                 else
                 {

                     listAttributes.elementHeight = EditorGUIUtility.singleLineHeight + 4.0f;
                 }
             };

        listAttributes.elementHeightCallback += (idx) => {
            if (foldout[idx]) return EditorGUIUtility.singleLineHeight * 4.0f + 4.0f;
            else return EditorGUIUtility.singleLineHeight + 4.0f;
        };

        // Draw attributes with formulas
        listAttributesFormulas.drawElementCallback =
             (Rect rectL, int index, bool isActive, bool isFocused) => {
                 var element = character.attributesWithFormulas[index];
                 var textDimensions = GUI.skin.label.CalcSize(new GUIContent(element.name));
                 rectL.y += 2;

                 EditorGUI.LabelField(new Rect(rectL.x, rectL.y, textDimensions.x, rectL.height), element.name);
                 EditorGUI.LabelField(new Rect(rectL.x + textDimensions.x - 5, rectL.y, 10, rectL.height), " = ");
                 EditorGUI.LabelField(new Rect(rectL.x + textDimensions.x + 5, rectL.y, rectL.width - textDimensions.x - 5, rectL.height), element.value.ToString());
             };

        // Slots header
        listSlots.drawHeaderCallback = (Rect rect) => {
        	EditorGUI.LabelField(rect, "Slots");
       	};

        // Slots defined in specialized classes header
        listSlotsSpecializedClasses.drawHeaderCallback = (Rect rect) => {
            EditorGUI.LabelField(rect, "Slots defined in specialized classes");
        };

        // listAttributes header
        listAttributes.drawHeaderCallback = (Rect rect) => {
            EditorGUI.LabelField(rect, "Attributes");
        };

        // listAttributes with Formulas header
        listAttributesFormulas.drawHeaderCallback = (Rect rect) => {
            EditorGUI.LabelField(rect, "Attributes from Formulas");
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

        // Add specialized class
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

        // Remove specialized classes
        listSpecializedClass.onRemoveCallback = (ReorderableList l) => {
            character.specializedClasses.Remove(listSpecializedClass.serializedProperty.GetArrayElementAtIndex(l.index).objectReferenceValue as SpecializedClass);
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Check if slots changed
        foreach (Slot s in character.Slots)
        {
            var calculate = false;
            if (s.modifier != null && !s.calculatedFormula)
            {
                Debug.Log(s.modifier.name);
                calculate = true;
                s.calculatedFormula = true;
            }
            if (calculate)
                character.calculateFormulas();
        }

        // Check is specialiazed Classes changed
        if(character.specializedClasses.Count != character.specializedClassesCount)
            character.calculateFormulas();


        var customStyle = new GUIStyle();
        customStyle.alignment = TextAnchor.UpperCenter;
        customStyle.fontSize = 17;
        GUI.Label(new Rect(EditorGUILayout.GetControlRect().x, EditorGUILayout.GetControlRect().y, EditorGUILayout.GetControlRect().width, 30), "Editing \"" + character.name +  "\" character:", customStyle);

        EditorGUILayout.BeginVertical();
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("name"), new GUIContent("Name: "), GUILayout.MinWidth(100));

        if (EditorGUI.EndChangeCheck())
        {
            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath((Character)target), character.name + ".asset");
        }

        listAttributes.DoLayoutList();
        listAttributesFormulas.DoLayoutList();
        listSlots.DoLayoutList();
        listSlotsSpecializedClasses.DoLayoutList();        
        listSpecializedClass.DoLayoutList();

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(target);
    }

	private void clickHandlerAttributes(object target)
    {
        var data = (Attribute) target;
        attributes.Add(data);
        foldout.Add(false);
        serializedObject.ApplyModifiedProperties();
    }

    private void clickHandlerSpecializedClass(object target)
    {
        var data = (SpecializedClass) target;
        character.specializedClasses.Add(data);
        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(this.target);
        reloadAttributes();
    }
}