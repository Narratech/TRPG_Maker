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
        Debug.Log("BIG BOY: ¡Start moving!");
        connector.MoveCharacterTo(characters[1], cell, MoveCharacterToParametrizedCallback(characters[1], (character, result) =>
        {
            Debug.Log("BIG BOY: ¡Finish moving!");
            connector.MoveCameraToCharacter(characters[0], MoveCameraToParametrizedCallback(characters[0], (character2, result2) =>
            {

                Cell cell2 = new Cell(13, -9);
                Debug.Log("LITTLE BOY: ¡Start calculating distance!");
                connector.ShowArea(characters[0], ShowAreaCallBackParametrizedCallback(characters[0], (character3, result3) =>
                {
                    Debug.Log("LITTLE BOY: ¡Finish calculating distance!");
                }));

            }));
        }));
    }

    // Update is called once per frame
    void Update()
    {

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

    public IsoUnityConnector.ShowAreaCallBack ShowAreaCallBackParametrizedCallback(CharacterScript character, System.Action<CharacterScript, bool> callback)
    {
        return (result) => callback(character, result);
    }
}