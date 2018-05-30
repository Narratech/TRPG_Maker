using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IsoUnity;
using IsoUnity.Entities;
using System.Linq;
using System;
using UnityEngine.EventSystems;

public class IsoUnityConnector : EventedEventManager, ITRPGMapConnector
{

    private GameEvent selectedCellEvent;
    private IsoUnity.IsoTexture colorMove;
    private IsoUnity.IsoTexture colorAttack;
    private IsoUnity.IsoTexture colorSkill;
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
            colorSkill = isoUnityOptions.skillCell;
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

    /******************* ASYNC METHODS FOR EVENTS *******************/

    // Teleport Character to Cell position   
    public void SetCharacterPosition(CharacterScript character, Cell cell, SetCharacterPositionCallBack callback)
    {
        StartCoroutine(SetCharacterPositionAsync(character, cell, callback));
    }

    // Async method for teleport
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

    // Async method for move character
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
    public void ShowArea(CharacterScript character, EventTypes eventType, ShowAreaCallBack callback, Skills skill = null)
    {
        StartCoroutine(ShowAreaAsync(character, eventType, skill, callback));
    }

    // Async method for show area
    private IEnumerator ShowAreaAsync(CharacterScript character, EventTypes eventType, Skills skill, ShowAreaCallBack callback)
    {
        Entity entity = character.transform.GetComponent(typeof(Entity)) as Entity;
        IsoUnity.Cell characterCurrentCell = character.transform.parent.transform.GetComponent(typeof(IsoUnity.Cell)) as IsoUnity.Cell;
        try
        {
            entity.mover.maxJumpSize = character.character.attributesWithFormulas.Find(x => x.attribute.id == moveHeight.id).value;
        }
        catch (NullReferenceException e)
        {
            Debug.Log("Character '" + character.character.name + "' doesn't have attribute '" + moveHeight.name + "'");
        }

        selectedCellEvent = new GameEvent("selected cell", new Dictionary<string, object>()
        {
            {"synchronous", true }
        });

        Game.main.enqueueEvent(selectedCellEvent);
        try
        {
            if (eventType == EventTypes.MOVE)
                CalculateDistanceArea(entity, characterCurrentCell, eventType, character.character.attributesWithFormulas.Find(x => x.attribute.id == moveRange.id).value, character.character.attributesWithFormulas.Find(x => x.attribute.id == moveHeight.id).value);
            else if (eventType == EventTypes.ATTACK)
                CalculateDistanceArea(entity, characterCurrentCell, eventType, character.character.attributesWithFormulas.Find(x => x.attribute.id == attackRange.id).value, character.character.attributesWithFormulas.Find(x => x.attribute.id == attackHeight.id).value);
        }
        catch (NullReferenceException e)
        {
            Debug.Log(e);
        }

        Dictionary<string, object> outParams;
        yield return new WaitForEventFinished(selectedCellEvent, out outParams);
        cleanCells();
        IsoUnity.Cell selectedCell = outParams["cell"] as IsoUnity.Cell;
        Cell returnCell = new Cell((int)selectedCell.Map.getCoords(selectedCell.gameObject).x, (int)selectedCell.Map.getCoords(selectedCell.gameObject).y);
        callback(returnCell, true);
    }
    
    // Skills
    public void Skills(CharacterScript character, EventTypes eventType, SkillsCallBack callback, Skills skill = null)
    {
        StartCoroutine(SkillsAsync(character, eventType, skill, callback));
    }

