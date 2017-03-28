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

    private SkillRequirement[] requirement;


    //Dependiendo de como vaya desarrollandose
    private Animation preparing;
    private Animation attacking;
    private Animation ending;



    //Constructora básica 
    public Skill(string name, string description, string skillType, int damage, int distance)
    {
        this.name = name;
        this.description = description;
        this.type = skillType;
        this.damage = damage;
        this.distance = distance;


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


    public string getName()
    {
        return this.name;
    }

    public string getDescription()
    {
        return this.description;
    }

    public string getTypeDamage()
    {
        return this.type;
    }

    public int getDamage()
    {
        return this.damage;
    }

    public int getDistance()
    {
        return this.distance;

    }


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
}
