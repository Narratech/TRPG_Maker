using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Slot{

    public string slotType;
    public Modifier _modifier;

    public Modifier modifier
    {
        get
        {
            return _modifier;
        }

        set
        {
            //#3
            _modifier = value;
            calculatedFormula = false;
        }
    }
    public bool calculatedFormula;

    public void setModifier(Modifier modifier)    {

        Character character = GameObject.Find("Character").GetComponent<Character>();
        Inventory inventory = character.inventory;
        List<int> indexes = new List<int>();
        int index = -1;
        // Buscamos el slot actual en el item
        for (int i = 0; i < modifier.SlotType.Count; i++)
        {
            int pos = modifier.SlotType[i].slotsOcupped.FindIndex(
                        delegate (string slotType)
                        {
                            return slotType == this.slotType;
                        });
            if (pos != -1)
                index = i;
        }

        // Guardamos las posiciones de todos los slots donde lo vamos a asignar
        for (int j = 0; j < modifier.SlotType[index].slotsOcupped.Count; j++)
        {
            int pos = character.Slots.FindIndex(
                    delegate (Slot slot) {
                        return slot.slotType == modifier.SlotType[index].slotsOcupped[j];
                    });
            if (pos == -1)
            {
                Debug.Log("Error: " +
                            "The character doesn't have the slot type " + modifier.SlotType[index].slotsOcupped[j] + ". Execute Slot.canEquip() before executing Slot.setModifier()");
            }
            else
                indexes.Add(pos);
        }

        // Quitamos los items que hubiese en esas posiciones y asignamos el nuevo item
        for(int i = 0; i < indexes.Count; i++)
        {
            // Si habia algo, lo quitamos y lo guardamos en el inventario
            if (character.Slots[indexes[i]].modifier != null)
            {
                Item itemAux = (Item) character.Slots[indexes[i]].modifier;
                // Lo quitamos de todos sus slots
                for (int j = 0; j < itemAux.SlotType.Count; j++)
                {
                    for (int h = 0; h < itemAux.SlotType[j].slotsOcupped.Count; h++)
                    {
                        // Buscamos el slot
                        int pos = character.Slots.FindIndex(
                            delegate (Slot slot)
                            {
                                return slot.slotType == itemAux.SlotType[j].slotsOcupped[h];
                            });
                        // Si existe lo quitamos
                        if (pos != -1 && character.Slots[pos].modifier != null && character.Slots[pos].modifier == itemAux) character.Slots[pos].modifier = null;
                    }
                }
                // Lo guardamos en el inventario.
                inventory.addItem(itemAux);
            }
            // Asignamos el item
            character.Slots[indexes[i]].modifier = modifier;
            // Lo quitamos del inventario
            inventory.removeItem((Item) modifier);
        }
    }

    public void removeItem()
    {

    }

    public Boolean canEquip(Modifier modifier)
    {
        Character character = GameObject.Find("Character").GetComponent<Character>();
        Boolean slotExist = true;
        List<int> indexes = new List<int>();

        // Buscamos el slot actual en el item
        for (int i = 0; i < modifier.SlotType.Count; i++)
        {
            int pos = modifier.SlotType[i].slotsOcupped.FindIndex(
                        delegate (string slotType)
                        {
                            return slotType == this.slotType;
                        });
            if (pos != -1)
                indexes.Add(i);
        }

        // Comprobamos si en alguno de las combinaciones del item donde está este tipo de slot puede
        // equiparse el item. Ejemplo: si el slot actual es "mano derecha" y se puede asignar 
        // en "mano derecha y pie derecho" o en "mano derecha y pie izquierdo" comprobamos que 
        // exista al menos uno de los pies.

        // Si no habia ninguna posicion posible, error
        if (indexes.Count == 0)
            slotExist = false;

        for(int i = 0; i < indexes.Count; i++)
        {
            for (int j = 0; j < modifier.SlotType[indexes[i]].slotsOcupped.Count; j++)
            {
                // Buscamos la posicion del slot en character
                int pos = character.Slots.FindIndex(
                    delegate (Slot slot) {
                        return slot.slotType == modifier.SlotType[indexes[i]].slotsOcupped[j];
                    });
                // Si no existe, error
                if (pos == -1)
                {
                    slotExist = false;
                }
            }
        }

        return slotExist;
    }

}
