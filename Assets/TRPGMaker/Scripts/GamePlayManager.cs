using System;
using System.Collections;
using System.Collections.Generic;
using IsoUnity;
using UnityEngine;

public class GamePlayManager : IsoUnity.EventManager {
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
    void Start () {
        IsoUnityConnector connector = (new GameObject("IsoUnityConector")).AddComponent<IsoUnityConnector>();
        Character[] characters = IsoUnity.Map.FindObjectsOfType<Character>();
        IsoUnity.Cell cell = GameObject.Find("FinalCell").GetComponent<IsoUnity.Cell>();
        Debug.Log("BIG BOY: ¡Start moving!");
        connector.MoveCharacterTo(characters[1], cell, ParametrizedCallback(characters[1], (character, result) =>
        {
            Debug.Log("BIG BOY: ¡Finish moving!");
            connector.moveCameraToCharacter(characters[0], moveCameraToParametrizedCallback(characters[1], (character2, result2) =>
            {

                IsoUnity.Cell cell2 = GameObject.Find("FinalCell2").GetComponent<IsoUnity.Cell>();
                Debug.Log("LITTLE BOY: ¡Start moving!");
                connector.MoveCharacterTo(characters[0], cell2, ParametrizedCallback(characters[0], (character3, result3) =>
                {
                    Debug.Log("LITTLE BOY: ¡Finish moving!");                    
                }));

            }));
        }));
    }

    // Update is called once per frame
    void Update()
    {

    }

    public IsoUnityConnector.MoveCharacterToCallBack ParametrizedCallback(Character character, System.Action<Character, bool> callback)
    {
        return (result) => callback(character, result);
    }

    public IsoUnityConnector.moveCameraToCallback moveCameraToParametrizedCallback(Character character, System.Action<Character, bool> callback)
    {
        return (result) => callback(character, result);
    }
}
