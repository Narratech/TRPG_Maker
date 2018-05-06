
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
    public List<Tag> tags = new List<Tag>();
    [SerializeField]
    private List<Formula> _formulas = new List<Formula>();
    public List<Formula> formulas
    {
        get
        {
            return _formulas;
        }
        set
        {
            _formulas = value;
        }
    }


    public Texture2D icon;

    [Serializable]
    public class SlotsOcupped
    {
        public SlotsOcupped()
        {
            slotsOcupped = new List<SlotType>();
        }

        public List<SlotType> slotsOcupped;
    }

    public List<SlotsOcupped> SlotType = new List<SlotsOcupped>(1);
}