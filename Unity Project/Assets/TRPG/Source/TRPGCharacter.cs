using System;
using System.Collections;
using System.Collections.Generic;
using IsoUnity;
using IsoUnity.Entities;
using UnityEngine;

public class TRPGCharacter : EntityScript {

    public bool playable = true;
    public string name;
    private bool finishedTurn = false;


    private double maxHP;
    private double HP;
    private double MP;
    private double speed;
    private double armor;
    private double strenght;
    
    CharacterManager cm;
    private Dictionary<string,double> modifiedAttributes;

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
    void Start ()
        {
        ///////////////////////////////////////////////////////////////////
        // UN EJEMPLO DE USO PARA OBTENER LA INFORMACION DE UN PERSONAJE //
        ///////////////////////////////////////////////////////////////////
        // Accedo a la base de datos de personajes
        cm=CharacterManager.Instance;
        // Busco el personaje por nombre (habria que poner 'name' en lugar de '"Aragorn"')
        CharacterSheet player=cm.getCharacter(name);
        // Se buscan todas las formulas que necesita el personaje para actualizar sus atributos
        player.collectFormulas();
        // Teniendo ya las formulas, se recalculan los atributos y se guardan en RealAttributes
        player.updateSheet(null);
        // Se obtienen los atributos deseados
        maxHP=player.RealAttributes["HPS"];
        MP=player.RealAttributes["MPS"];
        speed=player.RealAttributes["CON"];
        armor=player.RealAttributes["def"];
        strenght=player.RealAttributes["STR"];
        // Ahora se supone que estamos jugando.
        // Las formulas del personaje no cambian porque no le hemos equipado nada nuevo ni le ha afectado una pasiva
        // ya que eso no lo hemos programado. Por tanto nos olvidamos la parte de regenerar las formulas.
        // Sin embargo podemos hacer ataques causando dano y gasta mana usando habilidades. Imaginemos que 
        // el personaje ha recibido un ataque y le han quitado puntos de vida. Ademas el ha realizado una magia 
        // y ha gastado 10 puntos de mana
        double hpDamage=-30;
        double mpUsed=-10;
        // Actualizamos la ficha del personaje con los valores que afectan a sus atributos directamente (no a traves de formulas)
        modifiedAttributes=new Dictionary<string,double>();
        modifiedAttributes.Add("HPS",hpDamage); 
        modifiedAttributes.Add("MPS",mpUsed);
        player.updateSheet(modifiedAttributes);
        // Podemos consultar en RealAttributes el cambio que se ha producido
        double maxHPMod=(double)player.RealAttributes["HPS"];
        int MPMod=(int)player.RealAttributes["MPS"];
        }

    //deals damage to this character
    public void receibeDamage(Skill skill)
    {
        Dictionary<string, AttributeTRPG> diccionario = new Dictionary<string, AttributeTRPG>();


        if (skill.getTypeOfDamage() == 0)
        {
            this.HP = this.HP - skill.getDamage();
            if (HP > this.maxHP) HP = maxHP;
            Debug.Log("Actual health points: "  + ". Healing receibed: " + skill.getDamage());
            
        } else if(skill.getTypeOfDamage() == 1)
        {
            this.HP = this.HP - (skill.getDamage() - (0.01 * this.armor));
             Debug.Log("Actual health points: " + HP + ". Damage receibed: " + (skill.getDamage() - (0.05 * this.armor)));
        } else if(skill.getTypeOfDamage() == 2){

        }
    }

    //calculate if the character is dead or alive
    public bool isDead()
    {
        if (this.HP <= 0) return true;
        else return false;
    }

    public double getSpeed()
    {
        return this.speed;
    }

    public double getStrenght()
    {
        return this.strenght;
    }
}
