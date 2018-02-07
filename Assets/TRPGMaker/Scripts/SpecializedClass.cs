using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "SpecializedClass", menuName = "RPG/SpecializedClass", order = 3)]
public class SpecializedClass : ScriptableObject {

    public string className;
    //private List<Tags> tags;
    public List<Slot> slots;

    

}
