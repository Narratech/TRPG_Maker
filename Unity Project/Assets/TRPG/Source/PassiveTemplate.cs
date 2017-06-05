using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PassiveTemplate: Template
    {

    public PassiveTemplate Init(string nameId, string description,List<string> tags, List<Formula> formulas, SlotsConfig allowedSlots)
        {
        base.Init(nameId,description,tags,formulas,allowedSlots);
        return this;
        }
	}
