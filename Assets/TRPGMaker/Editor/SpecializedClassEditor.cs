using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Linq;

[CustomEditor(typeof(SpecializedClass))]
public class SpecializedClassEditor : Editor {

    private ReorderableList listTags;
    private ReorderableList listSlots;
    private ReorderableList listAttributes;
    //private ReorderableList listFormulas;
    private ReorderableList listSkills;
    private SpecializedClass specializedClass;
    private Texture2D removeTexture;
    private GUIStyle removeStyle;

    List<bool> foldout;
    private Vector2 scrollPosition;

    // Get attributes SpecialClass
    List<AttributeValue> attributes;
    private string[] attributesStringArray;
    private int formulaAttributeSelected = -1;
    private int slotTypeSelected = -1;
    private int slotItemSelected = -1;

    private void reloadAttributes()
    {
        specializedClass = (SpecializedClass) target;
        specializedClass.refreshAttributes();
        attributes = specializedClass.attributes;

        // String array of class atributes
        attributesStringArray = new string[0];
        attributesStringArray = attributes.Select(I => I.attribute.name).ToArray();

        foldout = Enumerable.Repeat(false, attributes.Count).ToList();
    }

    private void OnEnable()
    {
        // Remove button
        removeTexture = (Texture2D)Resources.Load("Buttons/remove", typeof(Texture2D));
        
        // Get tags
        listTags = new ReorderableList(serializedObject,
			serializedObject.FindProperty("tags"),
			true, true, true, true);

        // Get Slots
        listSlots = new ReorderableList(serializedObject,
                serializedObject.FindProperty("slots"),
                true, true, true, true);

        // Get Attributes
        listAttributes = new ReorderableList(serializedObject,
                serializedObject.FindProperty("attributes"),
                false, true, true, true);

        // Get Formulas
        /*listFormulas = new ReorderableList(serializedObject,
                serializedObject.FindProperty("formulas"),
                true, true, true, true);*/

        // Get Skills
        listSkills = new ReorderableList(serializedObject,
                serializedObject.FindProperty("skills"),
                true, true, true, true);

        reloadAttributes();

		// Draw Tags
		listTags.drawElementCallback =
			(Rect rect, int index, bool isActive, bool isFocused) =>
            {
                rect.y += 2;
                EditorGUI.LabelField(
                    new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    specializedClass.tags[index].tagName);

                if (GUI.Button(new Rect(rect.width, rect.y, 16, 16), new GUIContent("", removeTexture), removeStyle))
                {
                    specializedClass.tags.RemoveAt(index);
                }
            };

        // Draw Slots
        listSlots.drawElementCallback =
            (Rect rect, int index, bool isActive, bool isFocused) => {
                rect.y += 2;

                slotTypeSelected = specializedClass.slots.IndexOf(specializedClass.slots[index]);
                slotItemSelected = Database.Instance.items.IndexOf(specializedClass.slots[index].modifier);

                EditorGUI.BeginChangeCheck();
                EditorGUI.LabelField(new Rect(rect.x, rect.y, 60, EditorGUIUtility.singleLineHeight), "Slot type:");
                slotTypeSelected =  EditorGUI.Popup(new Rect(rect.x + 63, rect.y, 100, EditorGUIUtility.singleLineHeight), slotTypeSelected, Database.Instance.slotTypes.Select(x => x.slotName).ToArray());
                EditorGUI.LabelField(new Rect(rect.x + 170, rect.y, 35, EditorGUIUtility.singleLineHeight), "Item:");
                slotItemSelected = EditorGUI.Popup(new Rect(rect.x + 208, rect.y, rect.width - 238, EditorGUIUtility.singleLineHeight), slotItemSelected, Database.Instance.items.Select(s => (string)s.name).ToArray());
                if (EditorGUI.EndChangeCheck())
                {
                    if(slotTypeSelected != 0)
                        specializedClass.slots[index].slotType = Database.Instance.slotTypes[slotTypeSelected];
                    if(slotItemSelected != -1)
                        specializedClass.slots[index].modifier = Database.Instance.items[slotItemSelected];
                }

                if (GUI.Button(new Rect(rect.width, rect.y, 16, 16), new GUIContent("", removeTexture), removeStyle))
                {
                    specializedClass.slots.RemoveAt(index);
                }
            };
        
        // Draw attributes
        listAttributes.drawElementCallback =
             (Rect rectL, int index, bool isActive, bool isFocused) => {
                 var element = listAttributes.serializedProperty.GetArrayElementAtIndex(index);
                 var textDimensions = GUI.skin.label.CalcSize(new GUIContent(specializedClass.attributes[index].attribute.name));
                 rectL.y += 2;

                 foldout[index] = EditorGUI.Foldout(new Rect(rectL.x, rectL.y, textDimensions.x + 5, rectL.height), foldout[index], specializedClass.attributes[index].attribute.name);
                 if (!specializedClass.attributes[index].attribute.isCore && GUI.Button(new Rect(rectL.width - 14, rectL.y, 16, 16), new GUIContent("", removeTexture), removeStyle))
                 {
                     specializedClass.attributes.RemoveAt(index);
                 }
                 if (foldout[index])
                 {
                     rectL.height = EditorGUIUtility.singleLineHeight;
                     rectL.x += 15;
                     rectL.y += EditorGUIUtility.singleLineHeight;
                     GUI.SetNextControlName("Value");
                     EditorGUI.PropertyField(new Rect(rectL.x, rectL.y, rectL.width - 15, rectL.height), element.FindPropertyRelative("value"));
                     if (GUI.GetNameOfFocusedControl() != "Value" && specializedClass.attributes[index].value > specializedClass.attributes[index].maxValue)
                     {
                         if (EditorUtility.DisplayDialog("Value error!",
                             "The value introduced is greater than the max value", "Ok"))
                         {
                             specializedClass.attributes[index].value = 0;
                         }
                     }
                     rectL.y += EditorGUIUtility.singleLineHeight;
                     GUI.SetNextControlName("MinValue");
                     EditorGUI.PropertyField(new Rect(rectL.x, rectL.y, rectL.width - 15, rectL.height), element.FindPropertyRelative("minValue"));
                     if (GUI.GetNameOfFocusedControl() != "MinValue" && specializedClass.attributes[index].minValue > specializedClass.attributes[index].maxValue)
                     {
                         if (EditorUtility.DisplayDialog("Value error!",
                             "The min value introduced is greater than the max value", "Ok"))
                         {
                             specializedClass.attributes[index].minValue = 0;
                         }
                     }
                     rectL.y += EditorGUIUtility.singleLineHeight;
                     EditorGUI.PropertyField(new Rect(rectL.x, rectL.y, rectL.width - 15, rectL.height), element.FindPropertyRelative("maxValue"));
                     listAttributes.elementHeight = EditorGUIUtility.singleLineHeight * 4.0f + 4.0f;
                 } 
                 else
                 {

                     listAttributes.elementHeight = EditorGUIUtility.singleLineHeight + 4.0f;
                 }
             };

        listAttributes.elementHeightCallback += (idx) => {
            if (foldout[idx]) return EditorGUIUtility.singleLineHeight * 4.0f +4.0f;
            else return EditorGUIUtility.singleLineHeight + 4.0f;
        };

        // Draw formulas
        /*listFormulas.drawElementCallback =
            (Rect rect, int index, bool isActive, bool isFocused) => {
                var element = listFormulas.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;
                EditorGUI.BeginChangeCheck();
                formulaAttributeSelected = attributes.IndexOf(specializedClass.formulas[index].attributeModified);
                if (formulaAttributeSelected < 0) formulaAttributeSelected = 0;
                formulaAttributeSelected = EditorGUI.Popup(new Rect(rect.x, rect.y, 60, EditorGUIUtility.singleLineHeight),
                    formulaAttributeSelected, attributesStringArray);
                EditorGUI.PropertyField(
                    new Rect(rect.x + 70, rect.y, 60, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("operation"), GUIContent.none);
                EditorGUI.PropertyField(
                    new Rect(rect.x + 140, rect.y, 90, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("value"), GUIContent.none);
                if (formulaAttributeSelected < 0)
                    specializedClass.formulas.Remove(specializedClass.formulas[index]);
                if (EditorGUI.EndChangeCheck())
                {
                    specializedClass.formulas[index].setAttributeModified(attributes[formulaAttributeSelected]);
                }
            };*/

        // Draw Skills
        listSkills.drawElementCallback =
            (Rect rect, int index, bool isActive, bool isFocused) => {
                rect.y += 2;
                if (specializedClass.skills[index] != null)
                    EditorGUI.LabelField(
                        new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                        specializedClass.skills[index].name);
                else
                    specializedClass.skills.RemoveAt(index);
            };

        // Tags header
        listTags.drawHeaderCallback = (Rect rect) => {
			EditorGUI.LabelField(rect, "Tags");
		};

        // Slots header
        listSlots.drawHeaderCallback = (Rect rect) => {
        	EditorGUI.LabelField(rect, "Slots");
       	};

        // listFormulas header
        /*listFormulas.drawHeaderCallback = (Rect rect) => {
            EditorGUI.LabelField(rect, "Formulas");
        };*/

        // listSkills header
        listSkills.drawHeaderCallback = (Rect rect) => {
            EditorGUI.LabelField(rect, "Skills");
        };

        // listAttributes header
        listAttributes.drawHeaderCallback = (Rect rect) => {
            EditorGUI.LabelField(rect, "Attributes");
        };

		// Add tags
		listTags.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) => {
			var menu = new GenericMenu();
			foreach (Tag tag in Database.Instance.tags)
			{
				menu.AddItem(new GUIContent(tag.tagName),
						false, clickHandlerTags,
					tag);
				
			}
			menu.ShowAsContext();
		};


