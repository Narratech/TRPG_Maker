using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TagEditorWindow: EditorWindow
    #region Description
    // The 'TagEditorWindow' is the editor that allows to create/remove tags for/from the singleton database 'Database'
    #endregion
    {
    #region Attributes
    string _actualTag;  // The name we use when we create a new tag
    List<string> _templateFilterList;  // Filter to show/add tags by template
    string[] _templateFilterArray;  // The same list in aray format for the Editor
    int _templateCount;  // Number of templates in the array
    int _selectedTemplate;  // Selected template to filter
    List<string> _tagsInDatabaseList;  // List of tags (string) loaded from database
    string[] _tagsInDatabaseArray;  // The same list in aray format for the Editor
    int _tagCount;  // Number of real elements in the array
    int _selectedTag;  // Selected tag to delete
    #endregion

    // Constructor
    public TagEditorWindow()
        {
        Database d=Database.Instance;
        resetFields();
        }

    // Methods
    [MenuItem("TRPG/Tags",false,1)]
    static void CreateWindow()
        {
        var d=Database.Instance;
        TagEditorWindow window = (TagEditorWindow)EditorWindow.GetWindow(typeof(TagEditorWindow));
        window.Show();
        }

    void onGUIFilter()
        {
        EditorGUI.BeginChangeCheck();
        _selectedTemplate=EditorGUILayout.Popup(_selectedTemplate,_templateFilterArray,GUILayout.Width(150));
        if(EditorGUI.EndChangeCheck())
            {
            if(_selectedTemplate==0)  // if '_selectedTemplate' is 'Choose...'
                {
                // Limpiar listas de tags
                _tagsInDatabaseList=new List<string>();
                _tagsInDatabaseArray=_tagsInDatabaseList.ToArray();
                // Limpiar lo que pueda haber en el textField
                _actualTag="";
                }
            else  // else if '_selectedTemplate' exists then load the tags
                {
                // Cargar los tags correspondientes al template elegido
                _tagsInDatabaseList=new List<string>(Database.Instance.getTagValues(_templateFilterArray[_selectedTemplate]));
                // Añadir boton <NEW> en el selector de tags
                _tagsInDatabaseList.Insert(0,"<NEW>");
                _tagsInDatabaseArray=_tagsInDatabaseList.ToArray();
                }
            }
        }

    void onGUISelection()
        {
        if (_selectedTemplate!=0)
            {
            EditorGUI.BeginChangeCheck();
            _selectedTag=EditorGUILayout.Popup(_selectedTag,_tagsInDatabaseArray,GUILayout.Width(150));
            if (EditorGUI.EndChangeCheck())
                {
                if(_selectedTag==0)  // if '_selectedTag' is '<NEW>'
                    {
                    // Cargar los tags correspondientes al template elegido
                    _tagsInDatabaseList=new List<string>(Database.Instance.getTagValues(_templateFilterArray[_selectedTemplate]));
                    // Añadir boton <NEW> en el selector de tags
                    _tagsInDatabaseList.Insert(0,"<NEW>");
                    _tagsInDatabaseArray=_tagsInDatabaseList.ToArray();
                    // Limpiar y activar textField
                    _actualTag="Enter your tag name here";
                    }
                }
            }
        }

    void onGUITextField()
        {
        if (_tagsInDatabaseList.Count!=0)
            { 
            if (_selectedTag==0 && _tagsInDatabaseArray[_selectedTag]=="<NEW>")  
                _actualTag=EditorGUILayout.TextField("Actual tag",_actualTag); 
            }
        }

    void onGUIButtons()
        {
        EditorGUILayout.BeginVertical("Box");
        if (_selectedTemplate!=0 && _selectedTag==0 && _tagsInDatabaseArray[_selectedTag]=="<NEW>")  // if '_selectedTemplate' is 'Choose...' then the button adds
            {            
            if (GUILayout.Button("ADD",GUILayout.Width(80),GUILayout.Height(80)))
                {
                Debug.Log("Presionaste ADD");
                   /* 
                if (Database.Instance.addTag(_templateFilterArray[_selectedTemplate],_tagsInDatabaseArray[_selectedTag]))
                    {
                    resetFields();
                    }*/
                }
            }
        else if (_selectedTemplate!=0 && _selectedTag!=0)
            {
            if (GUILayout.Button("DELETE",GUILayout.Width(80),GUILayout.Height(80)))
                {
                Debug.Log("Presionaste DELETE");
                /*
                if (Database.Instance.deleteTag(_templateFilterArray[_selectedTemplate],_tagsInDatabaseArray[_selectedTag]))
                    {
                    resetFields();
                    }
                */
                }
            }
        EditorGUILayout.EndVertical();
        }

    void OnGUI()
        {
        EditorGUILayout.BeginHorizontal("Box");
        onGUIFilter();
        onGUISelection();
        EditorGUILayout.EndHorizontal();
        onGUITextField();
        onGUIButtons();
        }     
    
    private void resetFields()
        {       
        // Current attributes and container
        _actualTag="Enter your tag name here";
        // Attributes from database
        loadTagKeysFromDatabase();
        _tagsInDatabaseList=new List<string>();
        _tagCount=_tagsInDatabaseList.Count;
        _tagsInDatabaseArray=_tagsInDatabaseList.ToArray();
        _selectedTag=0;
        }

    private void loadTagKeysFromDatabase()
        {
        _templateFilterList=new List<string>(Database.Instance.getTagKeys());
        _templateFilterList.Insert(0,"Choose...");
        _templateFilterArray=_templateFilterList.ToArray();
        //_tagsInDatabaseList=new List<string>(Database.Instance.getAttribIdentifiers());
        //_tagsInDatabaseList.Insert(0,"<NEW>");
        }

    private void loadTagValuesFromDatabase()
        {
        _templateFilterList=new List<string>(Database.Instance.getTagValues(_templateFilterArray[_selectedTemplate]));

        }
    }
