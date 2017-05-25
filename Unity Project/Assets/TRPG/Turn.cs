using System.Collections;
using System.Collections.Generic;
using IsoUnity;
using UnityEngine;
using IsoUnity.Entities;

public class Turn: EventedEventManager
{

    private int numPlayableCharacters = 0;
    private TRPGCharacter[] characters;
    private Cell[] celdas;

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
                if (characters[i].playable)
                {
                    //colocamos la flecha en la posicion del personaje
                    IGameEvent evento = new GameEvent();
                    evento.Name = "select cell";
                    evento.setParameter("cell", characters[i].Entity.Position);
                    Skill habilidad = new Skill("bola de fuego", "", "", "", "", 2, 2200, 3, null, 0);
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

                    Debug.Log("Movement done");

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
