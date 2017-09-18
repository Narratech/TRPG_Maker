using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[Serializable]
//heredará de Slot?
public class Skill {

    [NonSerialized]
    public IsoUnity.IsoDecoration castingAnimation;

    [NonSerialized]
    public IsoUnity.IsoDecoration receiveAnimation;

    private string name;

    private string description;

    private double manaCost;
 
    private int distance;

    private int tiempoCasteo;
   
    private int damage;

    //Definir el tipo de habilidad, ya sea area, unitarget...
    private string type;
    //definir el tipo de casteo de habilidad
    private string cast;
    //effect
    private string effect;

    private int TypeOfDamage;


    private List<SkillRequirement> requirement = new List<SkillRequirement>();
    private int requirements = 0;



    //Constructora básica 
    public Skill(string name, string description, double manaCost, string spellType, string skillType, string skillEffect, int typeOfDamage, int damage, int distance, List<SkillRequirement> requirements, int requirementsCreated)
    {
        this.name = name;
        this.description = description;
        this.manaCost = manaCost;
        this.type = spellType;
        this.cast = skillType;
        this.effect = skillEffect;
        this.TypeOfDamage = typeOfDamage;
        this.damage = damage;
        this.distance = distance;
        this.requirement = requirements;
        this.requirements = requirementsCreated;
    }
    

    
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
       
        //version cutre para abrir una ventana al clickear en modo area de habilidad
        /*if (tipo == TipoHabilidad.area && !ventanaAbierta)
        {
            ventanaAbierta = true;
            UnityEditor.EditorWindow window = new UnityEditor.EditorWindow();
            window.Show();
            
        }*/

    }

    private void calcularDanio()
    {
        //A esta funcion solo se entra si la habilidad ha sido lanzada (el objetivo ya ha sido seleccionado y demás)
        //Calculo de propabilidad de acertar con la habilidad
        //Calculo del daño si ha golpeado
        //Calculo de muerte del objetivo o Calculo de respuesta si ha sobrevivido.
        
    }

    //Getters
    public string getName()
    {
        return this.name;
    }

    public string getDescription()
    {
        return this.description;
    }

    public int getTypeCast()
    {
        if(type.Equals("Linear Projectile"))
            return 0;
        else if (type.Equals("Parabolic Projectile"))
            return 1;
        else if (type.Equals("On Ground"))
            return 2;
        else
            return 3;
    }

    public int getCastCharacter()
    {
        if (cast.Equals("Self Character"))
            return 0;
        else if (cast.Equals("Self Character with direction"))
            return 1;
        else if (cast.Equals("Ranged place"))
            return 2;
        else if (cast.Equals("Global"))
            return 3;
        else
            return 4;
    }

    public int getSkillEffect()
    {
        if (effect.Equals("Single target"))
            return 0;
        else if (effect.Equals("Area"))
            return 1;
        else if (effect.Equals("Area in objective"))
            return 2;
        else if (effect.Equals("Global"))
            return 3;
        else
            return 4;
    }

    public int getTypeOfDamage()
    {
        return this.TypeOfDamage;
    }

    public int getDamage()
    {
        return this.damage;
    }

    public int getDistance()
    {
        return this.distance;

    }

    public int getTypeRQ(int numberRQ)
    {
        return this.requirement[numberRQ].getTypeRQ();
    }

    public string getDescRQ(int numberRQ)
    {
        return this.requirement[numberRQ].getRequirement();
    }



    //setters

    public void changeName(string name)
    {
        this.name = name;
    }

    public void changeDescription(string description)
    {
        this.description = description;
    }

    public void changeSkillDamage(int damage)
    {
        this.damage = damage;
    }

    public void changeDistance(int distance)
    {
        this.distance = distance;
    }

    public void changeTypeOfDamage(int type)
    {
        this.TypeOfDamage = type;
    }

    public void changeTypeSkill(int type)
    {
        switch (type)
        {
            case 0:
                this.type = "Linear Projectile";
                break;
            case 1:
                this.type = "Parabolic Projectile";
                break;
            case 2:
                this.type = "On Ground";
                break;
            default:
                break;

        }
    }

    public void changeCastSkill(int type)
    {
        switch (type)
        {
            case 0:
                this.cast = "Self Character";
                break;
            case 1:
                this.cast = "Self Character with direction";
                break;
            case 2:
                this.cast = "Ranged place";
                break;
            case 3:
                this.cast = "Global";
                break;
            default:
                break;

        }
    }

    public void changeSkillEffect(int type)
    {
        switch (type)
        {
            case 0:
                this.effect = "Single target";
                break;
            case 1:
                this.effect = "Area";
                break;
            case 2:
                this.effect = "Area in objective";
                break;
            case 3:
                this.effect = "Global";
                break;
            default:
                break;

        }
    }

public int numberRequirements()
    {
        return this.requirements;
    }


public int changeTypeRQ(int type, int skill)
    {
       return this.requirement[skill].changeTypeRQ(type);
    }

    
public string changeDescRQ(string description, int skill)
    {
       return this.requirement[skill].changeDescRQ(description);
    }

public void removeRQ(SkillRequirement position)
    {
        SkillRequirement remove = this.requirement.Find(x => x.getTypeRQ() == position.getTypeRQ());
        this.requirement.Remove(remove);
        this.requirements--;

    }

public double getManaCost()
    {
        return this.manaCost;
    }

}
