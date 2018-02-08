using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Database : SingletonScriptableObject<Database>
{
    public List<Attribute> attributes;
}
