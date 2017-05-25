using System.Collections;
using System.Collections.Generic;
using IsoUnity;
using UnityEngine;
using IsoUnity.Entities;

public class Turn: EventedEventManager
{

    private int numPlayableCharacters = 0;
    private TRPGCharacter[] characters;
    private SkillsDB skillsDataBase = new SkillsDB();

    public override void ReceiveEvent(IsoUnity.IGameEvent ge)
    {
        base.ReceiveEvent(ge);
        
    }

    public override void Tick()
    {
        base.Tick();
    }

    // Use this for initialization
    IEnumerator Start()
    {

        
        while (true)
        {
            yield return new WaitForSeconds(1);

            //todo dentro de un bucle, hasta que todos los enemigos esten derrotados, sigue dando vueltas
            characters = IsoUnity.Map.FindObjectsOfType<TRPGCharacter>();
            numPlayableCharacters = characters.Length;

            //Ordenar la lista de characters segun su "velocidad" o agilidad...

            //Turn of every playable character in game

            for (int i = 0; i < numPlayableCharacters; i++)
            {
                bool moved = false;
                bool attacked = false;
                bool finished = false;
                if (characters[i].playable)
                {
                    while(!moved && !attacked && !finished)
                    {
                        //if the character hasnt moved yet, he can do it, only one time.
                        if (!moved)
                        {
                            //colocamos la flecha en la posicion del personaje
                            IGameEvent evento = new GameEvent();
                            evento.Name = "select cell";
                            evento.setParameter("cell", characters[i].Entity.Position);

                            //recalcular la distancia de movimiento según la "agilidad" del personaje, 2 por defecto
                            int movement = 2;
                            Skill habilidad = new Skill("Mover", "", "", "", "", 2, 0, movement, null, 0);
                            evento.setParameter("skill", habilidad);
                            evento.setParameter("synchronous", true);
                            Game.main.enqueueEvent(evento);
                            IGameEvent eventFinished;
                            yield return new WaitForEventFinished(evento, out eventFinished);
                            Debug.Log("Selected received");

                            var selectedCell = eventFinished.getParameter("cellSelected") as Cell;

                            //primero marcamos el move 
                            evento = new GameEvent("move", new Dictionary<string, object>() {
                                 {"mover", characters[i].Entity.mover },
                                 {"cell", selectedCell },
                                 {"synchronous", true }
                            });

                            Game.main.enqueueEvent(evento);

                            yield return new WaitForEventFinished(evento);

                            moved = true;
                        }

                        //if the character hasnt attacked yet, he can do it (attacking or using a skill), only one time.
                        if (!attacked)
                        {
                            //mostrar lista de habilidades disponibles para las caracteristicas del personaje
                            Skill[] skills = skillsDataBase.getSavedSkills();
                            for(int j = 0; j < skills.Length; j++)
                            {

                            }
                            //Seleccionar una
                            Skill selectedSkill = skills[1];

                            //colocamos la flecha en la posicion del personaje y lanzamos el evento de seleccionar objetivo
                            //OJO QUE AHORA MISMO SI SE SELECCIONA UNA CELDA CON UN ENTITY DENTRO DA ERROR PORQUE ESTA USANDO EL MOVER (creo)
                            IGameEvent evento = new GameEvent();
                            evento.Name = "select cell";
                            evento.setParameter("cell", characters[i].Entity.Position);
                            evento.setParameter("skill", selectedSkill);
                            evento.setParameter("synchronous", true);
                            Game.main.enqueueEvent(evento);
                            IGameEvent eventFinished;
                            yield return new WaitForEventFinished(evento, out eventFinished);
                            Debug.Log("Selected received");

                            var selectedCell = eventFinished.getParameter("cellSelected") as Cell;

                            //Realizar animaciones

                            //terminar ronda de ataque, quitar puntos de daño, pm...
                            attacked = true;
                        }


                        //The character can finish his turn without attacking or moving 
                        if (!finished)
                        {
                            //set direction of the character
                            moved = true;
                            attacked = true;
                            finished = true;
                        }


                    }
                    

                    //segundo definimos habilidad

                    //terminamos turno del personaje

                }
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
