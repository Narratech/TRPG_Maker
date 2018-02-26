using UnityEngine;
using UnityEditor;
using System.Collections;

class SpecializedClassWindow : LayoutWindow
{
    public override void Init()
    {

    }

    public override void Draw(Rect rect)
    {
        editor = Editor.CreateEditor((SpecializedClass)ScriptableObject.CreateInstance(typeof(SpecializedClass)));
        editor.OnInspectorGUI();
    }

    public override bool Button(Rect rect)
    {
        return false;
    }
}