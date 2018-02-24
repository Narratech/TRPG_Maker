using UnityEngine;
using UnityEditor;
using System.Collections;

class ItemWindow : LayoutWindow
{
    public Rect rect;

    public override void OnGUI()
    {
        Editor editor = Editor.CreateEditor((Item)ScriptableObject.CreateInstance(typeof(Item)));
        editor.OnInspectorGUI();
    }
}