    // Async method for skills
    private IEnumerator SkillsAsync(CharacterScript character, EventTypes eventType, Skills skill, SkillsCallBack callback)
    {
        Entity entity = character.transform.GetComponent(typeof(Entity)) as Entity;
        IsoUnity.Cell characterCurrentCell = character.transform.parent.transform.GetComponent(typeof(IsoUnity.Cell)) as IsoUnity.Cell;
        try
        {
            entity.mover.maxJumpSize = character.character.attributesWithFormulas.Find(x => x.attribute.id == moveHeight.id).value;
        }
        catch (NullReferenceException e)
        {
            Debug.Log("Character '" + character.character.name + "' doesn't have attribute '" + moveHeight.name + "'");
        }

        selectedCellEvent = new GameEvent("selected cell", new Dictionary<string, object>()
        {
            {"synchronous", true }
        });

        Game.main.enqueueEvent(selectedCellEvent);
        // Get all characters
        List<CharacterScript> characters = FindObjectsOfType<CharacterScript>().ToList();
        List<CharacterScript> targets = null;

        // Switch
        if (skill.skillType == SkillTypes.SINGLE_TARGET)
        {
            foreach (CharacterScript target in characters)
            {
                IsoUnity.Cell targetCell = target.transform.parent.transform.GetComponent(typeof(IsoUnity.Cell)) as IsoUnity.Cell;
                PaintCell(targetCell, eventType);
            }
        }
        else if (skill.skillType == SkillTypes.AREA)
        {
            targets = CalculateDistanceArea(entity, characterCurrentCell, eventType, skill.areaRange, int.MaxValue);
            PaintCell(characterCurrentCell, eventType);
        }
        else if (skill.skillType == SkillTypes.AREA_IN_OBJETIVE)
        {
            foreach (IsoUnity.Cell cell in characterCurrentCell.Map.Cells)
            {
                showSelector(cell, eventType, ((result) => {
                    cleanSkillCells();
                    targets = CalculateDistanceArea(null, result, eventType, skill.areaRange, int.MaxValue);
                    PaintCell(cell, eventType);
                }));
            }
        }

        Dictionary<string, object> outParams;
        yield return new WaitForEventFinished(selectedCellEvent, out outParams);
        cleanCells();
        IsoUnity.Cell selectedCell = outParams["cell"] as IsoUnity.Cell;
        Cell returnCell = new Cell((int)selectedCell.Map.getCoords(selectedCell.gameObject).x, (int)selectedCell.Map.getCoords(selectedCell.gameObject).y);

        if (skill.skillType == SkillTypes.SINGLE_TARGET)
        {
            targets = new List<CharacterScript>();
            targets.Add(selectedCell.transform.GetComponentInChildren<CharacterScript>());
        }

        callback(returnCell, true, targets);
    }

    // Camera look to Character
    public void MoveCameraToCharacter(CharacterScript character, MoveCameraToCallback callback)
    {
        StartCoroutine(MoveCameraToCharacterAsync(character, callback));
    }

    // Async method for move camera to character
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

    /******************* IA ASYNC METHODS *******************/

    // Automatically show attack area and select the target
    public void IAAttack(CharacterScript character, CharacterScript target, ShowAreaCallBack callback)
    {
        StartCoroutine(IAAttackAsync(character, target, callback));
    }

    // Async method for IA Atack
    private IEnumerator IAAttackAsync(CharacterScript character, CharacterScript target, ShowAreaCallBack callback)
    {
        Entity entity = character.transform.GetComponent(typeof(Entity)) as Entity;
        IsoUnity.Cell characterCurrentCell = character.transform.parent.transform.GetComponent(typeof(IsoUnity.Cell)) as IsoUnity.Cell;
        IsoUnity.Cell targetCurrentCell = target.transform.parent.transform.GetComponent(typeof(IsoUnity.Cell)) as IsoUnity.Cell;

        try
        {
            entity.mover.maxJumpSize = character.character.attributesWithFormulas.Find(x => x.attribute.id == moveHeight.id).value;
        }
        catch (NullReferenceException e)
        {
            Debug.Log("Character '" + character.character.name + "' doesn't have attribute '" + moveHeight.name + "'");
        }

        try
        {
            CalculateDistanceArea(entity, characterCurrentCell, EventTypes.IA_ATTACK, character.character.attributesWithFormulas.Find(x => x.attribute.id == attackRange.id).value, character.character.attributesWithFormulas.Find(x => x.attribute.id == attackHeight.id).value);
        }
        catch (NullReferenceException e)
        {
            Debug.Log("Character '" + character.character.name + "' doesn't have some or any of this attributes: '" + moveHeight.name + "', '" + moveRange.name + "', '" + attackHeight.name + "', '" + attackRange.name + "'");
        }

        yield return new WaitForSeconds(1f);
        GameObject arrowObject = targetCurrentCell.addDecoration(targetCurrentCell.transform.position + new Vector3(0, targetCurrentCell.WalkingHeight, 0), 0, false, true, arrow);
        yield return new WaitForSeconds(1f);
        GameObject.Destroy(arrowObject);
        cleanCells();
        yield return new WaitForSeconds(1f);
        callback(null, true);
    }

    // Automatically show move area, select the destination cell and move to cell
    public void IAMove(CharacterScript character, Cell destiny, MoveCharacterToCallBack callback)
    {
        StartCoroutine(IAMoveAsync(character, destiny, callback));
    }

