using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GamePlayManager : MonoBehaviour
{
    private GameObject objectCanvas;
    private ITRPGMapConnector connector;
    private CharacterScript character;
    private Boolean start = false;
    private int index;
    private List<CharacterScript> characters;

    // Use this for initialization
    void Start()
    {
        connector = (new GameObject("IsoUnityConector")).AddComponent<IsoUnityConnector>();

        startCombat();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void startCombat()
    {
        Turn();
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
                        randomTurn();
                        break;
                    case TurnTypes.Attribute:
                        // ATTRIBUTE mode. Each playable character is selected on the battlefield according to its attribute (What attribute? Add it to battle options)
                        /*
                         * A list of characters is created based on the attribute (Attribute selected in battle options
                         * We will pass the turn to each player in the list when performing the operation "ATTACK" or "DEFENDER", move allows you to continue using the turn
                         * When only one team remains standing, the game is over. Have another list with the characters of each team and their lives.
                         * A character, when dying, does not disappear from the battlefield, has 0 life and can be revived (future extensions)
                         */
                        attributeTurn();
                        break;
                    default:
                        Console.WriteLine("Default case");
                        break;
                }
            }
        }
        else
        {
            Debug.Log("Must be at least one character");
        }
    }

    private void StartTurn()
    {
        //check if the character is in a playable team
        Boolean isPlayable = true;
        //Get info about character "health attribute" (battle connection)

        character = characters[index];

        if (index < characters.Count - 1)
        {
            index++;
        }
        else if (index == characters.Count - 1)
        {
            index = 0;
        }

        if (isPlayable) // and health > 0
        {
            connector.MoveCameraToCharacter(character, MoveCameraToParametrizedCallback(character, (character0, result0) =>
            {
                DrawCanvas();
            }));

        }
        else if (!isPlayable)
        {
            // call to IA
        } // else if ( health <= 0) character dead, check if this is the last character in the team
    }

    private void randomTurn()
    {
        if (start == false)
        {
            index = 0;
            start = true;
            reshuffle();
            Debug.Log("Random Turn Mode");
        }

        StartTurn();
    }

    private void attributeTurn()
    {
        if (start == false)
        {
            // Create the list with the characters placed according to the "attribute"
            index = 0;
            start = true;
            Debug.Log("Attribute Turn Mode");
            Debug.Log(characters.Count);
        }

        StartTurn();
    }

    /*
     * Knuth shuffle algorithm
     */
    private void reshuffle()
    {

        for (int t = 0; t < characters.Count; t++)
        {
            CharacterScript tmp = characters[t];
            int r = UnityEngine.Random.Range(t, characters.Count);
            characters[t] = characters[r];
            characters[r] = tmp;
        }
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
        Image imagebutton = objectButtonMove.AddComponent<Image>();

        RectTransform rtButtonMove = objectButtonMove.transform.GetComponent(typeof(RectTransform)) as RectTransform;
        rtButtonMove.sizeDelta = new Vector2(100, 20);

        GameObject objectTextMove = new GameObject("Text");
        objectTextMove.transform.position = objectButtonMove.transform.position;
        objectTextMove.transform.parent = objectButtonMove.transform;
        Text textMove = objectTextMove.AddComponent<Text>();
        textMove.text = "Move";
        textMove.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        textMove.color = Color.black;
        RectTransform rtTextMove = objectTextMove.transform.GetComponent(typeof(RectTransform)) as RectTransform;
        rtTextMove.sizeDelta = rtButtonMove.sizeDelta;

        // Button position        
        rtButtonMove.position = new Vector2(rtButtonMove.position.x - rtCanvas.position.x + (rtButtonMove.sizeDelta.x / 2) + 20, rtButtonMove.position.y - rtCanvas.position.y + (rtButtonMove.sizeDelta.x / 2) + 20);

        buttonMove.onClick.AddListener(() => moveEvent());

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
        RectTransform rtTextAttack = objectTextAttack.transform.GetComponent(typeof(RectTransform)) as RectTransform;
        rtTextAttack.sizeDelta = rtButtonAttack.sizeDelta;

        // Button position        
        rtButtonAttack.position = new Vector2(rtButtonAttack.position.x - rtCanvas.position.x + (rtButtonAttack.sizeDelta.x / 2) + 20, rtButtonAttack.position.y - rtCanvas.position.y + (rtButtonAttack.sizeDelta.x / 2) - 20);

        buttonAttack.onClick.AddListener(() => attackEvent());
    }

    public void moveEvent()
    {
        connector.cleanCells();
        connector.ShowArea(character, EventTypes.MOVE, ShowAreaCallBackParametrizedCallback(character, (character1, selectedCell, result1) =>
        {
            connector.MoveCharacterTo(character, selectedCell, MoveCharacterToParametrizedCallback(character, (character5, resul5t) =>
            {
                Turn();
            }));
        }));
    }

    public void attackEvent()
    {
        connector.cleanCells();
        connector.ShowArea(character, EventTypes.ATTACK, ShowAreaCallBackParametrizedCallback(character, (character1, selectedCell, result1) =>
        {
            CharacterScript characterDestAttack = connector.GetCharacterAtCell(selectedCell);
            characterDestAttack.character.attributes.Find(x => x.id == Database.Instance.battleOptions.healthAttribute.id).value -= 50; // This is a test. CHANGE THIS!!
            Turn();
        }));
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
}