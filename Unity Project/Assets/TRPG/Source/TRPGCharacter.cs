using System;
using System.Collections;
using System.Collections.Generic;
using IsoUnity;
using IsoUnity.Entities;
using UnityEngine;

public class TRPGCharacter : EventedEntityScript {

    public CharacterSheet sheet;

    public bool playable = true;

    private bool finishedTurn = false;

    [SerializeField]
    private UnityEngine.UI.Slider lifeBar;

    private CharacterSheet player;
    private double maxHP;
    private double HP;
    private double MP;
    private double speed;
    private double armor;
    private double strength;
    
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

    public override Option[] getOptions()
    {
        return null;
    }

    // Use this for initialization
    void Start ()
        {
        /*
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

    */
        updateCharacter();
        this.HP = player.RealAttributes["HPS"];
    }


    public bool lessMP(Skill skill)
    {
        if (this.MP < skill.getManaCost()) return false;
        else {
            this.MP = this.MP - skill.getManaCost();
            return true;
        }
    }

    [GameEvent]
    public IEnumerator CastSkill(Skill skill)
    {
        var previousTarget = CameraManager.Instance.Target;
        StartCoroutine(Game.FindObjectOfType<CameraManager>().LookTo(this.gameObject, false));

        yield return new WaitForSeconds(0.5f);

        if (skill.castingAnimation)
        {
            var animation = new GameEvent("show decoration animation", new Dictionary<string, object>()
            {
                { "objective", this.gameObject },
                { "animation", skill.castingAnimation },
                { "synchronous", true }
            });

            Game.main.enqueueEvent(animation);

            yield return new WaitForEventFinished(animation);

        }

        yield return new WaitForSeconds(0.5f);

        StartCoroutine(Game.FindObjectOfType<CameraManager>().LookTo(previousTarget, false));
    }

    [GameEvent]
    public IEnumerator ReceiveSkill(Skill skill)
    {
        var previousTarget = CameraManager.Instance.Target;
        StartCoroutine(Game.FindObjectOfType<CameraManager>().LookTo(this.gameObject, false));

        yield return new WaitForSeconds(0.5f);

        if (skill.receiveAnimation)
        {
            var animation = new GameEvent("show decoration animation", new Dictionary<string, object>()
            {
                { "objective", this.gameObject },
                { "animation", skill.receiveAnimation },
                { "synchronous", true }
            });

            Game.main.enqueueEvent(animation);

            yield return new WaitForEventFinished(animation);
        }

        yield return new WaitForSeconds(0.5f);

        StartCoroutine(Game.FindObjectOfType<CameraManager>().LookTo(previousTarget, false));

    }

    //deals damage to this character
    public void receiveDamage(Skill skill)
    {
        Dictionary<string, AttributeTRPG> diccionario = new Dictionary<string, AttributeTRPG>();

        updateCharacter();

        
        if (skill.getTypeOfDamage() == 0)
        {
            if(this.HP <= maxHP)
            {
                modifiedAttributes = new Dictionary<string, double>();
                //modifiedAttributes.Add("HPS", +skill.getDamage());

                // Send back the mana consumed in this action
                modifiedAttributes.Add("MPS", -skill.getManaCost());
                player.updateSheet(modifiedAttributes);
                this.HP += skill.getDamage();
                if(this.HP >= maxHP)
                {
                    this.HP = maxHP;
                }
            }
           

            Debug.Log("Actual health points: " + HP + ". Actual Mana Points: " + MP + ".");
            
        } else if(skill.getTypeOfDamage() == 1)
        {
            modifiedAttributes = new Dictionary<string, double>();
            //modifiedAttributes.Add("HPS", -skill.getDamage());
            modifiedAttributes.Add("MPS", -skill.getManaCost());
            player.updateSheet(modifiedAttributes);
            this.HP -= (skill.getDamage() - (armor / 10f));
            Debug.Log("Actual health points: " + HP + ". Actual Mana Points: " + MP + ".");
        } else if(skill.getTypeOfDamage() == 2){

        }

        lifeBar.value = (float) (HP / maxHP);
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

    public double getStrength()
    {
        return this.strength;
    }


    public double getHP()
    {
        return this.HP;
    }

    public string getName()
    {
        return this.name;
    }

    public void updateCharacter()
    {
        cm = CharacterManager.Instance;
        // Busco el personaje por nombre (habria que poner 'name' en lugar de '"Aragorn"')
        this.player = sheet;
        // Se buscan todas las formulas que necesita el personaje para actualizar sus atributos
        player.collectFormulas();
        // Teniendo ya las formulas, se recalculan los atributos y se guardan en RealAttributes
        player.updateSheet(null);
        // Se obtienen los atributos deseados
        maxHP = player.BaseAttributes["HPS"];
        MP = player.BaseAttributes["MPS"];
        speed = player.BaseAttributes["CON"];
        armor = player.RealAttributes["def"];
        strength = player.RealAttributes["STR"];

    }

    public override void Update()
    {
    }
}
