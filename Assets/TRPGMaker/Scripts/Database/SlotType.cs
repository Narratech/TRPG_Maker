using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SlotType {

    public string slotName = "Enter slot type";

    public SlotType(string name)
    {
        this.slotName = name;
    }
}
