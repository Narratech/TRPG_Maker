
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/**
 * @author PerezPrieto
 */
 [Serializable]
public class Modifier: ScriptableObject {
    public new string name = "Enter item name";
    public string description = "Enter item description";
    public List<string> tags = new List<String>();
    public Formula formula;
    public Texture2D icon;

    [Serializable]
    public class SlotsOcupped
    {
        public SlotsOcupped()
        {
            slotsOcupped = new List<String>();
        }

        public List<string> slotsOcupped;
    }

    public List<SlotsOcupped> SlotType = new List<SlotsOcupped>(1);
}