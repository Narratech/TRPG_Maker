using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OutputCharacter : MonoBehaviour {

    private List<GameObject> gameObjects;
    private Character character;
    private Inventory inventory;

    // Use this for initialization
    void Start () {
        // Asignar items
        Dropdown itemDropdown = GameObject.Find("ItemDropdown").GetComponent<Dropdown>();

        character = GameObject.Find("Character").GetComponent<Character>();
        inventory = GameObject.Find("Character").GetComponent<Inventory>();
        itemDropdown.options.Clear();
        for (int i = 0; i < inventory.items.Count; i++)
            if(inventory.items[i] != null) itemDropdown.options.Add(new UnityEngine.UI.Dropdown.OptionData() { text = inventory.items[i].name });
        itemDropdown.RefreshShownValue();

        // Escuchando al boton del canvas
        UnityEngine.UI.Button btn = GameObject.Find("SetButton").GetComponent<UnityEngine.UI.Button>();
		btn.onClick.AddListener(TaskOnClick);

        // Asignar slots
		Dropdown slotDropDown = GameObject.Find("SlotDropdown").GetComponent<Dropdown>();        

        slotDropDown.options.Clear();

		for (int i = 0; i < character.Slots.Count; i++)
			if (character.Slots [i] != null && character.Slots [i].slotType != null)
				slotDropDown.options.Add (new UnityEngine.UI.Dropdown.OptionData () { text = character.Slots[i].slotType.Name });
		
        // Creamos los textos de los slots
        gameObjects = new List<GameObject>();
        for (int i = 0; i < character.Slots.Count; i++)
        {
            gameObjects.Add(new GameObject());
            gameObjects[i].transform.parent = GameObject.Find("Canvas").transform;
            Text slotText = gameObjects[i].AddComponent<Text>();
            slotText.fontSize = 24;
            slotText.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            slotText.transform.localPosition = new Vector3(-200, 200 - (i*60), 0);
            
            slotText.rectTransform.sizeDelta = new Vector2(500, 100);
           
            string texto = "Slot " + i + ": ";
            if (character.Slots[i] != null && character.Slots[i].slotType != null) texto += character.Slots[i].slotType.Name + " - ";
            else texto += "Vacio - ";
            if (character.Slots[i] != null && character.Slots[i].modifier != null) texto += character.Slots[i].modifier.name;
            else texto += "Vacio";
            slotText.text = texto;
        }
    }

    // Update is called once per frame
    void Update() {
        // Actualizamos el inventario
        Dropdown itemDropdown = GameObject.Find("ItemDropdown").GetComponent<Dropdown>();
        itemDropdown.options.Clear();
        for (int i = 0; i < inventory.items.Count; i++)
            if (inventory.items[i] != null) itemDropdown.options.Add(new UnityEngine.UI.Dropdown.OptionData() { text = inventory.items[i].name });
        itemDropdown.RefreshShownValue();

        // Actualizamos slots
        for (int i = 0; i < character.Slots.Count; i++)
        {
			gameObjects [i].transform.SetParent (GameObject.Find ("Canvas").transform);
            Text slotText = gameObjects[i].GetComponent<Text>();
            string texto = "Slot " + i + ": ";
            if (character.Slots[i] != null && character.Slots[i].slotType != null) texto += character.Slots[i].slotType.Name + " - ";
            else texto += "Vacio - ";
            if (character.Slots[i] != null && character.Slots[i].modifier != null) texto += character.Slots[i].modifier.name;
            else texto += "Vacio";
            slotText.text = texto;
        }

    }

	void TaskOnClick(){
		// Cogemos el nombre del item seleccionado
        Dropdown itemDropDown = GameObject.Find("ItemDropdown").GetComponent<Dropdown>();
        int itemIndex = itemDropDown.GetComponent<Dropdown>().value;

        // Cogemos los tipos de slot seleccionados
        Dropdown slotDropDown = GameObject.Find("SlotDropdown").GetComponent<Dropdown>();
        int slotIndex = slotDropDown.GetComponent<Dropdown>().value;

        if (character.Slots[slotIndex].canEquip(inventory.items[itemIndex])) {
            character.Slots[slotIndex].setModifier(inventory.items[itemIndex]);
            itemDropDown.value = 0;
            slotDropDown.value = 0;
        }
        else
            Debug.Log("Can't equip!");       
	}
}
