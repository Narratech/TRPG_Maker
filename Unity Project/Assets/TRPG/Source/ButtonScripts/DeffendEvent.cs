using System.Collections;
using System.Collections.Generic;
using IsoUnity;
using IsoUnity.Entities;
using UnityEngine;

public class DeffendEvent : EventedEventManager
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
            {"state", TurnState.Deffense},
              {"synchronized", true}
        }));
        yield return null;


    }
}
