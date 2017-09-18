using System.Collections;
using System.Collections.Generic;
using IsoUnity;
using UnityEngine;
using IsoUnity.Entities;

public class CellSelectorManager: IsoUnity.Entities.EventedEventManager
{

    public IsoDecoration arrowDecoration;
    public IsoTexture[] colorTextures = new IsoUnity.IsoTexture[3];
    private CellSelectionArrow arrow;
  

    private Cell cellSelected = null;

    [IsoUnity.Entities.GameEvent(false, false)]
    //crear una clase abstracta en vez de Skill, que permita todas las dif posibilidades de seleccion de movimiento, ataque, habilidades...
    //Cambiar void a IEnumerator por si nos interesa que el vento no termine nada mas seleccionar la casilla. Importante
    public IEnumerator SelectCell(Cell cell , Skill skill, Entity entity = null){

        var ge = Current;

        Painter paint = new Painter();
        //painting the accesible cells for the used skill
        paint.paintCells(cell, skill, colorTextures[skill.getTypeOfDamage()], entity);



        //busqueda recursiva
        //Usar el trazador de rutas de IsoUnity
        //si devuelve null es que el personaje no puede llegar a la celda,
        //si devuelv
        // IsoUnity.Cell[] vecinos = cell.Map.getNeightbours(cell);
        //sistema nativo de isounity para bloqueos
            // vecinos[0].isAccesibleBy()
            //can block me es diferente a blocks, bloquea a todo lo que quiera acceder
           //  vecinos[0].Entities[0].mover.blocks = false;
        //SolidBody interfaz para calcular enemigo/amigo bloqueo


        //create the new arrow
        GameObject obj = cell.addDecoration(cell.transform.position +  new Vector3(0, cell.WalkingHeight, 0), 0, false, true, arrowDecoration);
        arrow = obj.AddComponent<CellSelectionArrow>();
        IsoUnity.Entities.Entity ent = obj.AddComponent<IsoUnity.Entities.Entity>();
        var previousTarget = CameraManager.Instance.Target;
        StartCoroutine(Game.FindObjectOfType<CameraManager>().LookTo(obj, false));

        ent.mover.blocks = false;
        ent.Position = cell;
        cell.Map.registerEntity(ent);

        // Wait until selection
        while (!paint.accesibleCell(cellSelected))
        {
            yield return new WaitUntil(() => cellSelected != null);
        }
     
        paint.removePaint(cell, skill, entity);
        StartCoroutine(Game.FindObjectOfType<CameraManager>().LookTo(previousTarget, false));
        Game.main.eventFinished(ge, new Dictionary<string, object>()
        {
            {"cellSelected", cellSelected }
        });
 
        cellSelected = null;

        //Destroy the arrow and create a new one
        if (arrow != null)
        {
            Cell selectedCell = arrow.Entity.Position;
            arrow.Entity.Position.Map.unRegisterEntity(arrow.Entity);
            GameObject.DestroyImmediate(arrow.gameObject);
        }

    }


    //metodo recursivo


  

    private void Update()
    {
        if (Input.GetButtonDown("Fire2") && arrow)
        {

            cellSelected = arrow.Entity.Position;
        }
    }
}