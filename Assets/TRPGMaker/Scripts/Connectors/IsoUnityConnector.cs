﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IsoUnity;
using IsoUnity.Entities;
using System.Linq;
using System;

public class IsoUnityConnector : EventedEventManager, ITRPGMapConnector {

    private GameEvent selectedCellEvent;
    private IsoUnity.IsoTexture colorMove;
    private IsoUnity.IsoTexture colorAttack;
    private IsoUnity.IsoDecoration arrow;

    private Attribute health;
    private Attribute moveRange;
    private Attribute moveHeight;
    private Attribute attackRange;
    private Attribute attackHeight;

    void Start()
    {
        // Get IsoUnityOptions
        try
        {
            IsoUnityOptions isoUnityOptions = GameObject.Find("Game").GetComponent(typeof(IsoUnityOptions)) as IsoUnityOptions;
            colorMove = isoUnityOptions.moveCell;
            colorAttack = isoUnityOptions.attackCell;
            arrow = isoUnityOptions.arrowDecoration;
        }
        catch (NullReferenceException e)
        {
            Debug.Log("IsoUnityConnector values are not properly defined");
        }

        // Get TRPGOptions
        try
        {
            health = Database.Instance.battleOptions.healthAttribute;
            moveHeight = Database.Instance.battleOptions.moveHeight;
            moveRange = Database.Instance.battleOptions.moveRange;
            attackHeight = Database.Instance.battleOptions.attackHeight;
            attackRange = Database.Instance.battleOptions.attackRange;
        }
        catch (NullReferenceException e)
        {
            Debug.Log("TRPGOptions in Battle Options values are not properly defined");
        }
    }

    // Teleport Character to Cell position   
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
    public void ShowArea(CharacterScript character, EventTypes eventType, ShowAreaCallBack callback)
    {
        StartCoroutine(ShowAreaAsync(character, eventType, callback));
    }

    private IEnumerator ShowAreaAsync(CharacterScript character, EventTypes eventType, ShowAreaCallBack callback)
    {

        Entity entity = character.transform.GetComponent(typeof(Entity)) as Entity;

        IsoUnity.Cell characterCurrentCell = character.transform.parent.transform.GetComponent(typeof(IsoUnity.Cell)) as IsoUnity.Cell;

        try
        {
            entity.mover.maxJumpSize = character.character.attributes.Find(x => x.id == moveHeight.id).value;
        } catch (NullReferenceException e)
        {
            Debug.Log("Character '"+ character.character.name + "' doesn't have attribute '" + moveHeight.name +  "'");
        }

        selectedCellEvent = new GameEvent("selected cell", new Dictionary<string, object>()
            {
                {"synchronous", true }
            });

        Game.main.enqueueEvent(selectedCellEvent);

        try
        {
            if (eventType == EventTypes.MOVE)
            CalculateDistanceArea(entity, characterCurrentCell, eventType, character.character.attributes.Find(x => x.id == moveRange.id).value, character.character.attributes.Find(x => x.id == moveHeight.id).value);
        else if (eventType == EventTypes.ATTACK)
            CalculateDistanceArea(entity, characterCurrentCell, eventType, character.character.attributes.Find(x => x.id == attackRange.id).value, int.MaxValue);
        }
        catch (NullReferenceException e)
        {
            Debug.Log("Character '" + character.character.name + "' doesn't have some or any of this attributes: '" + moveHeight.name + "', '" + moveRange.name + "', '" + attackHeight.name + "', '" + attackRange.name + "'");
        }

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
                            PaintCell(neighbour, eventType);
                            openList.Add(new CellWithDistance(neighbour, distanceManhattanFromCharacterToNeigh));
                        }
                    }                    
                }
            }
        }
    }

    private void PaintCell(IsoUnity.Cell cell, EventTypes eventType)
    {
        showSelector(cell, eventType);

        IsoTexture texture = null;
        switch (eventType)
        {
            case EventTypes.ATTACK:
                texture = colorAttack;
                break;
            case EventTypes.MOVE:
                texture = colorMove;
                break;
        }

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
    private void showSelector(IsoUnity.Cell cell, EventTypes eventType)
    {
        SelectableCell selectableCell = cell.transform.gameObject.AddComponent<SelectableCell>();
        selectableCell.arrow = arrow;
        selectableCell.selectedCellEvent = selectedCellEvent;
        selectableCell.previousTexture = cell.Properties.faces[cell.Properties.faces.Length - 1].TextureMapping;
        selectableCell.cell = cell;
        selectableCell.eventType = eventType;
    }

    public void cleanCells()
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

    // Get character at selected cell
    public CharacterScript GetCharacterAtCell(Cell cell)
    {
        IsoUnity.Cell selectedCell = SearchCellInMap(cell);

        CharacterScript character = selectedCell.transform.GetComponentInChildren<CharacterScript>();

        return character; 
    }

    // Camera look to Character
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
