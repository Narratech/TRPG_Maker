
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/**
 * @author PerezPrieto
 */
[RequireComponent(typeof(Inventory))]
public class Character: MonoBehaviour{

    public Inventory inventory
    {
        get
        {
            return this.GetComponent<Inventory>();
        }
    }

    public Slot[] Slots;

}