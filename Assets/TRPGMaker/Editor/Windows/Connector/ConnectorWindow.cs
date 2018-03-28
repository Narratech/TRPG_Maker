using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ConnectorWindow : EditorWindow{

    private int selectedIndex = -1;
    private List<Type> connectors;

    [MenuItem("TRPGMaker/Connector options", false, 1)]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(ConnectorWindow), false, "Connector options");
    }

    private void OnEnable()
    {
        // Search all classes that implement ITRPGMapConnector
        var type = typeof(ITRPGMapConnector);
        var types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => type.IsAssignableFrom(p) && !p.IsInterface);

        connectors = new List<Type>();
        foreach (var c in types)
        {
            connectors.Add(c);
        }

        // Search if connector exist and set selected index
        // If new connector is implemented this needs to be implmented in preference order
        UnityEngine.Object[] objects = Resources.FindObjectsOfTypeAll(typeof(IsoUnityOptions));
        if (objects.Length >= 1)
        {
            selectedIndex = connectors.IndexOf(connectors.FirstOrDefault(c => c.Name == "IsoUnityConnector"));
        }
    }

    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Select connector:");
        selectedIndex = EditorGUILayout.Popup(selectedIndex, connectors.Select(c => c.Name).ToArray());
        EditorGUILayout.EndHorizontal();

        // Case IsoUnityConnector
        if (selectedIndex == connectors.IndexOf(connectors.FirstOrDefault(c => c.Name == "IsoUnityConnector"))){
            UnityEngine.Object[] objects = Resources.FindObjectsOfTypeAll(typeof(IsoUnityOptions));
            if (objects.Length < 1)
            {
                GameObject isounityOptions = new GameObject("IsoUnityOptions");
                isounityOptions.AddComponent<IsoUnityOptions>();
                Debug.Log("IsoUnityConnector is not asigned to any 'IsoUnityOptions' GameObject, automatically created!");
                Repaint();
            }
            else if (objects.Length > 1)
            {
                Debug.Log("Multiple IsoUnityOptions scripts in scene! Assing just one to an 'IsoUnityOptions' GameObject or remove all an will be automatically created.");
            }
            else if (objects[0].name != "IsoUnityOptions")
            {
                Debug.Log("IsoUnityOptions needs to be assigned in an 'IsoUnityOptions' GameObject, rename GameObject or remove Component and it will be automatically created.");
            }
            else {
                Editor editor = Editor.CreateEditor(objects[0]);
                editor.DrawDefaultInspector();
            }
        }
        
        // Other cases:
        // If a new connector is created needs an editor to be implemented here
    }

}
