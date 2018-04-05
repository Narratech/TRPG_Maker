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
        CharacterScript[] characters = FindObjectsOfType<CharacterScript>();
        if (character == characters[1])
        {
            character = characters[0];
        }
        else
        {
            character = characters[1];
        }

        connector.MoveCameraToCharacter(character, MoveCameraToParametrizedCallback(character, (character0, result0) =>
        {
            DrawCanvas();
        }));
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