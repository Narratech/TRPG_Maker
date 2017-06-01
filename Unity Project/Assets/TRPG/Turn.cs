using System.Collections;
using System.Collections.Generic;
using IsoUnity;
using UnityEngine;
using IsoUnity.Entities;

public class Turn: EventedEventManager
{
    public Transform canvas;

    private TRPGCharacter[] characters;
    private SkillsDB skillsDataBase = new SkillsDB();
    private TRPGCharacter character;
    private int playableCharacters = 0;

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
        
        foreach (TRPGCharacter charac in characters)
            if (charac.playable) playableCharacters++;

        while (playableCharacters > 1)
        {
            Debug.Log(playableCharacters);
            //Ordenar la lista de characters segun su "velocidad" o agilidad...
            turnOrder();
            //Turn of every playable character in game


            if (turnFinished()) restartTurn();

            this.character = selectNextCharacter();
            if (character != null){
                if (character.playable){
                    activateButtons();
                    StartCoroutine(Game.FindObjectOfType<CameraManager>().LookTo(character.gameObject, false));

                    yield return new WaitWhile(() => this.state == TurnState.Idle);
                   
                    if(this.state == TurnState.Finalize){
                        //set direction of the character
                        this.moved = false;
                        this.attacked = false;
                        this.deffended = false;
                        character.finishTurn(true);
                        canvas.gameObject.SetActive(true);
                        this.state = TurnState.Idle;
                    }
                }
            }
        }
    }



    [GameEvent(true, false)]
    public void ChangeState(TurnState state)
    {

        if (!canvas.gameObject.activeInHierarchy)
        {
            //activa y desactiva los 4 botones de acciones al empezar el turno
            canvas.gameObject.SetActive(true);
        }

        //pressed mvoe button
        if (state == TurnState.Move && !moved)
        {
            if(this.character != null)
            {
                if (canvas.gameObject.activeInHierarchy) canvas.gameObject.SetActive(false);
                
                GameObject move = canvas.GetComponentInChildren<UnityEngine.UI.Image>().transform.Find("ButtonMove").gameObject;
                move.SetActive(false);
                StartCoroutine(moveState());
      
            }

            //ahora mismo aqui no entra por lanzar el evento como corutina
            if (moved && attacked)
            {
                state = TurnState.Finalize;
                GameObject attack = canvas.GetComponentInChildren<UnityEngine.UI.Image>().transform.Find("ButtonAttack").gameObject;
                attack.SetActive(false);
                GameObject skillb = canvas.GetComponentInChildren<UnityEngine.UI.Image>().transform.Find("ButtonSkill").gameObject;
                skillb.SetActive(false);
            }


        }
          
         //pressed attack button
        else if (state == TurnState.Attack && !attacked)
        {
            if (this.character != null)
            {
                if (canvas.gameObject.activeInHierarchy) canvas.gameObject.SetActive(false);
                StartCoroutine(attackState());
                GameObject attack = canvas.GetComponentInChildren<UnityEngine.UI.Image>().transform.Find("ButtonAttack").gameObject;
                attack.SetActive(false);
                GameObject skillb = canvas.GetComponentInChildren<UnityEngine.UI.Image>().transform.Find("ButtonSkill").gameObject;
                skillb.SetActive(false);
            }

            //ahora mismo aqui no entra por lanzar el evento como corutina
            if (moved && attacked) {
                state = TurnState.Finalize;
                GameObject attack = canvas.GetComponentInChildren<UnityEngine.UI.Image>().transform.Find("ButtonAttack").gameObject;
                attack.SetActive(false);
                GameObject skillb = canvas.GetComponentInChildren<UnityEngine.UI.Image>().transform.Find("ButtonSkill").gameObject;
                skillb.SetActive(false);
            }
        }

        //pressed skill list
        else if (state == TurnState.Skill && !attacked)
        {
            if (this.character != null)
            {
                if (canvas.GetComponentInChildren<UnityEngine.UI.Image>().IsActive())
                {
                    unactivateButtons();
                    canvas.GetComponentInChildren<UnityEngine.UI.Image>().enabled = false;
                }
                
                StartCoroutine(skillState());
                GameObject attack = canvas.GetComponentInChildren<UnityEngine.UI.Image>().transform.Find("ButtonAttack").gameObject;
                attack.SetActive(false);
                GameObject skillb = canvas.GetComponentInChildren<UnityEngine.UI.Image>().transform.Find("ButtonSkill").gameObject;
                skillb.SetActive(false);
            }

            //ahora mismo aqui no entra por lanzar el evento como corutina
            if (moved && attacked)
            {
                state = TurnState.Finalize;
                GameObject attack = canvas.GetComponentInChildren<UnityEngine.UI.Image>().transform.Find("ButtonAttack").gameObject;
                attack.SetActive(false);
                GameObject skillb = canvas.GetComponentInChildren<UnityEngine.UI.Image>().transform.Find("ButtonSkill").gameObject;
                skillb.SetActive(false);
            }
        }

        //pressed deffend button
        else if (state == TurnState.Deffense && !deffended)
        {
            if (this.character != null)
            {
                state = TurnState.Finalize;
            deffendState();
            deffended = true;
            }
        }
            
        //if turn is finished
        if (state == TurnState.Finalize && ((moved && attacked) || deffended))
        {
            this.state = TurnState.Finalize;
        }
                  
    }


    //functionallity of moveButton
    private IEnumerator moveState()
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

        var selectedCell = eventFinished.getParameter("cellSelected") as Cell;

        //primero marcamos el move 
        evento = new GameEvent("move", new Dictionary<string, object>() {
                                 {"mover", character.Entity.mover },
                                 {"cell", selectedCell },
                                 {"synchronous", true }
                            });

        Game.main.enqueueEvent(evento);

        if (!canvas.gameObject.activeInHierarchy) canvas.gameObject.SetActive(true);  

        yield return new WaitForEventFinished(evento);
        moved = true;
    }


    //functionallity of attackButton
    private IEnumerator attackState()
    {
        Skill attack = new Skill("Attack", "Attack with main weapon", "", "", "", 1, 100, 1, null, 0);
        

        //colocamos la flecha en la posicion del personaje y lanzamos el evento de seleccionar objetivo
        IGameEvent evento = new GameEvent();
        evento.Name = "select cell";
        evento.setParameter("cell", character.Entity.Position);
        evento.setParameter("skill", attack);
        evento.setParameter("synchronous", true);
        Game.main.enqueueEvent(evento);
        IGameEvent eventFinished;

        yield return new WaitForEventFinished(evento, out eventFinished);

        var selectedCell = eventFinished.getParameter("cellSelected") as Cell;
        doDamage(selectedCell, attack);


        //Realizar animaciones

        //terminar ronda de ataque, quitar puntos de daño, pm...
        attacked = true;
        if (!canvas.gameObject.activeInHierarchy) canvas.gameObject.SetActive(true);

    }

    //functionallity of SkillButton
    private IEnumerator skillState()
    {
        GameObject newGO = new GameObject("Skill-List");
        //mostrar lista de habilidades disponibles para las caracteristicas del personaje
        Skill[] skills = skillsDataBase.getSavedSkills();
        UnityEngine.UI.Button[] buttons = new UnityEngine.UI.Button[skills.Length];
        //Seleccionar una   
        /*
        for(int i = 0; i < skills.Length; i++)
        {
          
        }*/
        Skill attack = skills[2];

        newGO.transform.position = new Vector3(60, 320, 0);
        newGO.AddComponent<UnityEngine.UI.Text>();
        newGO.GetComponent<UnityEngine.UI.Text>().color = Color.black;
        newGO.GetComponent<UnityEngine.UI.Text>().text = skillOnString();
        newGO.GetComponent<UnityEngine.UI.Text>().font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        newGO.transform.SetParent(canvas);
        

        //colocamos la flecha en la posicion del personaje y lanzamos el evento de seleccionar objetivo
        IGameEvent evento = new GameEvent();
        evento.Name = "select cell";
        evento.setParameter("cell", character.Entity.Position);
        evento.setParameter("skill", attack);
        evento.setParameter("synchronous", true);
        Game.main.enqueueEvent(evento);
        IGameEvent eventFinished;

        yield return new WaitForEventFinished(evento, out eventFinished);

        var selectedCell = eventFinished.getParameter("cellSelected") as Cell;
        doDamage(selectedCell, attack);

        //Realizar animaciones
        //terminar ronda de ataque, quitar puntos de daño, pm...
        attacked = true;
        Destroy(newGO);
        canvas.GetComponentInChildren<UnityEngine.UI.Image>().enabled = true;
        if (!moved)
        {
            GameObject move = canvas.GetComponentInChildren<UnityEngine.UI.Image>().transform.Find("ButtonMove").gameObject;
            move.SetActive(true);
        }

        GameObject deffend = canvas.GetComponentInChildren<UnityEngine.UI.Image>().transform.Find("ButtonDeffend").gameObject;
        deffend.SetActive(true);
    }


    //functionallity of deffendButton
    private void deffendState()
    {

    }

    //returns next playable character;
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

    //restarts the turn for every character playable in game
    private void restartTurn()
    {
        foreach (TRPGCharacter charact in characters) {
            if(charact.playable) charact.finishTurn(false);
        }
    }

    //return true if the turn has finished
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

    //returns a string with all the names of the skills
    private string skillOnString()
    {
        string skillsNames = "";
        foreach (Skill skill in skillsDataBase.getSavedSkills()){
            skillsNames = skillsNames + skill.getName() + '\n';
        }

        return skillsNames;
    }

    //activates all the ui buttons
    private void activateButtons()
    {
        GameObject move = canvas.GetComponentInChildren<UnityEngine.UI.Image>().transform.Find("ButtonMove").gameObject;
        move.SetActive(true);
        GameObject deffend = canvas.GetComponentInChildren<UnityEngine.UI.Image>().transform.Find("ButtonDeffend").gameObject;
        deffend.SetActive(true);
        GameObject attack = canvas.GetComponentInChildren<UnityEngine.UI.Image>().transform.Find("ButtonAttack").gameObject;
        attack.SetActive(true);
        GameObject skillb = canvas.GetComponentInChildren<UnityEngine.UI.Image>().transform.Find("ButtonSkill").gameObject;
        skillb.SetActive(true);
    }

    //unactivates all the ui buttons
    private void unactivateButtons()
    {
        GameObject move = canvas.GetComponentInChildren<UnityEngine.UI.Image>().transform.Find("ButtonMove").gameObject;
        move.SetActive(false);
        GameObject deffend = canvas.GetComponentInChildren<UnityEngine.UI.Image>().transform.Find("ButtonDeffend").gameObject;
        deffend.SetActive(false);
        GameObject attack = canvas.GetComponentInChildren<UnityEngine.UI.Image>().transform.Find("ButtonAttack").gameObject;
        attack.SetActive(false);
        GameObject skillb = canvas.GetComponentInChildren<UnityEngine.UI.Image>().transform.Find("ButtonSkill").gameObject;
        skillb.SetActive(false);
    }


    //metodo de prueba que genera un boton
    private static UnityEngine.UI.Button CreateButton(UnityEngine.UI.Button buttonPrefab, Skill skill)
    {
        var button = Object.Instantiate(buttonPrefab, Vector3.zero, Quaternion.identity) as UnityEngine.UI.Button;
 /*       var rectTransform = button.GetComponent<RectTransform>();
        rectTransform.SetParent(canvas.transform);
        rectTransform.anchorMax = cornerTopRight;
        rectTransform.anchorMin = cornerBottomLeft;
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.offsetMin = Vector2.zero;*/
        return button;
    }


    //deals the damage of the skill in the selected character;
    private void doDamage(Cell cell, Skill skill)
    {
        bool damageDone = false;
        foreach (TRPGCharacter ch in characters)
        {
            if (ch.Entity.Position == cell)
            {
                ch.receibeDamage(skill);
                damageDone = true;
                if (ch.isDead())
                {
                    ch.playable = false;
                    Debug.Log("El enemigo ha muerto");
                    playableCharacters--;
                    //cambiar la textura a muerto.
                }
            }
        }

        if (!damageDone)
        {
            Debug.Log("La habilidad ha fallado");
        }
    }


    // Update is called once per frame
    void Update () {
		
	}
}
