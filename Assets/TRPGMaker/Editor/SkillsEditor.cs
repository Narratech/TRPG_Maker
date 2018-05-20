using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(Skills))]
public class SkillsEditor : Editor {

    //private static readonly string[] _dontIncludeMe = new string[] { "m_Script" };
    private ReorderableList listFormulas;
    private Texture2D removeTexture;
    int indexFormula;
    private GUIStyle removeStyle;

    private void OnEnable()
    {
        Skills skill = (Skills)target;

        // Remove button
        removeTexture = (Texture2D)Resources.Load("Buttons/remove", typeof(Texture2D));

        // Get Formulas
        listFormulas = new ReorderableList(serializedObject,
                serializedObject.FindProperty("formulas"),
                true, true, true, true);

        // Draw formulas
        listFormulas.drawElementCallback =
            (Rect rect, int index, bool isActive, bool isFocused) => {
                //var element = listFormulas.serializedProperty.GetArrayElementAtIndex(index);
                var formula = skill.formulas[index];
                rect.y += 2;

                indexFormula = Database.Instance.attributes.IndexOf(Database.Instance.attributes.Find(x => x.id == formula.attributeID));

                EditorGUI.BeginChangeCheck();
                indexFormula = EditorGUI.Popup(new Rect(rect.x, rect.y, 50, EditorGUIUtility.singleLineHeight), indexFormula, Database.Instance.attributes.Select(x => x.id).ToArray());

                EditorGUI.LabelField(new Rect(rect.x + 55, rect.y, 10, EditorGUIUtility.singleLineHeight), "=", EditorStyles.boldLabel);

                formula.formula = EditorGUI.TextField(new Rect(rect.x + 70, rect.y, rect.width - 98, EditorGUIUtility.singleLineHeight), formula.formula);

                bool removed = false;
                if (GUI.Button(new Rect(rect.width + 261, rect.y, 16, 16), new GUIContent("", removeTexture), removeStyle))
                {
                    skill.formulas.Remove(skill.formulas[index]);
                    removed = true;
                }

                var f = FormulaScript.Create(formula.formula);
                if (!removed && !f.FormulaParser.IsValidExpression)
                {
                    EditorGUI.LabelField(new Rect(rect.x + 70, rect.y + EditorGUIUtility.singleLineHeight + 2.0f, rect.width, EditorGUIUtility.singleLineHeight), f.FormulaParser.Error);
                }

                if (!removed && EditorGUI.EndChangeCheck())
                    formula.attributeID = Database.Instance.attributes[indexFormula].id;
            };

        listFormulas.elementHeight = (EditorGUIUtility.singleLineHeight * 2) + 4.0f;

        // listFormulas header
        listFormulas.drawHeaderCallback = (Rect rect) => {
            EditorGUI.LabelField(rect, "Formulas");
        };

        // Add formula
        listFormulas.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) => {
            skill.formulas.Add(new Formula());
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
        };

        // Remove formula
        listFormulas.onRemoveCallback = (ReorderableList l) => {
            skill.formulas.Remove(skill.formulas[l.index]);
            serializedObject.ApplyModifiedProperties();
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
        GUI.Label(new Rect(EditorGUILayout.GetControlRect().x, EditorGUILayout.GetControlRect().y, EditorGUILayout.GetControlRect().width, 30), "Editing \"" + ((Skills)target).name + "\" skill:", customStyle);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("name"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("description"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("skillType"));
        //DrawPropertiesExcluding(serializedObject, _dontIncludeMe);

        Skills skill = (Skills) target;

        if (skill.skillType.ToString().Contains("AREA"))
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("areaRange"));
        }

        listFormulas.DoLayoutList();

        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(target);
    }
}
