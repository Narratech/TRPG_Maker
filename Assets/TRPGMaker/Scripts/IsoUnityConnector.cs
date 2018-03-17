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

    // Teleport Character to Cell position
    public delegate void SetCharacterPositionCallBack(bool result);

    public void SetCharacterPosition(CharacterScript character, Cell cell, SetCharacterPositionCallBack callback)
    {
        StartCoroutine(SetCharacterPositionAsync(character, cell, callback));
    }

    private IEnumerator SetCharacterPositionAsync(CharacterScript character, Cell cell, SetCharacterPositionCallBack callback)
    {
        Entity entity = character.transform.GetComponent(typeof(Entity)) as Entity;

        IsoUnity.Cell destinyCell = SearchCellInMap(cell);

        var showAreaEvent = new GameEvent("teleport", new Dictionary<string, object>()
        {
            {"mover", entity.mover },
            {"cell", destinyCell},
            {"synchronous", true }
        });

        Game.main.enqueueEvent(showAreaEvent);

        yield return new WaitForEventFinished(showAreaEvent);
        callback(true);
    }

    // Move Character to Cell position
    public delegate void MoveCharacterToCallBack(bool result);

    public void MoveCharacterTo(CharacterScript character, Cell cell, MoveCharacterToCallBack callback /*, caracteristicas*/)
    {
        StartCoroutine(MoveCharacterToAsync(character, cell, callback));
    }

    private IEnumerator MoveCharacterToAsync(CharacterScript character, Cell cell, MoveCharacterToCallBack callback)
    {
        // Get entity of character
        Entity entity = character.transform.GetComponent(typeof(Entity)) as Entity;        

        IsoUnity.Cell destinyCell = SearchCellInMap(cell);

        // Set event
        var showAreaEvent = new GameEvent("move", new Dictionary<string, object>()
        {
            {"mover", entity.mover },
            {"cell", destinyCell},
            {"synchronous", true }
        });
        
        // Launch event
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

    // Camera look to Character
    public delegate void MoveCameraToCallback(bool result);

    public void MoveCameraToCharacter(CharacterScript character, MoveCameraToCallback callback)
    {
        StartCoroutine(MoveCameraToCharacterAsync(character, callback));
    }

    private IEnumerator MoveCameraToCharacterAsync(CharacterScript character, MoveCameraToCallback callback)
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

    private IsoUnity.Cell SearchCellInMap(Cell cell)
    {
        // Get map of scene
        IsoUnity.Map map = GameObject.Find("Map").GetComponent<IsoUnity.Map>();

        // Search cell with same coords
        IsoUnity.Cell isoCell = null;
        foreach (IsoUnity.Cell c in map.Cells)
        {
            var coords = map.getCoords(c.gameObject);
            if (coords.x == cell.x && coords.y == cell.y)
                isoCell = c;
        }

        if (isoCell == null)
            Debug.Log("The cell selected (coords x: " + cell.x + ", y: " + cell.y + ") doesn't exists in the current map!");

        return isoCell;
    }
}
