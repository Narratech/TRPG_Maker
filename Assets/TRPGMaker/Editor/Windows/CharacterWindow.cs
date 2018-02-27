using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class CharacterWindow : LayoutWindow
{
    public override void Init()
    {
        
    }

    public override void Draw(Rect rect)
    {
        
    }

    public override bool Button(Rect rect)
    {
        var characterTexture = (Texture2D)Resources.Load("Menu/Buttons/characters", typeof(Texture2D));
        GUIStyle myStyle = new GUIStyle("Button");
        myStyle.padding = new RectOffset(0, 0, 10, 10);

        if (GUILayout.Button(new GUIContent("Characters", characterTexture), myStyle))
        {
            Draw(rect);
            selected = true;
        }

        return selected;
    }
}
