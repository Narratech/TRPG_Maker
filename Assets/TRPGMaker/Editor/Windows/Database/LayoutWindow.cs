using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class LayoutWindow : ScriptableObject{

    public Editor editor;
    public bool selected;

    public abstract void Init();
    public abstract void Draw(Rect rect);
    public abstract bool Button(Rect rect);
}
