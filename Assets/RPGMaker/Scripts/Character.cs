
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

/**
 * @author PerezPrieto
 */
[RequireComponent(typeof(Inventory))]
[Serializable]
public class Character: MonoBehaviour{

    public Inventory inventory
    {
        get
        {
            return this.GetComponent<Inventory>();
        }
    }

    public List<Slot> Slots;

    // Custom Editor class for checking Slots and Items
    [CustomEditor(typeof(Character))]
    public class MyScriptEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // Show dinamic array panel
            serializedObject.Update();
            var myScript = target as Character;
            SerializedProperty tps = serializedObject.FindProperty("Slots");
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(tps, true);
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();

            // Check if item correspond with slot
            for (int i = 0; i < myScript.Slots.Count; i++) {
                Boolean correct = true;
                // Si el slot actual tiene item
                if (myScript.Slots[i].item != null)
                {
                    // Para cada tipo de slot del item
                    for(int j = 0; j < myScript.Slots[i].item.SlotType.Length; j++)
                    {
                        // Si el slot es distinto del actual
                        if(myScript.Slots[i].item.SlotType[j].GetInstanceID() != myScript.Slots[i].slotType.GetInstanceID())
                        {
                            // Buscamos el slot correspondiente
                            int pos = myScript.Slots.FindIndex(
                            delegate (Slot slot)
                            {
                                return slot.slotType == myScript.Slots[i].item.SlotType[j];
                            });
                            // Si tiene un item, error
                            if (myScript.Slots[pos].item != null)
                            {
                                correct = false;                             
                            }       
                        } else // Si es el slot actual
                        {
                            // Si no es del mismo tipo, error
                            if (myScript.Slots[i].item.SlotType[j].GetInstanceID() != myScript.Slots[i].slotType.GetInstanceID())
                            {
                                correct = false;
                            }
                        }
                    }
                }
                if (!correct) {
                    EditorUtility.DisplayDialog("Error",
                        "The item does not correspond with this slot", "Accept");
                    myScript.Slots[i].item = null;
                }
            }
        }
    }
}