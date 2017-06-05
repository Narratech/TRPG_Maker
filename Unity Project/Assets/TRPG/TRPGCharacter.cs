using System;
using System.Collections;
using System.Collections.Generic;
using IsoUnity;
using IsoUnity.Entities;
using UnityEngine;

public class TRPGCharacter : EntityScript {


    public bool playable = true;
    private bool finishedTurn = false;
    public int hp = new int();
    public int deffense = new int();
    

    //has finished the turn?
    public bool turnFinished()
    {
        return finishedTurn;
    }

    //finish the turn
    public void finishTurn(bool finished)
    {
        this.finishedTurn = finished;
    }

    public override void eventHappened(IGameEvent ge)
    {
    }

    public override Option[] getOptions()
    {
        return null;
    }


    public override void tick()
    {
    }

    public override void Update()
    {
    }

    // Use this for initialization
    void Start () {
		
	}

    //deals damage to this character
    public void receibeDamage(Skill skill)
    {
        CharacterManager cm = CharacterManager.Instance;// Obtiene el gestor de 'CharaceterSheet' que es un singleton (solo hay uno)
        List<CharacterSheet> charList = cm.Everyone;  // Obtiene la lista de 'CharacterSheet'
        // -Buscar el o las fichas de personaje que intervienen el la accion que estes haciendo. Las fichas se
        // buscan por su nameId
        // -Hacer la accion que sea (ataque, defensa, habilidad...)
        // -Guardar nuevos valores del atributo en un diccionario estilo Dictionary<string,int> y hacer update
        // en las fichas que sean necesarias
        //charList[1].updateSheet(diccionario_con_atributos_modificados)
        cm.Everyone = charList;



        if (skill.getTypeOfDamage() == 0)
        {
            this.hp = this.hp + skill.getDamage();
            Debug.Log("Actual health points: " + hp + ". Healing receibed: " + skill.getDamage());
        } else if(skill.getTypeOfDamage() == 1)
        {
            this.hp = this.hp - skill.getDamage();
            Debug.Log("Actual health points: " + hp + ". Damage receibed: " + skill.getDamage());
        } else if(skill.getTypeOfDamage() == 2){

        }
    }

    //calculate if the character is dead or alive
    public bool isDead()
    {
        if (hp <= 0) return true;
        else return false;
    }
}
