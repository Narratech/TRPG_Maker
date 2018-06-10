using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

public class GamePlayManager : MonoBehaviour
{
    private GameObject objectCanvas;
    private ITRPGMapConnector connector;
    private CharacterScript character;
    private Boolean start = false;
    private int index;
    private List<CharacterScript> characters;
    private Boolean move;
    private Boolean attack;
    private int round = 0;
    private GameObject skillsObjectScrollRect;
    private Image imagebuttonSkill;

    // Use this for initialization
    void Start()
    {
        connector = (new GameObject("IsoUnityConector")).AddComponent<IsoUnityConnector>();

        StartCombat();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartCombat()
    {
        if (checkAttributes())
        {
            Turn();
        }
    }

    private void Turn()
    {
        /*
         * - Knowing teams in play
         * - know if they are playable or not
         * - Manage the turn based on the "turn type" of "battle options"
         * - Random for all the characters or for equipment? ? VARIOUS TYPES OF TURN!
         *
         * - Defeat / victory? IN COMBAT it is analyzed, return true or false depending on...
         * - If character dies, does not continue
        */

        TurnTypes turnType = Database.Instance.battleOptions.turnType;

        //Check if the battle is start to don't update constantly the characters' list
        if (start == false)
        {
            CharacterScript[] tempCharacters = FindObjectsOfType<CharacterScript>();
            characters = new List<CharacterScript>(tempCharacters);
            round = 0;
        }

        if (characters.Count > 0)
        {
            if (characters.Exists(x => x.team == null))
            {
                Debug.Log("Some character doesn't have any team assigned");
            }
            else
            {
                switch (turnType)
                {
                    case TurnTypes.Random:
                        // RANDOM mode. Each playable character is randomly selected on the battlefield
                        /* 
                         * Given the "teams" in the field, we created a random list with each of the characters, EYE with the non-playable!
                         * We will pass the turn to each player in the list when performing the operation "ATTACK" or "DEFENDER", move allows you to continue using the turn
                         * When only one team remains standing, the game is over. Have another list with the characters of each team and their lives.
                         * A character, when dying, does not disappear from the battlefield, has 0 life and can be revived (future extensions)
                         */
                        RandomTurn();
                        break;
                    case TurnTypes.Attribute:
                        // ATTRIBUTE mode. Each playable character is selected on the battlefield according to its attribute (What attribute? Add it to battle options)
                        /*
                         * A list of characters is created based on the attribute (Attribute selected in battle options
                         * We will pass the turn to each player in the list when performing the operation "ATTACK" or "DEFENDER", move allows you to continue using the turn
                         * When only one team remains standing, the game is over. Have another list with the characters of each team and their lives.
                         * A character, when dying, does not disappear from the battlefield, has 0 life and can be revived (future extensions)
                         */
                        AttributeTurn();
                        break;
                    default:                        
                        break;
                }
            }
        }
        else
        {
            Debug.Log("Must be at least one character in the scene");
        }
    }

    private void StartTurn()
    {
        //Get info about character "health attribute" (battle connection)

        int healthValue = characters[index].character.attributesWithFormulas.Find(x => x.attribute.id == Database.Instance.battleOptions.healthAttribute.id).value;

        character = characters[index];
        //Actual round here

        Boolean isPlayable = character.team.playable;

        index++;
        if (index == characters.Count)
        {
            index = 0;
            round++;
        }

        if (healthValue > 0)
        {
            if (isPlayable)
            {
                move = false;
                attack = false;
                connector.MoveCameraToCharacter(character, MoveCameraToParametrizedCallback(character, (character0, result0) =>
                {
                    DrawCanvas();
                }));

            }
            else if (!isPlayable)
            {
                connector.MoveCameraToCharacter(character, MoveCameraToParametrizedCallback(character, (character0, result0) =>
                {
                    IATurnManager();
                }));
            }
        }
        else if (healthValue <= 0)
        {
            //character is dead, check if this is the last character in the team 
            //Next turn
            Turn();
        }
    }

    private void RandomTurn()
    {
        if (start == false)
        {
            index = 0;
            start = true;
            Reshuffle();
        }

        StartTurn();
    }

    private void AttributeTurn()
    {
        if (start == false)
        {
            // Create the list with the characters placed according to the "attribute"
            index = 0;
            start = true;
        }

        StartTurn();
    }

    /*
     * Knuth shuffle algorithm
     */
    private void Reshuffle()
    {

        for (int t = 0; t < characters.Count; t++)
        {
            CharacterScript tmp = characters[t];
            int r = UnityEngine.Random.Range(t, characters.Count);
            characters[t] = characters[r];
            characters[r] = tmp;
        }
    }

    public void MoveEvent(Button button)
    {
        connector.cleanCells();
        if (!move)
        {
            connector.ShowArea(character, EventTypes.MOVE, ShowAreaCallBackParametrizedCallback(character, (character1, selectedCell, result1) =>
            {
                connector.MoveCharacterTo(character, selectedCell, MoveCharacterToParametrizedCallback(character, (character5, resul5t) =>
                {
                    //Check if the character attack? or if move lost turn?
                    move = true;
                    button.gameObject.SetActive(false);
                    if (attack)
                        Turn();

                }));
            }));
        }
    }

    public void AttackEvent()
    {
        connector.cleanCells();
        connector.ShowArea(character, EventTypes.ATTACK, ShowAreaCallBackParametrizedCallback(character, (character1, selectedCell, result1) =>
        {
            CharacterScript characterDestAttack = connector.GetCharacterAtCell(selectedCell);

            characterDestAttack.character.attributesWithFormulas.Find(x => x.attribute.id == Database.Instance.battleOptions.healthAttribute.id).value -= character.character.attributesWithFormulas.Find(x => x.attribute.id == Database.Instance.battleOptions.damageAttribute.id).value; 

            if (characterDestAttack.character.attributesWithFormulas.Find(x => x.attribute.id == Database.Instance.battleOptions.healthAttribute.id).value <= 0)
            {
                characterDestAttack.character.attributesWithFormulas.Find(x => x.attribute.id == Database.Instance.battleOptions.healthAttribute.id).value = 0;
                // Dead Animation
                connector.TriggerAnimation(characterDestAttack, "die", MoveCameraToParametrizedCallback(characterDestAttack, (character5, resul5t) =>
                {
                    attack = true;
                    objectCanvas.SetActive(false);
                    Turn();
                }));
            }
            else
            {
                attack = true;
                objectCanvas.SetActive(false);
                Turn();
            }
        }));
    }

    public void SkillEvent(Skills skill)
    {
        connector.cleanCells();
        connector.Skills(character, EventTypes.SKILL, SkillsCallBackParametrizedCallback(character, (character, selectedCell, result, targets) =>
        {
            foreach(CharacterScript target in targets)
            {
                var f = FormulaScript.Create("");
                foreach (Formula formula in skill.formulas)
                {
                    f.formula = formula.formula;
                    if (f.FormulaParser.IsValidExpression && target != null)
                    {
                        var r = f.FormulaParser.Evaluate(target.character.attributes);
                        AttributeValue attrbValue = target.character.attributesWithFormulas.Find(x => x.attribute.id == formula.attributeID);
                        if (attrbValue != null)
                        {
                            attrbValue.value += (int)r;
                            if (attrbValue.value > attrbValue.maxValue)
                                attrbValue.value = attrbValue.maxValue;
                        }
                    }
                }
            }
            Turn();
        }), skill);
    }

    public void attackEventIA(CharacterScript target)
    {
        connector.IAAttack(character, target, ShowAreaCallBackParametrizedCallback(character, (character1, selectedCell, result1) =>
        {
            target.character.attributesWithFormulas.Find(x => x.attribute.id == Database.Instance.battleOptions.healthAttribute.id).value -= character.character.attributesWithFormulas.Find(x => x.attribute.id == Database.Instance.battleOptions.damageAttribute.id).value;
            Turn();
        }));
    }

    public void moveEventIA()
    {
        //connector.cleanCells();

        // Get nearest character path
        int nearestDistance = int.MaxValue;
        List<Cell> nearestPath = null;

        foreach(CharacterScript charact in characters.Where(x => x.team != character.team))
        {
            List<Cell> cellsToCharacter = connector.GetPathFromCharToChar(character, charact);
            if(cellsToCharacter != null && cellsToCharacter.Count < nearestDistance)
            {
                nearestPath = cellsToCharacter;
                nearestDistance = cellsToCharacter.Count;
            }
        }

        // If there is any posible nearest path -> move
        if (nearestPath != null)
        {
            // Move to nearest cell
            int destinyCell = 0;
            int moveRange = character.character.attributesWithFormulas.Find(x => x.attribute.id == Database.Instance.battleOptions.moveRange.id).value;
            // Can't be 0, because is the enemy target position
            if (nearestPath.Count == 0)
                Turn();
            else { 
                if (nearestPath.Count - 1 < moveRange)
                    destinyCell = nearestPath.Count - 2; // -2 because -1 is the enemy target position
                else
                    destinyCell = moveRange - 2; // -2 because is 0-based and the position 0 have cost 1

                connector.IAMove(character, nearestPath[destinyCell], MoveCharacterToParametrizedCallback(character, (character1, result1) =>
                {
                    // Get a list of the characters in attack range
                    List<CharacterScript> targets = connector.GetAttackRangeTargets(character);

                    // Filter only other teams characters ordered by max health
                    targets = targets.Where(x => x.team != character.team).OrderByDescending(x => x.character.attributesWithFormulas.Find(y => y.attribute.id == Database.Instance.battleOptions.healthAttribute.id).value).ToList();

                    // Check if now is any attackable character
                    if (targets.Count > 0)
                        attackEventIA(targets.First());
                    else
                        Turn();
                }));
            }
        }
        else
            Turn();
    }

    private void IATurnManager()
    {
        // Get a list of the characters in attack range
        List<CharacterScript> targets = connector.GetAttackRangeTargets(character);

        // Filter only other teams characters ordered by max health
        targets = targets.Where(x => x.team != character.team).OrderByDescending(x => x.character.attributesWithFormulas.Find(y => y.attribute.id == Database.Instance.battleOptions.healthAttribute.id).value).ToList();

        // If any character is attackable, attack
        if(targets.Count > 0)
        {
            attackEventIA(targets.First());
        }
        else // Move to nearest character
        {
            moveEventIA();
        }
    }

    public void FinishTurnEvent()
    {
        connector.cleanCells();
        objectCanvas.SetActive(false);
        Turn();
    }

    private bool checkAttributes()
    {
        // Set all battle attributes required in a List
        List<Attribute> requiredAttributes = new List<Attribute>();
        requiredAttributes.Add(Database.Instance.battleOptions.healthAttribute);
        requiredAttributes.Add(Database.Instance.battleOptions.damageAttribute);
        requiredAttributes.Add(Database.Instance.battleOptions.moveRange);
        requiredAttributes.Add(Database.Instance.battleOptions.moveHeight);
        requiredAttributes.Add(Database.Instance.battleOptions.attackRange);
        requiredAttributes.Add(Database.Instance.battleOptions.attackHeight);

        // Check if all characters scripts contains this attributes and a character
        foreach (CharacterScript character in FindObjectsOfType<CharacterScript>()) {
            if(character.character == null)
            {
                Debug.Log("Missing 'character' in some or many 'character script' components");
                return false;
            }
            character.character.calculateFormulas();
            foreach (Attribute attribute in requiredAttributes) {
                if (!character.character.attributesWithFormulas.Any(x => x.attribute == attribute))
                {
                    Debug.Log("Character '" + character.character.name + "' doesn't have attribute '" + attribute.name + "'");
                    return false;
                }
            }
        }

        return true;
    }

    private void DrawCanvas()
    {
        //Required for buttons
        if (GameObject.Find("EventSystem") == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
        }

        // Creating UI Menu
        objectCanvas = new GameObject("Button Canvas");
        Canvas canvas = objectCanvas.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        objectCanvas.AddComponent<GraphicRaycaster>();
        RectTransform rtCanvas = objectCanvas.transform.GetComponent(typeof(RectTransform)) as RectTransform;

        // Button Move
        GameObject objectButtonMove = new GameObject("Button");
        objectButtonMove.transform.position = objectCanvas.transform.position;
        objectButtonMove.transform.parent = objectCanvas.transform;

        Button buttonMove = objectButtonMove.AddComponent<Button>();
        Image imageButtonMove = objectButtonMove.AddComponent<Image>();

        RectTransform rtButtonMove = objectButtonMove.transform.GetComponent(typeof(RectTransform)) as RectTransform;
        rtButtonMove.sizeDelta = new Vector2(100, 20);

        GameObject objectTextMove = new GameObject("Text");
        objectTextMove.transform.position = objectButtonMove.transform.position;
        objectTextMove.transform.parent = objectButtonMove.transform;
        Text textMove = objectTextMove.AddComponent<Text>();
        textMove.text = "Move";
        textMove.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        textMove.color = Color.black;
        textMove.alignment = TextAnchor.MiddleCenter;
        RectTransform rtTextMove = objectTextMove.transform.GetComponent(typeof(RectTransform)) as RectTransform;
        rtTextMove.sizeDelta = rtButtonMove.sizeDelta;

        // Button position        
        rtButtonMove.position = new Vector2(rtButtonMove.position.x - rtCanvas.position.x + (rtButtonMove.sizeDelta.x / 2) + 20, rtButtonMove.position.y - rtCanvas.position.y + (rtButtonMove.sizeDelta.x / 2) + 70);

        // Button Attack
        GameObject objectButtonAttack = new GameObject("Button");
        objectButtonAttack.transform.position = objectCanvas.transform.position;
        objectButtonAttack.transform.parent = objectCanvas.transform;

        Button buttonAttack = objectButtonAttack.AddComponent<Button>();
        Image imagebuttonAttack = objectButtonAttack.AddComponent<Image>();

        RectTransform rtButtonAttack = objectButtonAttack.transform.GetComponent(typeof(RectTransform)) as RectTransform;
        rtButtonAttack.sizeDelta = new Vector2(100, 20);

        GameObject objectTextAttack = new GameObject("Text");
        objectTextAttack.transform.position = objectButtonAttack.transform.position;
        objectTextAttack.transform.parent = objectButtonAttack.transform;
        Text textAttack = objectTextAttack.AddComponent<Text>();
        textAttack.text = "Attack";
        textAttack.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        textAttack.color = Color.black;
        textAttack.alignment = TextAnchor.MiddleCenter;
        RectTransform rtTextAttack = objectTextAttack.transform.GetComponent(typeof(RectTransform)) as RectTransform;
        rtTextAttack.sizeDelta = rtButtonAttack.sizeDelta;

        // Button position        
        rtButtonAttack.position = new Vector2(rtButtonAttack.position.x - rtCanvas.position.x + (rtButtonAttack.sizeDelta.x / 2) + 20, rtButtonAttack.position.y - rtCanvas.position.y + (rtButtonAttack.sizeDelta.x / 2) + 40);

        // Button Skills
        GameObject objectButtonSkill = new GameObject("Button");
        objectButtonSkill.transform.position = objectCanvas.transform.position;
        objectButtonSkill.transform.parent = objectCanvas.transform;

        Button buttonSkill = objectButtonSkill.AddComponent<Button>();
        imagebuttonSkill = objectButtonSkill.AddComponent<Image>();

        RectTransform rtButtonSkill = objectButtonSkill.transform.GetComponent(typeof(RectTransform)) as RectTransform;
        rtButtonSkill.sizeDelta = new Vector2(100, 20);

        GameObject objectTexSkill = new GameObject("Text");
        objectTexSkill.transform.position = objectButtonSkill.transform.position;
        objectTexSkill.transform.parent = objectButtonSkill.transform;
        Text textSkills = objectTexSkill.AddComponent<Text>();
        textSkills.text = "Skills";
        textSkills.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        textSkills.color = Color.black;
        textSkills.alignment = TextAnchor.MiddleCenter;
        RectTransform rtTextSkills = objectTexSkill.transform.GetComponent(typeof(RectTransform)) as RectTransform;
        rtTextSkills.sizeDelta = rtButtonSkill.sizeDelta;

        // Button position        
        rtButtonSkill.position = new Vector2(rtButtonSkill.position.x - rtCanvas.position.x + (rtButtonSkill.sizeDelta.x / 2) + 20, rtButtonSkill.position.y - rtCanvas.position.y + (rtButtonSkill.sizeDelta.x) - 40);

        // Button Defence
        GameObject objectButtonFinishTurn = new GameObject("Button");
        objectButtonFinishTurn.transform.position = objectCanvas.transform.position;
        objectButtonFinishTurn.transform.parent = objectCanvas.transform;

        Button buttonFinishTurn = objectButtonFinishTurn.AddComponent<Button>();
        Image imagebuttonFinishTurn = objectButtonFinishTurn.AddComponent<Image>();

        RectTransform rtButtonFinishTurn = objectButtonFinishTurn.transform.GetComponent(typeof(RectTransform)) as RectTransform;
        rtButtonFinishTurn.sizeDelta = new Vector2(100, 20);

        GameObject objectTexFinishTurn = new GameObject("Text");
        objectTexFinishTurn.transform.position = objectButtonFinishTurn.transform.position;
        objectTexFinishTurn.transform.parent = objectButtonFinishTurn.transform;
        Text textFinishTurn = objectTexFinishTurn.AddComponent<Text>();
        textFinishTurn.text = "Finish turn";
        textFinishTurn.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        textFinishTurn.color = Color.black;
        textFinishTurn.alignment = TextAnchor.MiddleCenter;
        RectTransform rtTextFinishTurn = objectTexFinishTurn.transform.GetComponent(typeof(RectTransform)) as RectTransform;
        rtTextFinishTurn.sizeDelta = rtButtonFinishTurn.sizeDelta;

        // Button position        
        rtButtonFinishTurn.position = new Vector2(rtButtonFinishTurn.position.x - rtCanvas.position.x + (rtButtonFinishTurn.sizeDelta.x / 2) + 20, rtButtonFinishTurn.position.y - rtCanvas.position.y + (rtButtonFinishTurn.sizeDelta.x) - 70);

        //Buttons listeners
        buttonMove.onClick.AddListener(() =>
        {
            if (skillsObjectScrollRect != null)
                Destroy(skillsObjectScrollRect);
            imagebuttonSkill.color = UnityEngine.Color.white;
            imagebuttonAttack.color = UnityEngine.Color.white;
            imageButtonMove.color = UnityEngine.Color.grey;
            MoveEvent(buttonMove);
        });

        buttonAttack.onClick.AddListener(() =>
        {
            if (skillsObjectScrollRect != null)
                Destroy(skillsObjectScrollRect);
            imagebuttonSkill.color = UnityEngine.Color.white;
            imageButtonMove.color = UnityEngine.Color.white;
            imagebuttonAttack.color = UnityEngine.Color.grey;
            AttackEvent();
        });

        buttonSkill.onClick.AddListener(() =>
        {
            imageButtonMove.color = UnityEngine.Color.white;
            imagebuttonAttack.color = UnityEngine.Color.white;
            imagebuttonSkill.color = UnityEngine.Color.grey;
            connector.cleanCells();
            skillButtonListener(rtCanvas);
        });

        buttonFinishTurn.onClick.AddListener(() =>
        {
            FinishTurnEvent();
        });
    }

    private void skillButtonListener(RectTransform rtCanvas)
    {
        skillsObjectScrollRect = new GameObject("RectTransform");
        skillsObjectScrollRect.transform.position = rtCanvas.transform.position;
        skillsObjectScrollRect.transform.parent = objectCanvas.transform;
        ScrollRect skillScrollRect = skillsObjectScrollRect.AddComponent<ScrollRect>();
        skillsObjectScrollRect.AddComponent<Mask>();
        //skillScrollRect.horizontalNormalizedPosition = 0;
        skillScrollRect.movementType = ScrollRect.MovementType.Unrestricted;
        skillScrollRect.horizontal = false;
        skillScrollRect.inertia = false;
        skillScrollRect.scrollSensitivity = 4;
        Image skillsImage = skillsObjectScrollRect.AddComponent<Image>();
        skillsImage.color = UnityEngine.Color.grey;
        RectTransform rt = skillsObjectScrollRect.GetComponent(typeof(RectTransform)) as RectTransform;
        rt.sizeDelta = new Vector2(125, 100);
        rt.position = new Vector2(rt.position.x - rtCanvas.position.x + (rt.sizeDelta.x / 2) + 150, rt.position.y - rtCanvas.position.y + (rt.sizeDelta.x) - 50);

        GameObject skillsObjectRectTransform = new GameObject("Content");
        skillsObjectRectTransform.transform.position = rtCanvas.transform.position;
        skillsObjectRectTransform.transform.parent = skillsObjectScrollRect.transform;
        RectTransform skillsRectTransform = skillsObjectRectTransform.AddComponent<RectTransform>();
        skillScrollRect.content = skillsRectTransform;

        // Get all the skills inside the specialized classes
        List<List<Skills>> skills = character.character.specializedClasses.Select(x => x.skills).ToList();

        foreach (List<Skills> skillList in skills)
        {
            for(int i = 0; i < skillList.Count; i++)
            {
                Skills skill = skillList[i];

                if (skill != null)
                {
                    GameObject objectButtonSkill = new GameObject("Button");
                    objectButtonSkill.transform.position = skillsObjectScrollRect.transform.position;
                    objectButtonSkill.transform.parent = skillsObjectRectTransform.transform;

                    Button buttonSkill = objectButtonSkill.AddComponent<Button>();
                    Image imagebuttonSkill = objectButtonSkill.AddComponent<Image>();

                    RectTransform rtButtonSkill = objectButtonSkill.transform.GetComponent(typeof(RectTransform)) as RectTransform;
                    rtButtonSkill.sizeDelta = new Vector2(100, 20);

                    GameObject objectTextSkill = new GameObject("Text");
                    objectTextSkill.transform.position = objectButtonSkill.transform.position;
                    objectTextSkill.transform.parent = objectButtonSkill.transform;
                    Text textSkill = objectTextSkill.AddComponent<Text>();
                    textSkill.text = skill.name;
                    textSkill.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
                    textSkill.color = Color.black;
                    textSkill.alignment = TextAnchor.MiddleCenter;
                    RectTransform rtTexttext = objectTextSkill.transform.GetComponent(typeof(RectTransform)) as RectTransform;
                    rtTexttext.sizeDelta = rtButtonSkill.sizeDelta;

                    // Button position        
                    rtButtonSkill.position = new Vector2(rt.position.x, rt.position.y - (30 * i) + 25);

                    //Buttons listeners
                    buttonSkill.onClick.AddListener(() =>
                    {
                        if (skillsObjectScrollRect != null)
                            Destroy(skillsObjectScrollRect);
                        this.imagebuttonSkill.color = UnityEngine.Color.white;
                        SkillEvent(skill);
                    });
                }
            }
        }
    }

    // Callbacks
    public MoveCharacterToCallBack MoveCharacterToParametrizedCallback(CharacterScript character, System.Action<CharacterScript, bool> callback)
    {
        return (result) => callback(character, result);
    }

    public MoveCameraToCallback MoveCameraToParametrizedCallback(CharacterScript character, System.Action<CharacterScript, bool> callback)
    {
        return (result) => callback(character, result);
    }

    public ShowAreaCallBack ShowAreaCallBackParametrizedCallback(CharacterScript character, System.Action<CharacterScript, Cell, bool> callback)
    {
        return (selectedCell, result) => callback(character, selectedCell, result);
    }

    public SkillsCallBack SkillsCallBackParametrizedCallback(CharacterScript character, System.Action<CharacterScript, Cell, bool, List<CharacterScript>> callback)
    {
        return (selectedCell, result, targets) => callback(character, selectedCell, result, targets);
    }
}