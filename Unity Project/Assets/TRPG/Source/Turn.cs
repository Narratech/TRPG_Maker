using System.Collections;
using System.Collections.Generic;
using IsoUnity;
using UnityEngine;
using IsoUnity.Entities;
using UnityEngine.UI;
using UnityEngine.Events;

public class Turn: EventedEventManager
{
    public Transform canvas;
    public Transform skillScroll;
    public Transform skillContent;
    public GameObject skillButton;

    private TRPGCharacter[] characters;
    private SkillsDB skillsDataBase;
    private TRPGCharacter character;
    private int playableCharacters = 0;

    private bool moved = false;
    private bool attacked = false;
    private bool deffended = false;
    private TurnState state = TurnState.Idle;

    private int contador;


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
        contador = characters.Length;
        //startCharacters();
        foreach (TRPGCharacter charac in characters)
            if (charac.playable)
                playableCharacters++;
                
        while (contador > 2)
        {
            //Ordenar la lista de characters segun su "velocidad" o agilidad...
            turnOrder();
            //Turn of every playable character in game


            if (turnFinished()) restartTurn();

            this.character = selectNextCharacter();
            changeHPPlayer(this.character.getHP(), this.character.getName());
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
        Debug.Log("Fin de la partida");
        unactivateButtons();
        canvas.gameObject.SetActive(false);
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
                attack.SetActive(false);
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
                if (!canvas.GetComponentInChildren<UnityEngine.UI.Image>().IsActive())
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
        int movement = (int)this.character.getSpeed();
        Skill habilidad = new Skill("Mover", "", 0 , "", "", "", 2, 0, movement, null, 0);
        evento.setParameter("skill", habilidad);
        evento.setParameter("entity", character.Entity);
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
        int distance = (int)this.character.getSpeed();


        int damage = (int)this.character.getStrength();

        Skill attack = new Skill("Attack", "Attack with main weapon", 0, "", "", "", 1, damage, distance, null, 0);

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

            changeHPPlayer(this.character.getHP(), this.character.getName());

        //Realizar animaciones

        //terminar ronda de ataque, quitar puntos de daño, pm...
        attacked = true;
        if (!canvas.gameObject.activeInHierarchy) canvas.gameObject.SetActive(true);

    }
    private Skill selected = null;
    public UnityAction createLamdaFor(Skill s)
    {
        return () => selected = s;
    }

    //functionallity of SkillButton
    private IEnumerator skillState()
    {
        skillScroll.gameObject.SetActive(true);

        //mostrar lista de habilidades disponibles para las caracteristicas del personaje
        skillsDataBase = new SkillsDB();
        Skill[] skills = skillsDataBase.getSavedSkills();

        UnityEngine.UI.Button[] buttons = new UnityEngine.UI.Button[skills.Length];
        //Seleccionar una   

        List<GameObject> toDestroy = new List<GameObject>();
        selected = null;
        for (int i = 0; i < skills.Length; i++)
        {
            Skill s = skills[i];

            var newGO = GameObject.Instantiate(skillButton, skillContent);
            var rt = skillContent.gameObject.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, newGO.GetComponent<RectTransform>().rect.height * i+1);
            toDestroy.Add(newGO);
            newGO.GetComponentInChildren<Text>().text = s.getName();
            newGO.GetComponent<Button>().onClick.AddListener(createLamdaFor(s));
        }
        
        yield return new WaitUntil(() => selected != null);

        toDestroy.ForEach(go => DestroyImmediate(go));

            skillScroll.gameObject.SetActive(false);
            //colocamos la flecha en la posicion del personaje y lanzamos el evento de seleccionar objetivo
            IGameEvent evento = new GameEvent();
            evento.Name = "select cell";
            evento.setParameter("cell", character.Entity.Position);
            evento.setParameter("skill", selected);
            evento.setParameter("synchronous", true);
            Game.main.enqueueEvent(evento);
            IGameEvent eventFinished;

            yield return new WaitForEventFinished(evento, out eventFinished);

            var selectedCell = eventFinished.getParameter("cellSelected") as Cell;
            bool goodMP = this.character.lessMP(selected);



        if (goodMP)
        {
            Database.SkillAnimation animation = Database.Instance.animations.Find(a => selected.getName() == a.name);
            if (animation != null)
            {
                selected.castingAnimation = animation.castAnimation;
                selected.receiveAnimation = animation.receiveAnimation;
            }

            // Cast event (for animation)
            var cast = new GameEvent("cast skill", new Dictionary<string, object>()
            {
                { "trpgcharacter", character },
                { "skill", selected },
                { "synchronous", true }
            });

            Game.main.enqueueEvent(cast);

            yield return new WaitForEventFinished(cast);

            // Receive event (for animation)
            var receive = new GameEvent("receive skill", new Dictionary<string, object>()
            {
                { "trpgcharacter", selectedCell.GetComponentInChildren<TRPGCharacter>() },
                { "skill", selected },
                { "synchronous", true }
            });

            Game.main.enqueueEvent(receive);

            yield return new WaitForEventFinished(receive);

            doDamage(selectedCell, selected);
        }
        else Debug.Log("No mana for this skill");


        //Realizar animaciones
        //terminar ronda de ataque, quitar puntos de daño, pm...
        attacked = true;
        canvas.GetComponentInChildren<UnityEngine.UI.Image>().enabled = false;
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
        changeHPEnemy(-1, "");
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
            if (characters[i].playable)
            {
                finished = characters[i].turnFinished();
            }
          
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
        return button;
    }


    //deals the damage of the skill in the selected character;
    private bool doDamage(Cell cell, Skill skill)
    {
        bool damageDone = false;
        foreach (TRPGCharacter ch in characters)
        {
            if (ch.Entity.Position == cell)
            {
                ch.receiveDamage(skill);
                changeHPEnemy(ch.getHP(), ch.getName());
                damageDone = true;
                if (ch.isDead())
                {
                    ch.playable = false;
                    Debug.Log("El enemigo ha muerto");
                    contador--;

                    //cambiar la textura a muerto.
                }
            }
        }

        if (!damageDone)
        {
            Debug.Log("La habilidad ha fallado");
        }

        return damageDone;
    }

    void Muertos()
    {
        characters = IsoUnity.Map.FindObjectsOfType<TRPGCharacter>();
        foreach (TRPGCharacter charac in characters)
        {
            if (charac.isDead())
            {
                Destroy(charac);
            }
        }
    }


    // Update is called once per frame
    void Update () {
		
	}

    private void changeHPPlayer(double hp, string name)
    {
        /*
        GameObject hpcharacter = canvas.transform.Find("CharacterHP").gameObject;
        hpcharacter.GetComponentInChildren<Text>().text = name + ": " + hp.ToString();*/

    }

    private void changeHPEnemy(double hp, string name)
    {
        /*
         GameObject hpcharacter = canvas.transform.Find("EnemyHP").gameObject;

        if (hp == -1)  hpcharacter.GetComponentInChildren<Text>().text = "";
        else hpcharacter.GetComponentInChildren<Text>().text = name + ": " + hp.ToString();
        */
    }
}
