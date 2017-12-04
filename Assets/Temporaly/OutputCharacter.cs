using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OutputCharacter : MonoBehaviour {

    private List<GameObject> gameObjects;
    private Character character;

    // Use this for initialization
    void Start () {
        // Asignar items
        Dropdown itemDropdown = GameObject.Find("ItemDropdown").GetComponent<Dropdown>();

        character = GameObject.Find("Character").GetComponent<Character>();
        Inventory inventory = GameObject.Find("Character").GetComponent<Inventory>();
        itemDropdown.options.Clear();
        for (int i = 0; i < inventory.items.Count; i++)
            if(inventory.items[i] != null) itemDropdown.options.Add(new UnityEngine.UI.Dropdown.OptionData() { text = inventory.items[i].name });

        // Asignar slots
        Dropdown slotDropDown = GameObject.Find("SlotDropdown").GetComponent<Dropdown>();        

        slotDropDown.options.Clear();
        for (int i = 0; i < character.Slots.Count; i++)
            if(character.Slots[i] != null && character.Slots[i].slotType != null) slotDropDown.options.Add(new UnityEngine.UI.Dropdown.OptionData() { text = character.Slots[i].slotType.Name });

        // Creamos los textos de los slots
        gameObjects = new List<GameObject>();
        for (int i = 0; i < character.Slots.Count; i++)
        {
            gameObjects.Add(new GameObject());
            gameObjects[i].transform.parent = GameObject.Find("Canvas").transform;
            Text m_nameText = gameObjects[i].AddComponent<Text>();
            m_nameText.fontSize = 24;
            m_nameText.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            m_nameText.transform.localPosition = new Vector3(-200, 200 - (i*60), 0);
            
            m_nameText.rectTransform.sizeDelta = new Vector2(500, 100);
           
            string texto = "Slot " + i + ": ";
            if (character.Slots[i] != null && character.Slots[i].slotType != null) texto += character.Slots[i].slotType.Name + " - ";
            else texto += "Vacio - ";
            if (character.Slots[i] != null && character.Slots[i].item != null) texto += character.Slots[i].item.Name;
            else texto += "Vacio";
            m_nameText.text = texto;
        }
    }
	
	// Update is called once per frame
	void Update () {
        for (int i = 0; i < character.Slots.Count; i++)
        {
			gameObjects [i].transform.SetParent (GameObject.Find ("Canvas").transform);
            Text m_nameText = gameObjects[i].GetComponent<Text>();
            string texto = "Slot " + i + ": ";
            if (character.Slots[i] != null && character.Slots[i].slotType != null) texto += character.Slots[i].slotType.Name + " - ";
            else texto += "Vacio - ";
            if (character.Slots[i] != null && character.Slots[i].item != null) texto += character.Slots[i].item.Name;
            else texto += "Vacio";
            m_nameText.text = texto;
        }
    }
}
