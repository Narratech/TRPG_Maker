using System.Collections;
using System.Collections.Generic;
using IsoUnity;
using UnityEngine;


public class CellSelectorManager: IsoUnity.Entities.EventedEventManager
{

    public IsoDecoration arrowDecoration;
    public Cell celda;
    public IsoTexture[] oldTextures;
    public IsoTexture[] colorTextures = new IsoUnity.IsoTexture[3];
    public Cell[] changedCells;
    private CellSelectionArrow arrow;

    [IsoUnity.Entities.GameEvent(false, false)]
    //crear una clase abstracta en vez de Skill, que permita todas las dif posibilidades de seleccion de movimiento, ataque, habilidades...
    //Cambiar void a IEnumerator por si nos interesa que el vento no termine nada mas seleccionar la casilla. Importante
    public void SelectCell(Cell cell , Skill skill){

        //Destroy the arrow and create a new one
        if(arrow != null)
        {
            Cell selectedCell = arrow.Entity.Position;
            arrow.Entity.Position.Map.unRegisterEntity(arrow.Entity);
            GameObject.DestroyImmediate(arrow.gameObject);
        }
       

        //painting the accesible cells for the used skill
        paintCells(cell, skill);



        //busqueda recursiva
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
        ent.Position = cell;
        cell.Map.registerEntity(ent);

    }


    //metodo recursivo


    private void paintCells(Cell cell, Skill skill)
    {
        Vector2 position = cell.Map.getCoords(cell.gameObject);

        changedCells = new Cell[((skill.getDistance() * 2) + 1) * ((skill.getDistance() * 2) + 1)];
        oldTextures = new IsoTexture[((skill.getDistance() * 2) + 1) * ((skill.getDistance() * 2) + 1)];
        int contador = 0;
        //busqueda directa
        //Vamos a marcar todas las casillas que esten en la distancia de la habilidad
        for (float i = position.x - skill.getDistance(); i <= position.x + skill.getDistance(); i++)
        {
            //vamos aumentando el radio de casillas hasta la distancia de la habilidad
            for (float j = position.y - skill.getDistance(); j <= position.y + skill.getDistance(); j++)
            {

                IsoUnity.Cell celdaAPintar = cell.Map.getCell(new Vector2(i, j));
                if (celdaAPintar != null)
                {
                    
                    changedCells[contador] = celdaAPintar;
                    oldTextures[contador] = celdaAPintar.Properties.faces[celdaAPintar.Properties.faces.Length - 1].TextureMapping;
                    celdaAPintar.Properties.faces[celdaAPintar.Properties.faces.Length - 1].TextureMapping = colorTextures[skill.getTypeOfDamage()];
                    celdaAPintar.Properties.faces[celdaAPintar.Properties.faces.Length - 1].Texture = colorTextures[skill.getTypeOfDamage()].getTexture();
                    celdaAPintar.forceRefresh();
                    contador++;
                }

            }
        }

    }


    private void removePaint(Cell cell, Skill skill)
    {
        Vector2 position = cell.Map.getCoords(cell.gameObject);
        int contador = 0;

        //busqueda directa
        //Vamos a marcar todas las casillas que esten en la distancia de la habilidad
        for (float i = position.x - skill.getDistance(); i <= position.x + skill.getDistance(); i++)
        {
            //vamos aumentando el radio de casillas hasta la distancia de la habilidad
            for (float j = position.y - skill.getDistance(); j <= position.y + skill.getDistance(); j++)
            {

                Cell celdaAPintar = cell.Map.getCell(new Vector2(i, j));
                if (celdaAPintar != null)
                {
                    celdaAPintar.Properties.faces[celdaAPintar.Properties.faces.Length - 1].TextureMapping = oldTextures[contador];
                    celdaAPintar.Properties.faces[celdaAPintar.Properties.faces.Length - 1].Texture = oldTextures[contador].getTexture();
                    celdaAPintar.forceRefresh();
                    contador++;
                }

            }
        }
    }

    void Start()
    {
        Skill habilidad = new Skill("bola de fuego", "", "", "", "",0, 2200 , 2, null, 0);
        SelectCell(celda, habilidad);
        removePaint(celda, habilidad);
    }
}