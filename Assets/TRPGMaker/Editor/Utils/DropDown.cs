using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class DropDown
{

    private const float labelSize = 150;

    public string Value;
    public string Label;
    public List<string> Elements;

    private bool showDropdown = false;
    private bool eventUsed = false;
    private EventType lastEvent;
    private Vector2 scrollPos;
    private Rect scrollRect;


    private GUISkin dropdownskin;

    public DropDown(string label)
    {
        Label = label;
        dropdownskin = Resources.Load<GUISkin>("DropdownSkin");

    }

    public string LayoutBegin()
    {
        // Creating Layout element
        GUI.SetNextControlName(Label);
        Value = EditorGUILayout.TextField(Label, Value);

        // Calculating dropdown scroll rect
        var addressRect = GUILayoutUtility.GetLastRect();
        addressRect = new Rect(addressRect.x + labelSize, addressRect.y, addressRect.width - labelSize, addressRect.height);
        scrollRect = new Rect(addressRect.x, addressRect.y + addressRect.height, addressRect.width, 100);

        // If focused show
        showDropdown = false;
        if (GUI.GetNameOfFocusedControl() == Label)
        {
            if (Elements != null && Elements.Count > 0)
            {
                showDropdown = true;
            }
        }

        // Event management
        lastEvent = Event.current.type;
        if (showDropdown && scrollRect.Contains(Event.current.mousePosition)
            && Event.current.type != EventType.Repaint
            && Event.current.type != EventType.Layout)
        {
            eventUsed = true;
            Event.current.Use();
        }

        return Value;
    }

    public bool LayoutEnd()
    {
        var r = false;
        if (showDropdown)
        {
            if (eventUsed) Event.current.type = lastEvent;

            GUI.skin = dropdownskin;
            // Show Scrollview
            scrollPos = GUI.BeginScrollView(scrollRect, scrollPos, new Rect(0, 0, scrollRect.width - 15, 15 * Elements.Count), false, false);
            for (int i = 0; i < Elements.Count; i++)
            {
                var buttonRect = new Rect(0, 15 * i, scrollRect.width, 15);
                if (GUI.Button(buttonRect, Elements[i]))
                {
                    if (Value != null && Value.LastIndexOf(',') != -1)
                    {
                        Value = Value.Substring(0, Value.LastIndexOf(',') + 1);
                        Value += Elements[i];
                    }
                    else
                        Value = Elements[i];
                    this.Elements = null;
                    r = true;
                    GUI.FocusControl(null);                    
                    //GUI.FocusControl("Tags:");
                    break;
                }
            }
            GUI.EndScrollView();

            GUI.skin = null;

            //Repaint();
        }

        return r;
    }
}