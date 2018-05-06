using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(Item))]
public class ItemEditor : Editor
{
    bool changed = false;
    private DropDown dropDown;
    private Item item;
    private string tag;
    private string lastTag;
    int lastCursorPos = 0;
    int indexFormula;

    private ReorderableList listFormulas;
    private Vector2 scrollPosition;
    private Texture2D removeTexture;
    private GUIStyle removeStyle;

    void Init()
    {
        dropDown = new DropDown("Tags:");
        item = (Item)target;

        // Draw stored tags
        drawTags();
    }

    private void OnEnable()
    {
        // Remove button
        removeTexture = (Texture2D)Resources.Load("Buttons/remove", typeof(Texture2D));

        // Get Formulas
        listFormulas = new ReorderableList(serializedObject,
                serializedObject.FindProperty("_formulas"),
                true, true, true, true);

        // Draw formulas
        listFormulas.drawElementCallback =
            (Rect rect, int index, bool isActive, bool isFocused) => {
                //var element = listFormulas.serializedProperty.GetArrayElementAtIndex(index);
                var formula = item.formulas[index];
                rect.y += 2;

                indexFormula = Database.Instance.attributes.IndexOf(Database.Instance.attributes.Find(x => x.id == formula.attributeID));

                EditorGUI.BeginChangeCheck();
                indexFormula = EditorGUI.Popup(new Rect(rect.x, rect.y, 50, EditorGUIUtility.singleLineHeight), indexFormula, Database.Instance.attributes.Select(x => x.id).ToArray());

                EditorGUI.LabelField(new Rect(rect.x + 55, rect.y, 10, EditorGUIUtility.singleLineHeight), "=", EditorStyles.boldLabel);

                formula.formula = EditorGUI.TextField(new Rect(rect.x + 70, rect.y, rect.width - 98, EditorGUIUtility.singleLineHeight), formula.formula);

                bool removed = false;
                if (GUI.Button(new Rect(rect.width, rect.y, 16, 16), new GUIContent("", removeTexture), removeStyle))
                {
                    item.formulas.Remove(item.formulas[index]);
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
            item.formulas.Add(new Formula());
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
        };

        // Remove formula
        listFormulas.onRemoveCallback = (ReorderableList l) => {
            item.formulas.Remove(item.formulas[l.index]);
            serializedObject.ApplyModifiedProperties();
        };
    }

    public override void OnInspectorGUI()
    {
        removeStyle = new GUIStyle("Button");
        removeStyle.padding = new RectOffset(2, 2, 2, 2);

        if (dropDown == null)
            Init();

        serializedObject.Update();

        var customStyle = new GUIStyle();
        customStyle.alignment = TextAnchor.UpperCenter;
        customStyle.fontSize = 17;
        GUI.Label(new Rect(EditorGUILayout.GetControlRect().x, EditorGUILayout.GetControlRect().y, EditorGUILayout.GetControlRect().width, 30), "Editing \"" + item.name + "\" item:", customStyle);
        var iconStyle = new GUIStyle();
        iconStyle.alignment = TextAnchor.MiddleCenter;
        iconStyle.margin = new RectOffset(0, 0, 0, 10);
        if (item.icon != null)
            GUILayout.Label(item.icon, iconStyle, GUILayout.MaxHeight(64));

        EditorGUILayout.BeginVertical();
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("name"), new GUIContent("Name: "), GUILayout.MinWidth(100));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("description"), new GUIContent("Description: "), GUILayout.MinWidth(100));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("icon"), new GUIContent("Icon: "), GUILayout.MinWidth(100));

        // Tags dropdown
        dropDownSearch();
        tag = dropDown.LayoutBegin();

        // For formulas
        listFormulas.DoLayoutList();

        // All possible equip combinations
        GUILayout.BeginHorizontal();
        GUILayout.Label("Slots combinations for this item:");        
        GUILayout.EndHorizontal();

        // Add button
        var addTexture = (Texture2D)Resources.Load("Buttons/add", typeof(Texture2D));

        var centeredStyle = new GUIStyle();
        centeredStyle.alignment = TextAnchor.UpperCenter;
        bool removed = false;
        for (int i = 0; i < item.SlotType.Count; i++)
        {
            GUILayout.BeginVertical("Box");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent("", removeTexture), removeStyle, GUILayout.MaxHeight(16), GUILayout.MaxWidth(16)))
            {
                item.SlotType.RemoveAt(i);
                removed = true;
            }
            GUILayout.Label("Slots required:");
            GUILayout.EndHorizontal();

            if (!removed)
            {
                for (int j = 0; j < item.SlotType[i].slotsOcupped.Count; j++)
                {
                    //SerializedProperty property = serializedObject.FindProperty("SlotType").GetArrayElementAtIndex(i).FindPropertyRelative("slotsOcupped").GetArrayElementAtIndex(j);
                    EditorGUI.BeginChangeCheck();
                    GUILayout.BeginHorizontal();
                    int selectedIndex = Database.Instance.slotTypes.IndexOf(item.SlotType[i].slotsOcupped[j]);
                    Rect rectPopup = EditorGUILayout.GetControlRect();
                    selectedIndex = EditorGUI.Popup(rectPopup, selectedIndex, Database.Instance.slotTypes.Select(x => x.slotName).ToArray());

                    if (GUILayout.Button(new GUIContent("", removeTexture), removeStyle, GUILayout.MaxHeight(16), GUILayout.MaxWidth(16)))
                    {
                        item.SlotType[i].slotsOcupped.RemoveAt(j);
                    }
                    GUILayout.EndHorizontal();
                    if (EditorGUI.EndChangeCheck() && selectedIndex != -1)
                    {
                        item.SlotType[i].slotsOcupped[j] = Database.Instance.slotTypes[selectedIndex];
                    }
                    if (j != item.SlotType[i].slotsOcupped.Count - 1)
                        GUILayout.Label("AND", centeredStyle);
                }
            }
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent("", addTexture), removeStyle, GUILayout.MaxHeight(16), GUILayout.MaxWidth(16)))
            {
                item.SlotType[i].slotsOcupped.Add(new SlotType(""));
                serializedObject.Update();
            }            
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            if(i != item.SlotType.Count - 1)
                GUILayout.Label("OR", centeredStyle);
        }

        if (GUILayout.Button("Add new combination"))
        {
            item.SlotType.Add(new Modifier.SlotsOcupped());
        }        

        // Detect if text changed
        if (EditorGUI.EndChangeCheck())
        {
            changed = true;
        }

        // Text in tag changed
        if (changed && GUI.GetNameOfFocusedControl() != "Tags:" && (lastTag == null || (tag != null && lastTag != tag))) //&& !Database.Instance.tags.Contains(item.tag))
        {
            saveTagsInDatabase();
            lastTag = dropDown.Value;
            changed = false;
        }

        // Save changes
        if (GUI.changed)
        {
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);

            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath((Item)target), item.name + ".asset");
        }

        if (dropDown.LayoutEnd())
        {
            // Save all tags
            if (lastTag == null || (tag != null && lastTag != tag))
            {
                //dropDown.LayoutBegin();
                saveTagsInDatabase();
                lastTag = dropDown.Value;
                //GUI.FocusControl("Tags:");
            }
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(target);
    }

    void dropDownSearch()
    {
        var dropDownList = new List<string>();
        foreach (var r in Database.Instance.tags)
        {
            if (tag != null)
            {
                // Detect comma separator
                if (tag.LastIndexOf(',') != -1)
                    tag = tag.Substring(tag.LastIndexOf(',') + 1, tag.Length - tag.LastIndexOf(',') - 1);
                // Search in Database
                if (tag.Trim() != "" && r.tagName.ToLower().Contains(tag.ToLower()))
                    dropDownList.Add(r.tagName);
            }
        }
        dropDown.Elements = dropDownList;
    }

    void drawTags()
    {
        dropDown.Value = "";
        for(int i = 0; i < item.tags.Count; i++)
        {
            Tag s = item.tags[i];
            // If tag removed from database:
            if (!Database.Instance.tags.Contains(s))
                item.tags.Remove(s);
            dropDown.Value += s.tagName + ",";
        }
        if (dropDown.Value.Length > 0)
            dropDown.Value = dropDown.Value.Substring(0, dropDown.Value.Length - 1); // Remove last ,
    }

    void saveTagsInDatabase()
    {
        string tagsTemp = dropDown.Value;
        item.tags = new List<Tag>();
        if (tagsTemp.Trim() != "")
        {
            if (tagsTemp.IndexOf(',') != -1)
            {
                while (tagsTemp.IndexOf(',') != -1)
                {
                    string tagName = tagsTemp.Substring(0, tagsTemp.IndexOf(','));
                    checkTagExistsInDatabaseAndStore(tagName);
                    tagsTemp = tagsTemp.Substring(tagsTemp.IndexOf(',') + 1, tagsTemp.Length - tagsTemp.IndexOf(',') - 1);
                }
            }
            checkTagExistsInDatabaseAndStore(tagsTemp);
        }
        drawTags();
    }

    void checkTagExistsInDatabaseAndStore(string tagName)
    {
        Tag tag = Database.Instance.tags.Find(x => x.tagName == tagName);
        if (tag == null)
        {
            if (EditorUtility.DisplayDialog("Create new tag \'" + tagName + "\'?",
                    "The tag \'" + tagName + "\' doesn't exists in Databes, you want to create this tag or remove froms tags assigned?", "Create", "Remove"))
            {
                tag = new Tag(tagName);
                Database.Instance.tags.Add(tag);
                item.tags.Add(tag);
            }
        }
        else
        {
            item.tags.Add(tag);
        }
    }
}