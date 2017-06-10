using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SkillRequirement{

    private string requirement;
    private int typeOfRequirement;

    public SkillRequirement(int typeOfRequirement, string requirement)
    {
        this.typeOfRequirement = typeOfRequirement;
        this.requirement = requirement;
    }


    public string getRequirement()
    {
        return this.requirement;
    }

    public int getTypeRQ()
    {
        return this.typeOfRequirement;
    }


    public int changeTypeRQ(int number)
    {
        return this.typeOfRequirement = number;
    }


    public string changeDescRQ(string descp)
    {
        return this.requirement = descp;
    }
}

