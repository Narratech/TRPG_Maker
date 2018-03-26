using System;
using System.Collections;
using System.Collections.Generic;
using IsoUnity;
using UnityEngine;

public class GamePlayManager : IsoUnity.EventManager
{
    public override void ReceiveEvent(IGameEvent ev)
    {
        //throw new NotImplementedException();
    }

    public override void Tick()
    {
        //throw new NotImplementedException();
    }

    // Use this for initialization
    // This is a TEST. This class NEEDS TO BE EDITED
    void Start()
    {
        IsoUnityConnector connector = (new GameObject("IsoUnityConector")).AddComponent<IsoUnityConnector>();
        CharacterScript[] characters = IsoUnity.Map.FindObjectsOfType<CharacterScript>();
        Cell cell = new Cell(13, -10);

        /*connector.MoveCharacterTo(characters[1], cell, MoveCharacterToParametrizedCallback(characters[1], (character, result) =>
        {
            Debug.Log("BIG BOY: ¡Finish moving!");
            connector.MoveCameraToCharacter(characters[0], MoveCameraToParametrizedCallback(characters[0], (character2, result2) =>
            {

                //Cell cell2 = new Cell(13, -9);
                Debug.Log("LITTLE BOY: ¡Start calculating distance!");
                /*connector.ShowArea(characters[0], ShowAreaCallBackParametrizedCallback(characters[0], (character3, result3) =>
                {
                    Debug.Log("LITTLE BOY: ¡Finish calculating distance!");
                }));

            }));
        }));*/

        //connector.SetCharacterPosition(characters[1], new Cell(11,-7), SetCharacterPositionParametrizedCallback(characters[1], (character, result) =>
        //{
            walkableLoop1(characters[1], connector);
        //}));           

        /*connector.MoveCharacterTo(characters[1], cell, MoveCharacterToParametrizedCallback(characters[1], (character, result) =>
        {
            Debug.Log("LITTLE BOY: ¡Finish calculating distance!");
        }));*/
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void walkableLoop1(CharacterScript character, IsoUnityConnector connector)
    {
        connector.MoveCameraToCharacter(character, MoveCameraToParametrizedCallback(character, (character0, result0) =>
        {
            connector.ShowArea(character, EventTypes.MOVE, ShowAreaCallBackParametrizedCallback(character, (character1, selectedCell, result1) =>
             {
                 connector.MoveCharacterTo(character, selectedCell, MoveCharacterToParametrizedCallback(character, (character5, resul5t) =>
                 {
                     // Alternate characters
                     CharacterScript[] characters = IsoUnity.Map.FindObjectsOfType<CharacterScript>();
                     characters[0].character.attributes.Find(x => x.id == "HP").value = 50;
                     walkableLoop2(characters[0], connector);
                 }));
             }));
        }));
    }

    private void walkableLoop2(CharacterScript character, IsoUnityConnector connector)
    {
        connector.MoveCameraToCharacter(character, MoveCameraToParametrizedCallback(character, (character0, result0) =>
        {

            connector.ShowArea(character, EventTypes.ATTACK, ShowAreaCallBackParametrizedCallback(character, (character1, selectedCell, result1) =>
            {
                CharacterScript[] characters = IsoUnity.Map.FindObjectsOfType<CharacterScript>();
                characters[1].character.attributes.Find(x => x.id == "HP").value = 50;
                walkableLoop1(characters[1], connector);
            }));
        }));
    }

    public IsoUnityConnector.MoveCharacterToCallBack MoveCharacterToParametrizedCallback(CharacterScript character, System.Action<CharacterScript, bool> callback)
    {
        return (result) => callback(character, result);
    }

    public IsoUnityConnector.MoveCameraToCallback MoveCameraToParametrizedCallback(CharacterScript character, System.Action<CharacterScript, bool> callback)
    {
        return (result) => callback(character, result);
    }

    public IsoUnityConnector.SetCharacterPositionCallBack SetCharacterPositionParametrizedCallback(CharacterScript character, System.Action<CharacterScript, bool> callback)
    {
        return (result) => callback(character, result);
    }

    public IsoUnityConnector.ShowAreaCallBack ShowAreaCallBackParametrizedCallback(CharacterScript character, System.Action<CharacterScript, Cell, bool> callback)
    {
        return (selectedCell, result) => callback(character, selectedCell, result);
    }
}