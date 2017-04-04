using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//heredará de Slot?
public class Skill {


    private string name;

    private string description;
 
    private int distance;

    private int tiempoCasteo;
   
    private int damage;

    //Definir el tipo de habilidad, ya sea area, unitarget...
    private string type;

    private int TypeSkill;


    private List<SkillRequirement> requirement = new List<SkillRequirement>();
    private int requirements = 0;
    
    


    //Dependiendo de como vaya desarrollandose
    private Animation preparing;
    private Animation attacking;
    private Animation ending;



    //Constructora básica 
    public Skill(string name, string description, string skillType, int damage, int distance, List<SkillRequirement> requirements, int requirementsCreated)
    {
        this.name = name;
        this.description = description;
        this.type = skillType;
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

    public int getTypeDamage()
    {
        if(type.Equals("One Target"))
            return 0;
        else if (type.Equals("All Enemies"))
            return 1;
        else if (type.Equals("All Allies"))
            return 2;
        else if (type.Equals("All Map"))
            return 3;
        else
            return 4;
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

    public void changeTypeSkill(int type)
    {
        switch (type)
        {
            case 0:
                this.type = "One Target";
                break;
            case 1:
                this.type = "All Enemies";
                break;
            case 2:
                this.type = "All Allies";
                break;
            case 3:
                this.type = "All Map";
                break;
            case 4:
                this.type = "Area";
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

public void removeRQ(int position)
    {
        this.requirement[position] = null;
    }

}
