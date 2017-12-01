using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SlotType", menuName = "RPG/Slot Type", order = 2)]
public class SlotType : ScriptableObject {

    public string Name = "Enter slot type";
    public int NumberItems = 1;
}
