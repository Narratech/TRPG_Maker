using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IsoUnity;
using IsoUnity.Entities;

public class IsoUnityConnector : ITRPGMapConnector {
    
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
    public void moveCharacter(Character character, Cell cell)
    {

    }

    // Mostrar área para un ataque o movimiento de un Character
    // ActionType: define el tipo de acción (ataque, movimiento...)
    // ¿Realiza la acción o sólo devuelve información? Si realiza la acción
    // sobrarian bastantes metodos implementados a continuación
    public void showArea(Cell cell/*, caracteristicas*/)
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
    public void moveCameraToCharacter(Character character)
    {

    }
}