    // Async method for IA Move
    private IEnumerator IAMoveAsync(CharacterScript character, Cell destiny, MoveCharacterToCallBack callback)
    {
        Entity entity = character.transform.GetComponent(typeof(Entity)) as Entity;
        IsoUnity.Cell characterCurrentCell = character.transform.parent.transform.GetComponent(typeof(IsoUnity.Cell)) as IsoUnity.Cell;
        IsoUnity.Cell destinyCell = SearchCellInMap(destiny);

        try
        {
            entity.mover.maxJumpSize = character.character.attributesWithFormulas.Find(x => x.attribute.id == moveHeight.id).value;
        }
        catch (NullReferenceException e)
        {
            Debug.Log("Character '" + character.character.name + "' doesn't have attribute '" + moveHeight.name + "'");
        }

        try
        {
            CalculateDistanceArea(entity, characterCurrentCell, EventTypes.IA_MOVE, character.character.attributesWithFormulas.Find(x => x.attribute.id == attackRange.id).value, character.character.attributesWithFormulas.Find(x => x.attribute.id == attackHeight.id).value);
        }
        catch (NullReferenceException e)
        {
            Debug.Log("Character '" + character.character.name + "' doesn't have some or any of this attributes: '" + moveHeight.name + "', '" + moveRange.name + "', '" + attackHeight.name + "', '" + attackRange.name + "'");
        }

        yield return new WaitForSeconds(1f);
        GameObject arrowObject = destinyCell.addDecoration(destinyCell.transform.position + new Vector3(0, destinyCell.WalkingHeight, 0), 0, false, true, arrow);
        yield return new WaitForSeconds(1f);
        GameObject.Destroy(arrowObject);
        cleanCells();
        yield return MoveCharacterToAsync(character, destiny, callback);
    }

    /******************* EVENTS *******************/

    // Selected cell
    [GameEvent(true, false)]
    public IEnumerator SelectedCell()
    {
        var starter = Current;
        Dictionary<string, object> outParams;
        yield return new WaitForEventFinished(starter, out outParams);
    }

    /******************* GET INFO AND AUX METHODS *******************/

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

