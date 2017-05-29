using System.Collections;
using System.Collections.Generic;
using IsoUnity;
using UnityEngine;
using IsoUnity.Entities;

public class Turn: EventedEventManager
{
    
    private TRPGCharacter[] characters;
    private SkillsDB skillsDataBase = new SkillsDB();
    public Transform canvas;


    private bool moved = false;
    private bool attacked = false;
    private bool deffended = false;
    private TurnState state = TurnState.Idle;


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

        //todo dentro de un bucle, hasta que todos los enemigos esten derrotados, sigue dando vueltas
        characters = IsoUnity.Map.FindObjectsOfType<TRPGCharacter>();
        

        while (true)
        {
            yield return new WaitForSeconds(1);

            //Ordenar la lista de characters segun su "velocidad" o agilidad...
            turnOrder();
            //Turn of every playable character in game


            if (turnFinished()) restartTurn();

            TRPGCharacter character = selectNextCharacter();


            if(character != null)
            {
                    yield return new WaitWhile(() => this.state == TurnState.Idle);
                    switch (this.state)
                    {
                       /* case "elquesea":
                            ---lo que sea
                    ChangeState(TurnState.Finalize);
                    */
                    }
                





                bool moved = false;
                bool attacked = false;
                bool finished = false;
                if (character.playable)
                {
                    while(!moved && !attacked && !finished)
                    {
                        if (!canvas.gameObject.activeInHierarchy)
                        {
                            //activa y desactiva los 4 botones de acciones al empezar el turno
                            canvas.gameObject.SetActive(true);
                        }
                        //if the character hasnt moved yet, he can do it, only one time.
                        //SI HA PULSADO MOVE, ENTRAMOS A ESTE IF
                        if (!moved)
                        {



                            //colocamos la flecha en la posicion del personaje
                            IGameEvent evento = new GameEvent();
                            evento.Name = "select cell";
                            evento.setParameter("cell", character.Entity.Position);

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
                                 {"mover", character.Entity.mover },
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

                          



                            canvas.gameObject.SetActive(true);
                            //mostrar lista de habilidades disponibles para las caracteristicas del personaje
                            Skill[] skills = skillsDataBase.getSavedSkills();
                            for(int j = 0; j < skills.Length; j++)
                            {

                            }
                            //Seleccionar una   
                            Skill selectedSkill = skills[2];

                            
                            GameObject newGO = new GameObject("Skill-List");
                            newGO.transform.position = new Vector3(60, 320, 0);
                            newGO.AddComponent <UnityEngine.UI.Text>();
                            newGO.GetComponent<UnityEngine.UI.Text>().color = Color.black;
                            newGO.GetComponent<UnityEngine.UI.Text>().text = skillOnString();
                            newGO.GetComponent<UnityEngine.UI.Text>().font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                            newGO.transform.SetParent(canvas);
                            
                          

                            //colocamos la flecha en la posicion del personaje y lanzamos el evento de seleccionar objetivo
                            //OJO QUE AHORA MISMO SI SE SELECCIONA UNA CELDA CON UN ENTITY DENTRO DA ERROR PORQUE ESTA USANDO EL MOVER (creo)
                            IGameEvent evento = new GameEvent();
                            evento.Name = "select cell";
                            evento.setParameter("cell", character.Entity.Position);
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
                               Destroy(newGO);
                        }


                        //The character can finish his turn without attacking or moving 
                        if (!finished)
                        {
                            //set direction of the character
                            character.finishTurn(true);
                            canvas.gameObject.SetActive(false);
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

    [GameEvent(true, false)]
    public void ChangeState(TurnState state)
    {
        if (state == TurnState.Move && !moved)
            this.state = TurnState.Move;
        else if (state == TurnState.Action && !attacked)
            this.state = TurnState.Action;
        else if (state == TurnState.Deffense && !deffended)
            this.state = TurnState.Deffense;
        else if (state == TurnState.Finalize && ((moved && attacked) || deffended))
            this.state = TurnState.Finalize;
        else this.state = TurnState.Idle;
    }

    private TRPGCharacter selectNextCharacter()
    {
        TRPGCharacter character = null;
        bool find = false;
        for(int i = 0; i < characters.Length; i++)
        {
            if (!characters[i].turnFinished() && !find)
            {
                character = characters[i];
                find = true;
            }
        }
        return character;
    }

    private void restartTurn()
    {
        foreach (TRPGCharacter charact in characters) charact.finishTurn(false);
    }

    private bool turnFinished()
    {
        bool finished = true;
        int i = 0;
        while (finished && i < characters.Length)
        {
            finished = characters[i].turnFinished();
            i++;
        }
        return finished;
    }


    //Ordenamos el array de personajes
    private void turnOrder()
    {

    }

    private string skillOnString()
    {
        string skillsNames = "";
        foreach (Skill skill in skillsDataBase.getSavedSkills()){
            skillsNames = skillsNames + skill.getName() + '\n';
        }

        return skillsNames;
    }


    // Update is called once per frame
    void Update () {
		
	}
}
