using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class AttributeEditorWindow: EditorWindow
    #region Description
    // The 'AttributeEditorWindow' is the editor that allows to create/edit/remove attributes used in the game. They are stored
    // in the main and singleton database 'Database'
    #endregion
    {
    // Current attribute values and container for the values
    private static int TAG_LENGTH=3;  // Tags should be 3 characters long. Not less, neither more
    bool attribIsCore;  // 'true' if is a core attribute
    string attribId;  // Three letters identifier for the attribute
    string attribName;  // Name of the attribute readable by humans
    string attribDescription;  // Description of the attribute
    int attribMinValue;  // Minimum value the attribute can have
    int attribMaxValue;  // Maximum value the attribute can have
    AttributeTRPG temporal;  // Temporal container for the Attribute
    // Attributes from database
    List<string> attribsInDatabaseList;  // List of attributes identifiers (string) loaded from database
    string[] attribsInDatabaseArray;  // The same list in aray format for the Editor
    int attribCount;  // Number of real elements in the array
    int selectedAttrib;  // Selected attribute to edit/delete

    // Constructor
    public AttributeEditorWindow()
        {
        Database d=Database.Instance;
        resetFields();
        }

    // Methods
    [MenuItem("TRPG/Attributes",false,0)]
    static void CreateWindow()
        {
        var d=Database.Instance;
        AttributeEditorWindow window = (AttributeEditorWindow)EditorWindow.GetWindow(typeof(AttributeEditorWindow));
        window.Show();
        }

    void OnGUI()
        {
        // Events
        // Left mouse click on ADD/DELETE button
        if(Event.current.isMouse && Event.current.type == EventType.mouseDown && Event.current.button == 0)
            loadAttribsFromDatabase();
        // Zones
        // Selection zone
        EditorGUILayout.BeginVertical("Box");
        attribsInDatabaseArray=attribsInDatabaseList.ToArray();
        EditorGUI.BeginChangeCheck();
        selectedAttrib=EditorGUILayout.Popup(selectedAttrib,attribsInDatabaseArray,GUILayout.Width(60));
        if(EditorGUI.EndChangeCheck())
            {
            if(selectedAttrib==0)  // if 'selectedAttrib' is '<NEW>' then reset the fields
                resetFields();
            else  // else if 'selectedAttrib' exists then manage it directly from database through 'temporal' container
                temporal=Database.Instance.Attributes[attribsInDatabaseArray[selectedAttrib]];
            }
        EditorGUILayout.EndVertical();
        // Edit (add, modify and delete) zone
        EditorGUILayout.BeginVertical("Box");
        if(selectedAttrib==0)  // if 'selectedAttrib' is '<NEW>' then allow to create a id
            temporal.id=EditorGUILayout.TextField("Attribute id",temporal.id);
        else  // else if 'selectedAttrib' exists then do not allow to create a existing id
            EditorGUILayout.LabelField("Attribute id                      " + temporal.id);
        temporal.name=EditorGUILayout.TextField("Attribute name",temporal.name);
        temporal.description=EditorGUILayout.TextField("Attribute description",temporal.description,GUILayout.Height(50));
        temporal.isCore=EditorGUILayout.Toggle("Is basic/core attribute?",temporal.isCore);
        temporal.minValue=EditorGUILayout.IntField("Min value",temporal.minValue);
        temporal.maxValue=EditorGUILayout.IntField("Max value",temporal.maxValue);
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical("Box");
        if(selectedAttrib==0)  // if 'selectedAttrib' is '<NEW>' then the button adds
            {            
            if (GUILayout.Button("ADD",GUILayout.Width(80),GUILayout.Height(80)))
                {
                if (Database.Instance.addAttribute(temporal))
                    {
                    resetFields();
                    Debug.Log(">> Attribute added to database!");
                    }
                else
                    Debug.Log("X Error adding attribute to database!");
                }
            }
        else
            {
            if (GUILayout.Button("DELETE",GUILayout.Width(80),GUILayout.Height(80)))
                {
                if (Database.Instance.deleteAttribute(attribsInDatabaseArray[selectedAttrib]))
                    {
                    resetFields();
                    Debug.Log("<< Attribute deleted from database!");
                    }
                else
                    Debug.Log("X Error deleting attribute from database!");
                }
            }
        EditorGUILayout.EndVertical();
        }     
    
    private void resetFields()
        {       
        // Current attributes and container
        attribIsCore=false;
        attribId="Enter your attribute id here";
        attribName="Enter your attribute name here";
        attribDescription="Enter your attribute description here";
        attribMinValue=1;
        attribMaxValue=100;
        temporal=new AttributeTRPG(attribIsCore,attribId,attribName,attribDescription,attribMinValue,attribMaxValue);
        // Attributes from database
        loadAttribsFromDatabase();
        attribCount=attribsInDatabaseList.Count;
        selectedAttrib=0;
        }

    private void loadAttribsFromDatabase()
        {
        attribsInDatabaseList=new List<string>(Database.Instance.getAttribIdentifiers());
        attribsInDatabaseList.Insert(0,"<NEW>");
        }
    }