    // Get cell where character is right now
    public Cell GetCellAtCharacter(CharacterScript character)
    {
        IsoUnity.Cell characterCell = character.transform.parent.transform.GetComponent(typeof(IsoUnity.Cell)) as IsoUnity.Cell;
        Cell cell = new Cell(characterCell.Map.getCoords(characterCell.gameObject).x, characterCell.Map.getCoords(characterCell.gameObject).y);
        return cell;
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

    // Get characters in attack range of the character
    // Returns the list of all the characters in the attack range
    public List<CharacterScript> GetAttackRangeTargets(CharacterScript character)
    {
        List<CharacterScript> characters = new List<CharacterScript>();
        Entity entity = character.transform.GetComponent(typeof(Entity)) as Entity;
        IsoUnity.Cell currentCell = character.transform.parent.transform.GetComponent(typeof(IsoUnity.Cell)) as IsoUnity.Cell;
        List<CellWithDistance> openList = new List<CellWithDistance>();
        List<CellWithDistance> closeList = new List<CellWithDistance>();
        float attackRan = character.character.attributesWithFormulas.Find(x => x.attribute.id == attackRange.id).value;
        float heighMax = character.character.attributesWithFormulas.Find(x => x.attribute.id == attackHeight.id).value;

        openList.Add(new CellWithDistance(currentCell, 0, null));
        while (openList.Count > 0)
        {
            CellWithDistance current = openList[0];
            openList.Remove(current);
            closeList.Add(current);
            foreach (IsoUnity.Cell neighbour in current.cell.Map.getNeightbours(current.cell))
            {
                if (neighbour != null && !closeList.Any(x => x.cell == neighbour) && neighbour.Walkable)
                {
                    float distanceManhattanFromCurrentToNeigh = Mathf.Abs(current.cell.Map.getCoords(current.cell.gameObject).x - neighbour.Map.getCoords(neighbour.gameObject).x) + Mathf.Abs(current.cell.Map.getCoords(current.cell.gameObject).y - neighbour.Map.getCoords(neighbour.gameObject).y);
                    float distanceManhattanFromCharacterToNeigh = current.distanceFromCharacter + distanceManhattanFromCurrentToNeigh;
                    if (distanceManhattanFromCurrentToNeigh <= 1 && distanceManhattanFromCharacterToNeigh <= attackRan &&
                        Mathf.Abs(neighbour.Height - current.cell.Height) <= heighMax)
                    {
                        if (!openList.Any(x => x.cell == neighbour))
                        {
                            CharacterScript charAtCell = GetCharacterAtCell(new Cell(neighbour.Map.getCoords(neighbour.gameObject).x, neighbour.Map.getCoords(neighbour.gameObject).y));
                            if (charAtCell != null)
                                characters.Add(charAtCell);
                            openList.Add(new CellWithDistance(neighbour, distanceManhattanFromCharacterToNeigh, null));
                        }
                    }
                }
            }
        }
        return characters;
    }

    // Pathfinding from one character to other
    // Returns cells of the path (in order)
    public List<Cell> GetPathFromCharToChar(CharacterScript character, CharacterScript target)
    {
        Entity entity = character.transform.GetComponent(typeof(Entity)) as Entity;
        IsoUnity.Cell currentCell = character.transform.parent.transform.GetComponent(typeof(IsoUnity.Cell)) as IsoUnity.Cell;
        IsoUnity.Cell targetCell = target.transform.parent.transform.GetComponent(typeof(IsoUnity.Cell)) as IsoUnity.Cell;
        List<CellWithDistance> openList = new List<CellWithDistance>();
        List<CellWithDistance> closeList = new List<CellWithDistance>();
        float attackRan = character.character.attributesWithFormulas.Find(x => x.attribute.id == attackRange.id).value;
        float heighMax = character.character.attributesWithFormulas.Find(x => x.attribute.id == attackHeight.id).value;

        openList.Add(new CellWithDistance(currentCell, 0, null));
        while (openList.Count > 0)
        {
            CellWithDistance current = openList[0];
            openList.Remove(current);
            closeList.Add(current);
            if (current.cell == targetCell)
            {
                List<Cell> path = new List<Cell>();
                while (current.previousCell != null)
                {
                    path.Add(new Cell(current.cell.Map.getCoords(current.cell.gameObject).x, current.cell.Map.getCoords(current.cell.gameObject).y));
                    current = current.previousCell;
                }
                path.Reverse();
                return path;
            }

            float distanceManhattan = Mathf.Abs(current.cell.Map.getCoords(current.cell.gameObject).x - targetCell.Map.getCoords(targetCell.gameObject).x) + Mathf.Abs(current.cell.Map.getCoords(current.cell.gameObject).y - targetCell.Map.getCoords(targetCell.gameObject).y);
            foreach (IsoUnity.Cell neighbour in current.cell.Map.getNeightbours(current.cell))
            {
                if (neighbour != null && !closeList.Any(x => x.cell == neighbour) && neighbour.Walkable)
                {
                    float distanceManhattanFromCurrentToNeigh = Mathf.Abs(current.cell.Map.getCoords(current.cell.gameObject).x - neighbour.Map.getCoords(neighbour.gameObject).x) + Mathf.Abs(current.cell.Map.getCoords(current.cell.gameObject).y - neighbour.Map.getCoords(neighbour.gameObject).y);
                    if (distanceManhattanFromCurrentToNeigh <= 1 && Mathf.Abs(neighbour.Height - current.cell.Height) <= heighMax)
                    {
                        if (!openList.Any(x => x.cell == neighbour))
                        {
                            CellWithDistance neigh = new CellWithDistance(neighbour, current.distanceFromCharacter + distanceManhattanFromCurrentToNeigh, current);
                            openList.Add(neigh);
                        }
                        else
                        {
                            CellWithDistance neigh = openList.Find(x => x.cell == neighbour);
                            if (current.distanceFromCharacter + distanceManhattanFromCurrentToNeigh < neigh.distanceFromCharacter)
                            {
                                neigh.distanceFromCharacter = current.distanceFromCharacter + distanceManhattanFromCurrentToNeigh;
                                neigh.previousCell = current;
                            }
                        }
                    }
                }
            }
        }
        return null;
    }

    /******************* PRIVATE AUX METHODS *******************/

    // Calculate the area of the event type
    private List<CharacterScript> CalculateDistanceArea(Entity entity, IsoUnity.Cell currentCell, EventTypes eventType, int distanceMax, int heighMax)
    {
        List<CellWithDistance> openList = new List<CellWithDistance>();
        List<CellWithDistance> closeList = new List<CellWithDistance>();
        List<CharacterScript> charactersInArea = new List<CharacterScript>();
        charactersInArea.Add(currentCell.transform.GetComponentInChildren<CharacterScript>());

        openList.Add(new CellWithDistance(currentCell, 0, null));
        while (openList.Count > 0)
        {
            CellWithDistance current = openList[0];
            openList.Remove(current);
            closeList.Add(current);
            foreach (IsoUnity.Cell neighbour in current.cell.Map.getNeightbours(current.cell))
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
                            openList.Add(new CellWithDistance(neighbour, distanceManhattanFromCharacterToNeigh, null));
                            CharacterScript character = neighbour.transform.GetComponentInChildren<CharacterScript>();
                            if (character != null) charactersInArea.Add(character);
                        }
                    }
                }
            }
        }

        return charactersInArea;
    }

    // Paint the cell in function of the event type
    private void PaintCell(IsoUnity.Cell cell, EventTypes eventType)
    {
        showSelector(cell, eventType);
        IsoTexture texture = null;
        switch (eventType)
        {
            case EventTypes.ATTACK:
            case EventTypes.IA_ATTACK:
                texture = colorAttack;
                break;
            case EventTypes.MOVE:
            case EventTypes.IA_MOVE:
                texture = colorMove;
                break;
            case EventTypes.SKILL:
                texture = colorSkill;
                break;
        }
        cell.Properties.faces[cell.Properties.faces.Length - 1].TextureMapping = texture;
        cell.Properties.faces[cell.Properties.faces.Length - 1].Texture = texture.getTexture();
        cell.forceRefresh();
    }

    public delegate void CallbackSelector(IsoUnity.Cell cell);

    // Show an arrow in selectables cells 
    private void showSelector(IsoUnity.Cell cell, EventTypes eventType, CallbackSelector callback = null)
    {
        SelectableCell selectableCell = cell.transform.gameObject.AddComponent<SelectableCell>();
        selectableCell.arrow = arrow;
        selectableCell.selectedCellEvent = selectedCellEvent;
        selectableCell.previousTexture = cell.Properties.faces[cell.Properties.faces.Length - 1].TextureMapping;
        selectableCell.cell = cell;
        selectableCell.eventType = eventType;
        selectableCell.callback = callback;
    }

    private void cleanSkillCells()
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
        }
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

    /******************* AUX CLASSES *******************/

    // Specific class for pathfinder and area algorithms
    class CellWithDistance
    {
        public IsoUnity.Cell cell;
        public float distanceFromCharacter;
        public CellWithDistance previousCell;
        public CellWithDistance(IsoUnity.Cell cell, float distance, CellWithDistance previous)
        {
            this.cell = cell;
            this.distanceFromCharacter = distance;
            this.previousCell = previous;
        }

        public void setPreviousCell(CellWithDistance previous) { this.previousCell = previous; }
        public CellWithDistance getPreviousCell() { return this.previousCell; }
    };

    // Specific class make a cell selectable and draw an arrow
    class SelectableCell : MonoBehaviour
    {
        public CallbackSelector callback;
        public GameEvent selectedCellEvent;
        public IsoDecoration arrow;
        public IsoUnity.Cell cell = null;
        public IsoTexture previousTexture;
        public GameObject arrowObject;
        public EventTypes eventType;

        public SelectableCell()
        {
        }

        private void OnMouseOver()
        {
            if (arrowObject == null)
            {
                if (eventType == EventTypes.ATTACK)
                {
                    CharacterScript character = cell.GetComponentInChildren<CharacterScript>();
                    if (character != null)
                    {
                        arrowObject = cell.addDecoration(cell.transform.position + new Vector3(0, cell.WalkingHeight, 0), 0, false, true, arrow);
                    }
                }
                else if (eventType == EventTypes.MOVE)
                {
                    arrowObject = cell.addDecoration(cell.transform.position + new Vector3(0, cell.WalkingHeight, 0), 0, false, true, arrow);
                }
                else if (eventType == EventTypes.SKILL)
                {
                    arrowObject = cell.addDecoration(cell.transform.position + new Vector3(0, cell.WalkingHeight, 0), 0, false, true, arrow);
                    if(callback != null) callback(cell);
                }
            }
        }

        void OnMouseExit()
        {
            if (arrowObject != null)
                GameObject.Destroy(arrowObject);
        }

        private void OnMouseDown()
        {
            if (selectedCellEvent != null && arrowObject != null && !EventSystem.current.IsPointerOverGameObject())
            {
                Game.main.eventFinished(selectedCellEvent, new Dictionary<string, object>
                {
                    {"cell", cell}
                });
            }
        }
    }
}