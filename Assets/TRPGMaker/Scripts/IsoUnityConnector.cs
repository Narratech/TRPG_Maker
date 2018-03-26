using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IsoUnity;
using IsoUnity.Entities;
using System.Linq;

public class IsoUnityConnector : EventedEventManager, ITRPGMapConnector  {

    private GameEvent selectedCellEvent;

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

        var setCharacterPositionEvent = new GameEvent("teleport", new Dictionary<string, object>()
        {
            {"mover", entity.mover },
            {"cell", destinyCell},
            {"synchronous", true }
        });

        Game.main.enqueueEvent(setCharacterPositionEvent);

        yield return new WaitForEventFinished(setCharacterPositionEvent);

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
        var moveCharacterTo = new GameEvent("move", new Dictionary<string, object>()
        {
            {"mover", entity.mover },
            {"cell", destinyCell},
            {"synchronous", true }
        });
        
        // Launch event
        Game.main.enqueueEvent(moveCharacterTo);

        yield return new WaitForEventFinished(moveCharacterTo);

        callback(true);
    }

    // Show calculated area with the character distance requirement
    public delegate void ShowAreaCallBack(Cell selectedCell, bool result);

    public void ShowArea(CharacterScript character, EventTypes eventType, ShowAreaCallBack callback)
    {
        StartCoroutine(ShowAreaAsync(character, eventType, callback));
    }

    private IEnumerator ShowAreaAsync(CharacterScript character, EventTypes eventType, ShowAreaCallBack callback)
    {

        Entity entity = character.transform.GetComponent(typeof(Entity)) as Entity;

        IsoUnity.Cell characterCurrentCell = character.transform.parent.transform.GetComponent(typeof(IsoUnity.Cell)) as IsoUnity.Cell;

        entity.mover.maxJumpSize = character.character.height;

        selectedCellEvent = new GameEvent("selected cell", new Dictionary<string, object>()
            {
                {"synchronous", true }
            });

        Game.main.enqueueEvent(selectedCellEvent);

        CalculateDistanceArea(entity, characterCurrentCell, eventType, character.character.distance, character.character.height);

        Dictionary<string, object> outParams;
        yield return new WaitForEventFinished(selectedCellEvent, out outParams);

        cleanCells();

        IsoUnity.Cell selectedCell = outParams["cell"] as IsoUnity.Cell;

        Cell returnCell = new Cell((int)selectedCell.Map.getCoords(selectedCell.gameObject).x, (int)selectedCell.Map.getCoords(selectedCell.gameObject).y);

        callback(returnCell, true);
    }

    // Speciific class for calculate distance for character
    struct CellWithDistance {
        public IsoUnity.Cell cell;
        public float distanceFromCharacter;

        public CellWithDistance(IsoUnity.Cell cell, float distance)
        {
            this.cell = cell;
            this.distanceFromCharacter = distance;
        }
    };   

    private void CalculateDistanceArea(Entity entity, IsoUnity.Cell currentCell, EventTypes eventType, int distanceMax, int heighMax)
    {
        /************ This would be changed! ********************/
        IsoUnity.Game game = GameObject.Find("Game").GetComponent<IsoUnity.Game>();
        IsoUnityOptions isoUnityOptions = game.transform.GetComponent(typeof(IsoUnityOptions)) as IsoUnityOptions;
        IsoUnity.IsoTexture colorMove = isoUnityOptions.moveCell;
        IsoUnity.IsoTexture colorAttack = isoUnityOptions.attackCell;
        IsoUnity.IsoDecoration arrow = isoUnityOptions.arrowDecoration;
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
                if (neighbour != null && !closeList.Any(x => x.cell == neighbour) && neighbour.Walkable)
                {
                    float distanceManhattanFromCurrentToNeigh = Mathf.Abs(current.cell.Map.getCoords(current.cell.gameObject).x - neighbour.Map.getCoords(neighbour.gameObject).x) + Mathf.Abs(current.cell.Map.getCoords(current.cell.gameObject).y - neighbour.Map.getCoords(neighbour.gameObject).y);
                    float distanceManhattanFromCharacterToNeigh = current.distanceFromCharacter + distanceManhattanFromCurrentToNeigh;

                    if (distanceManhattanFromCurrentToNeigh <= 1 && distanceManhattanFromCharacterToNeigh <= distanceMax && 
                        Mathf.Abs(neighbour.Height - current.cell.Height) <= heighMax)
                    {
                        if (!openList.Any(x => x.cell == neighbour))
                        {
                            switch (eventType)
                            {
                                case EventTypes.ATTACK:
                                    PaintCell(neighbour, eventType, colorAttack, arrow);
                                    break;
                                case EventTypes.MOVE:
                                    PaintCell(neighbour, eventType, colorMove, arrow);
                                    break;
                            }
                            openList.Add(new CellWithDistance(neighbour, distanceManhattanFromCharacterToNeigh));
                        }
                    }                    
                }
            }
        }
    }

    private void PaintCell(IsoUnity.Cell cell, EventTypes eventType, IsoTexture texture, IsoDecoration arrow)
    {
        showSelector(cell, eventType, arrow);

        cell.Properties.faces[cell.Properties.faces.Length - 1].TextureMapping = texture;
        cell.Properties.faces[cell.Properties.faces.Length - 1].Texture = texture.getTexture();
        cell.forceRefresh();
    }

    // Specific class for draw arrow in cell if is selectable
    class SelectableCell : MonoBehaviour
    {
        public GameEvent selectedCellEvent;
        public IsoDecoration arrow;
        public IsoUnity.Cell cell = null;
        public IsoTexture previousTexture;
        public GameObject arrowObject;
        public EventTypes eventType;

        public SelectableCell() {
        }

        private void OnMouseOver()
        {
            if (arrowObject == null)
            {
                if (eventType == EventTypes.ATTACK)
                {
                    CharacterScript character = cell.GetComponentInChildren<CharacterScript>();
                    if(character != null)
                    {
                        arrowObject = cell.addDecoration(cell.transform.position + new Vector3(0, cell.WalkingHeight, 0), 0, false, true, arrow);
                    }
                }
                else if (eventType == EventTypes.MOVE) {
                    arrowObject = cell.addDecoration(cell.transform.position + new Vector3(0, cell.WalkingHeight, 0), 0, false, true, arrow);
                }
            }
        }

        void OnMouseExit()
        {
            if(arrowObject!= null)

                GameObject.Destroy(arrowObject);
        }

        private void OnMouseDown()
        {
            if (selectedCellEvent != null && arrowObject != null)
            {
                Game.main.eventFinished(selectedCellEvent, new Dictionary<string, object>
                {
                    {"cell", cell}
                });
            }
        }
    }

    // Show an arrow in selectables cells 
    private void showSelector(IsoUnity.Cell cell, EventTypes eventType, IsoDecoration arrow)
    {
        cell.transform.gameObject.AddComponent<SelectableCell>().arrow = arrow;
        SelectableCell selectableCell = cell.transform.gameObject.GetComponent<SelectableCell>();
        selectableCell.selectedCellEvent = selectedCellEvent;
        selectableCell.previousTexture = cell.Properties.faces[cell.Properties.faces.Length - 1].TextureMapping;
        selectableCell.cell = cell;
        selectableCell.eventType = eventType;
    }

    private void cleanCells()
    {
        SelectableCell[] selectableCells = FindObjectsOfType<SelectableCell>();

        foreach (SelectableCell selectableCell in selectableCells)
        {
            IsoUnity.Cell cell = selectableCell.cell;
            if (selectableCell.previousTexture != null)
            {
                cell.Properties.faces[cell.Properties.faces.Length - 1].TextureMapping = selectableCell.previousTexture;
                cell.Properties.faces[cell.Properties.faces.Length - 1].Texture = selectableCell.previousTexture.getTexture();
            }
            else
            {
                cell.Properties.faces[cell.Properties.faces.Length - 1].TextureMapping = null;
                cell.Properties.faces[cell.Properties.faces.Length - 1].Texture = null;
            }
            cell.forceRefresh();
            if (selectableCell.arrowObject != null)
                Destroy(selectableCell.arrowObject);
            Destroy(selectableCell);
        }

    }

    // Selected cell
    [GameEvent(true, false)]
    public IEnumerator SelectedCell()
    {
        var starter = Current;

        Dictionary<string, object> outParams;

        yield return new WaitForEventFinished(starter, out outParams);
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
    private CharacterScript getCharacterAtCell(IsoUnity.Cell cell)
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
