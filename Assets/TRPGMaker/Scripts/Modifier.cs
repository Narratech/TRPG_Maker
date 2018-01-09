
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

/**
 * @author PerezPrieto
 */
 [Serializable]
public class Modifier: ScriptableObject {

    public string Name = "Enter item name";
    public string Description = "Enter item description";

    [Serializable]
    public class SlotsOcupped
    {
        public List<SlotType> slotsOcupped;
    }

    public List<SlotsOcupped> SlotType = new List<SlotsOcupped>(1);

    [CustomEditor(typeof(Item))]
    public class ItemInfo : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EditorGUILayout.HelpBox("Each element of the slot defines the types of slots where the item could be placed, multiple 'Slots Occuped' in the same position are used for items who needs multiple slots.", MessageType.Info);
        }
    }
}