using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IsoUnity;
using IsoUnity.Entities;

public delegate void SetCharacterPositionCallBack(bool result);
public delegate void MoveCharacterToCallBack(bool result);
public delegate void ShowAreaCallBack(Cell selectedCell, bool result);
public delegate void SkillsCallBack(Cell selectedCell, bool result, List<CharacterScript> characters = null);
public delegate void MoveCameraToCallback(bool result);

public interface ITRPGMapConnector {
    List<Cell> calculateAttackPath(Character character, Cell cell);
    void cleanCells();
    void MoveCameraToCharacter(CharacterScript character, MoveCameraToCallback callback);
    void MoveCharacterTo(CharacterScript character, Cell cell, MoveCharacterToCallBack callback);
    void SetCharacterPosition(CharacterScript character, Cell cell, SetCharacterPositionCallBack callback);
    void ShowArea(CharacterScript character, EventTypes eventType, ShowAreaCallBack callback, Skills skill = null);
    void Skills(CharacterScript character, EventTypes eventType, SkillsCallBack callback, Skills skill = null);
    void triggerAnimation(Character character, Cell cell);
    CharacterScript GetCharacterAtCell(Cell cell);
    Cell GetCellAtCharacter(CharacterScript character);

    // IA Methods
    List<CharacterScript> GetAttackRangeTargets(CharacterScript character);
    List<Cell> GetPathFromCharToChar(CharacterScript character, CharacterScript target);
    void IAAttack(CharacterScript character, CharacterScript target, ShowAreaCallBack callback);
    void IAMove(CharacterScript character, Cell destiny, MoveCharacterToCallBack callback);
}
