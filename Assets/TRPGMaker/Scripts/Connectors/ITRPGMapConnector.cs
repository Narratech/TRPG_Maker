using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IsoUnity;
using IsoUnity.Entities;

public delegate void SetCharacterPositionCallBack(bool result);
public delegate void MoveCharacterToCallBack(bool result);
public delegate void ShowAreaCallBack(Cell selectedCell, bool result);
public delegate void MoveCameraToCallback(bool result);

public interface ITRPGMapConnector {
    List<Cell> calculateAttackPath(Character character, Cell cell);
    void cleanCells();
    void MoveCameraToCharacter(CharacterScript character, MoveCameraToCallback callback);
    void MoveCharacterTo(CharacterScript character, Cell cell, MoveCharacterToCallBack callback);
    void SetCharacterPosition(CharacterScript character, Cell cell, SetCharacterPositionCallBack callback);
    void ShowArea(CharacterScript character, EventTypes eventType, ShowAreaCallBack callback);
    void triggerAnimation(Character character, Cell cell);
    CharacterScript GetCharacterAtCell(Cell cell);

    // IA Methods
    List<CharacterScript> GetAttackRangeTargets(CharacterScript character);
    void IAAttack(CharacterScript character, CharacterScript target, ShowAreaCallBack callback);
}