        // Add slot
        listSlots.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) => {
            specializedClass.slots.Add(new Slot());
        };

        // Add attribute
        listAttributes.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) => {
            var menu = new GenericMenu();
            foreach (Attribute attrib in Database.Instance.attributes)
            {
                if (!attrib.isCore && !attributes.Any(x => x.attribute == attrib)) {
                    menu.AddItem(new GUIContent(attrib.name),
						false, clickHandlerAttributes,
                    attrib);
                }
            }
            menu.ShowAsContext();
        };

        // Remove attribute
        listAttributes.onCanRemoveCallback = (ReorderableList l) => {
            if (attributes[l.index].attribute.isCore)
                return false;
            else
                return true;
        };

        // Add skill
        listSkills.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) => {
            var menu = new GenericMenu();
            foreach (Skills skill in Database.Instance.skills)
            {
                if (!specializedClass.skills.Contains(skill))
                {
                    menu.AddItem(new GUIContent(skill.name),
                        false, clickHandlerSkills,
                    skill);
                }
            }
            menu.ShowAsContext();
        };

        // Remove skill
        listSkills.onRemoveCallback = (ReorderableList l) => {
            specializedClass.skills.Remove(listSkills.serializedProperty.GetArrayElementAtIndex(l.index).objectReferenceValue as Skills);
        };
    }

    public override void OnInspectorGUI()
    {
        removeStyle = new GUIStyle("Button");
        removeStyle.padding = new RectOffset(2, 2, 2, 2);

        serializedObject.Update();

        var customStyle = new GUIStyle();
        customStyle.alignment = TextAnchor.UpperCenter;
        customStyle.fontSize = 17;
        GUI.Label(new Rect(EditorGUILayout.GetControlRect().x, EditorGUILayout.GetControlRect().y, EditorGUILayout.GetControlRect().width, 30), "Editing \"" + specializedClass.name +  "\" specialized class:", customStyle);

        EditorGUILayout.BeginVertical();
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("name"), new GUIContent("Name: "), GUILayout.MinWidth(100));
        if (EditorGUI.EndChangeCheck())
        {
            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath((SpecializedClass)target), specializedClass.name + ".asset");
        }

        listAttributes.DoLayoutList();
        listTags.DoLayoutList();
		listSlots.DoLayoutList();        
        listSkills.DoLayoutList();
        //listFormulas.DoLayoutList();

        /* // For formulas  
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.LabelField("Formula:", EditorStyles.boldLabel);
        var f = specializedClass.formula;
        f.formula = EditorGUILayout.TextField(f.formula);

        if (!f.FormulaParser.IsValidExpression)
        {
            EditorGUILayout.LabelField(f.FormulaParser.Error);
        }*/

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(specializedClass);
    }

	private void clickHandlerAttributes(object target)
    {
        Attribute data = (Attribute) target;
        AttributeValue attributeValue = (AttributeValue) data.Clone();
        specializedClass.attributes.Add(attributeValue);
        foldout.Add(false);
        serializedObject.ApplyModifiedProperties();
    }

	private void clickHandlerTags(object target)
	{
		var data = (string) target;
		specializedClass.tags.Add (Database.Instance.tags.Find(x => x.tagName == data));
		serializedObject.ApplyModifiedProperties();
	}

    private void clickHandlerSkills(object target)
    {
        var data = (Skills)target;
        specializedClass.skills.Add(data);
        serializedObject.ApplyModifiedProperties();
    }    
}
