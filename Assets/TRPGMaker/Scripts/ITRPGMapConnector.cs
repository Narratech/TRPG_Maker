using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IsoUnity;
using IsoUnity.Entities;

public interface ITRPGMapConnector {

 	void setCharacterPosition(Character character, Cell cell);
	 
	void moveCharacter(Character character, Cell cell);

	void showArea(Character character, Cell cell /*, caracteristicas*/);
	
	void showSelector(Cell cell);

    //void selectCell(ActionType actionType);

    List<Cell> calculateAttackPath(Character character, Cell cell);

    void triggerAnimation(Character character, Cell cell);

	Character getCharacterAtCell(Cell cell);

    void moveCameraToCharacter(Character character);
}
