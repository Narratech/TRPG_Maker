using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BancoHabilidades {
    
    private List<Skill> habilidadesDisponibles = new List<Skill>();

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    public void add(Skill skill)
    {
        Skill yaExiste = busqueda(skill);
        if(yaExiste == null)
        {
            habilidadesDisponibles.Add(skill);
        }
    }

    public void remove(Skill skill)
    {
        Skill yaExiste = busqueda(skill);
        if (yaExiste != null)
        {
            habilidadesDisponibles.Remove(skill);
        }
    }


    public Skill busqueda(Skill skill)
    {
        Skill encontrada = habilidadesDisponibles.Find(x => x.getName() == skill.getName());
        return encontrada;
    }

    public Skill getHabilidad(string name)
    {
        /*
        Skill encontrada = new Skill();
        encontrada.name = name;
        encontrada = busqueda(encontrada);
        */
        return null;
    }

    public int getSize()
    {
        return this.habilidadesDisponibles.Count;
    }

    public Skill getHabilidad(int i)
    {
        Skill skill = null;

        Skill[] skills = habilidadesDisponibles.ToArray();
        skill = skills[i];

        return skill;
    }

    public void deleteSkill(Skill deletedskill)
    {
        this.habilidadesDisponibles.Remove(deletedskill);
    }


}
