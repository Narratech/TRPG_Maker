using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IsoUnity;
using IsoUnity.Entities;

public class IsoUnityConnector : MonoBehaviour/*, ITRPGMapConnector*/  {
    
    /*
     * Character contendrá una información concreta para realizar las acciones:
     *      - Distancia: cuantas casillas puede moverse o en que rango puede
     *                   atacar en un turno.
     *      - Altura: que casillas puede atravesar en función de la altura de la
     *                misma.
     *                
     * La habilidad utilizada tendrá tambien una información concreta para saber
     * a que celdas/personajes afecta o la animación que se realiza:
     *      - Tipo de ataque: Cuerpo a cuerpo ó a distancia.
     *      - Tipo de daño: solo a un personaje, daño en area ó daño por trayectoria
     *                      (este último especifica si hace daño a todos los personajes
     *                       que se encuentren en la trayectoria en la que se lanza la
     *                       habilidad).
     *
     */
    // Posicionar a un Character en una posicion del mapa
	public void setCharacterPosition(Character character, Cell cell)
    {

    }

    // Mover a un Character a una posicion
    // CAMBIAR ISOUNITY.CELL POR CELL DE TRPGMAKER!! 
    public delegate void MoveCharacterToCallBack(bool result);

    public void MoveCharacterTo(Character character, IsoUnity.Cell cell, MoveCharacterToCallBack callback /*, caracteristicas*/)
    {
        StartCoroutine(MoveCharacterToAsync(character, cell, callback));
    }

    private IEnumerator MoveCharacterToAsync(Character character, IsoUnity.Cell cell, MoveCharacterToCallBack callback)
    {
        Entity entity = character.transform.GetComponent(typeof(Entity)) as Entity;

        var showAreaEvent = new GameEvent("move", new Dictionary<string, object>()
        {
            {"mover", entity.mover },
            {"cell", cell},
            {"synchronous", true }
        });

        Game.main.enqueueEvent(showAreaEvent);

        yield return new WaitForEventFinished(showAreaEvent);
        callback(true);
    }

    // Mostrar área para un ataque o movimiento de un Character
    // ActionType: define el tipo de acción (ataque, movimiento...)
    // ¿Realiza la acción o sólo devuelve información? Si realiza la acción
    // sobrarian bastantes metodos implementados a continuación
    public void showArea(Character character, IsoUnity.Cell cell /*, caracteristicas*/)
    {
        
    }

    // Mostramos una flecha en las casillas en las que podemos realizar la acción
    public void showSelector(Cell cell)
    {

    }

    // Seleccionamos la casilla donde realizaremos la acción
    //ACTION TYPE, da errores
    /*public void selectCell(ActionType actionType)
    {

    }*/

    

    // Calculamos las celdas a las que afectará un ataque
    // en función de su trayectoria
    // ¿Quizás este método es privado? ¿Debería devolver a que Characters
    // afecta en lugar de a que celdas?
    public List<Cell> calculateAttackPath(Character character, Cell cell)
    {
        return new List<Cell>();
    }

    // Ralizamos la animacion de ataque, defensa...
    public void triggerAnimation(Character character, Cell cell)
    {

    }

    // Obtener el character que se encuentra en una posicion
    public Character getCharacterAtCell(Cell cell)
    {
        return null; // Si no hay nadie en esa casilla
    }

    // Centrar la cámara en un Character
    public delegate void moveCameraToCallback(bool result);

    public void moveCameraToCharacter(Character character, moveCameraToCallback callback)
    {
        StartCoroutine(moveCameraToCharacterAsync(character, callback));
    }

    private IEnumerator moveCameraToCharacterAsync(Character character, moveCameraToCallback callback)
    {
        Entity entity = character.transform.GetComponent(typeof(Entity)) as Entity;

        var lookToEvent = new GameEvent("look to", new Dictionary<string, object>()
        {
            {"gameobject", character.transform.gameObject },
            {"instant", false},
            {"synchronous", true }
        });

        Game.main.enqueueEvent(lookToEvent);

        yield return new WaitForEventFinished(lookToEvent);

        callback(true);
    }
}
