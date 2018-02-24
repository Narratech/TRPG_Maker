using UnityEngine;
using UnityEditor;
using System.Collections;

class SpecializedClassWindow : LayoutWindow
{
    public Rect rect;

    public override void OnGUI()
    {
        Editor editor = Editor.CreateEditor((SpecializedClass)ScriptableObject.CreateInstance(typeof(SpecializedClass)));
        editor.OnInspectorGUI();
    }
}