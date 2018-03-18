using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IsoUnity;
using IsoUnity.Entities;
using System.Linq;

public class IsoUnityConnector : MonoBehaviour/*, ITRPGMapConnector*/  {

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

    // Show calculated area with the character distance requirement
    public delegate void ShowAreaCallBack(bool result);

    public void ShowArea(CharacterScript character, ShowAreaCallBack callback)
    {
        StartCoroutine(ShowAreaAsync(character, callback));
    }

    private IEnumerator ShowAreaAsync(CharacterScript character, ShowAreaCallBack callback)
    {

        Entity entity = character.transform.GetComponent(typeof(Entity)) as Entity;

        IsoUnity.Cell characterCurrentCell = character.transform.parent.transform.GetComponent(typeof(IsoUnity.Cell)) as IsoUnity.Cell;

        entity.mover.maxJumpSize = character.character.height;

        CalculateDistanceArea(entity, characterCurrentCell, character.character.distance);

        yield return true;

        callback(true);
    }

    struct CellWithDistance {
        public IsoUnity.Cell cell;
        public float distanceFromCharacter;

        public CellWithDistance(IsoUnity.Cell cell, float distance)
        {
            this.cell = cell;
            this.distanceFromCharacter = distance;
        }
    };

    private void CalculateDistanceArea(Entity entity, IsoUnity.Cell currentCell, int distanceMax)
    {
        /************ This would be changed! ********************/
        IsoUnity.Game game = GameObject.Find("Game").GetComponent<IsoUnity.Game>();
        IsoUnityOptions isoUnityOptions = game.transform.GetComponent(typeof(IsoUnityOptions)) as IsoUnityOptions;
        IsoUnity.IsoTexture color = isoUnityOptions.moveCell;
        /*******************************************************/

        List<CellWithDistance> openList = new List<CellWithDistance>();
        List<CellWithDistance> closeList = new List<CellWithDistance>();

        openList.Add(new CellWithDistance(currentCell, 0));

        while (openList.Count > 0)
        {
            CellWithDistance current = openList[0];
            openList.Remove(current);
            closeList.Add(current);

            foreach(IsoUnity.Cell neighbour in current.cell.Map.getNeightbours(current.cell))
            {
                if (neighbour != null && !closeList.Any(x => x.cell == neighbour))
                {
                    float distanceManhattan = Mathf.Abs(currentCell.Map.getCoords(currentCell.gameObject).x - neighbour.Map.getCoords(neighbour.gameObject).x) + Mathf.Abs(currentCell.Map.getCoords(currentCell.gameObject).y - neighbour.Map.getCoords(neighbour.gameObject).y);

                    if(distanceManhattan <= distanceMax)
                    {
                        bool planify = RoutePlanifier.planifyRoute(entity.mover, neighbour);

                        if (!openList.Any(x => x.cell == neighbour) && planify)
                        {
                            PaintCell(neighbour, color);
                            openList.Add(new CellWithDistance(neighbour, distanceManhattan));
                        }

                        RoutePlanifier.cancelRoute(entity.mover);
                    }                    
                }
            }
        }
    }

    private void PaintCell(IsoUnity.Cell cell, IsoTexture texture)
    {
        cell.Properties.faces[cell.Properties.faces.Length - 1].TextureMapping = texture;
        cell.Properties.faces[cell.Properties.faces.Length - 1].Texture = texture.getTexture();
        cell.forceRefresh();
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
