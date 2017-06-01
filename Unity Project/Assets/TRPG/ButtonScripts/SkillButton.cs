using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IsoUnity.Entities;
using IsoUnity;

public class SkillButton : EventedEventManager
{

    public void onClick()
    {
        //se salta este metodo despues de poner el mensaje en el Log
        StartCoroutine(throwEvent());
    }


    //Nunca entra aqui
    public IEnumerator throwEvent()
    {
        //Evento de prueba

        Game.main.enqueueEvent(new GameEvent("change state", new Dictionary<string, object>() {
            {"state", TurnState.Skill},
              {"synchronized", true}
        }));
        yield return null;


    }
}
