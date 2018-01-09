
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

/**
 * @author PerezPrieto
 */
[RequireComponent(typeof(Inventory))]
[Serializable]
public class Character: MonoBehaviour{

    public Inventory inventory
    {
        get
        {
            return this.GetComponent<Inventory>();
        }
    }

    public List<Slot> Slots;    
}