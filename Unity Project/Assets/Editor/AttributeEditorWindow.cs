using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class AttributeEditorWindow: EditorWindow
    {
    // Current attribute values and container
    bool attribIsCore;  // 'true' if is a core attribute
    string attribLabel;  // Three letters identifier
    string attribName;  // Long identifier
    string attribDescription;
    int attribMinValue;
    int attribMaxValue;
    Attribute temporal;
    // Attributes in database
    List<string> attribsInDatabaseList;  // List of attributes identifiers (string) loaded from database
    string[] attribsInDatabaseArray;  // The same list in aray format for the Editor
    int attribCount;  // Number of real elements in the array
    int selectedAttrib;  // Selected attribute to delete

    // Constructor
    public AttributeEditorWindow()
        {
        Database d=Database.Instance;
        resetFields();
        }

    // Methods
    [MenuItem("Window/Attribute Editor")]
    static void CreateWindow()
        {
        AttributeEditorWindow window = (AttributeEditorWindow)EditorWindow.GetWindow(typeof(AttributeEditorWindow));
        window.Show();
        }

    void OnGUI()
        {
        // Events
        // Left mouse click
        if(Event.current.isMouse && Event.current.type == EventType.mouseDown && Event.current.button == 0)
            reloadAttribsFromDatabase();
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
            else  // else if 'selectedAttrib' exists then manage it directly from database
                temporal=Database.Instance.attributes[attribsInDatabaseArray[selectedAttrib]];
            }
        EditorGUILayout.EndVertical();
        // Edit (add, modify and delete) zone
        EditorGUILayout.BeginVertical("Box");
        if(selectedAttrib==0)  // if 'selectedAttrib' is '<NEW>' then allow to create a label
            temporal.label=EditorGUILayout.TextField("Attribute label",temporal.label);
        else  // else if 'selectedAttrib' exists then do not allow to create a existing label
            EditorGUILayout.LabelField("Attribute label                 " + temporal.label);
        temporal.name=EditorGUILayout.TextField("Attribute name",temporal.name);
        temporal.description=EditorGUILayout.TextField("Attribute description",temporal.description,GUILayout.Height(50));
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
        attribLabel="Enter your attribute label here";
        attribName="Enter your attribute name here";
        attribDescription="Enter your attribute description here";
        attribMinValue=1;
        attribMaxValue=100;
        temporal=new Attribute(attribIsCore, attribLabel,attribName,attribDescription, attribMaxValue, attribMinValue);
        // Attributes in database
        reloadAttribsFromDatabase();
        attribCount=attribsInDatabaseList.Count;
        selectedAttrib=0;
        }

    private void reloadAttribsFromDatabase()
        {
        attribsInDatabaseList=new List<string>(Database.Instance.getAttribIdentifiers());
        attribsInDatabaseList.Insert(0,"<NEW>");
        }
    }

