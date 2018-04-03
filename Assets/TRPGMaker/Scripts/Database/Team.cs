using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Team : ScriptableObject {

    public string name;
    public int id;
	public List<Character> characters;
}
