using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellSelectorManager: IsoUnity.Entities.EventedEventManager
{

    public IsoUnity.IsoDecoration flechaDecotarion;
    public IsoUnity.Cell celda;
    public IsoUnity.IsoTexture selectingIsoTexture;
    public IsoUnity.IsoTexture[] oldTextures;
    public IsoUnity.Cell[] changedCells;
    private CellSelectionArrow arrow;

    [IsoUnity.Entities.GameEvent(false, false)]
    //crear una clase abstracta en vez de Skill, que permita todas las dif posibilidades de seleccion de movimiento, ataque, habilidades...
    //Cambiar void a IEnumerator por si nos interesa que el vento no termine nada mas seleccionar la casilla. Importante
    public void SelectCell(IsoUnity.Cell cell , Skill skill){



        if(arrow != null)
        {
            IsoUnity.Cell selectedCell = arrow.Entity.Position;
            arrow.Entity.Position.Map.unRegisterEntity(arrow.Entity);
            GameObject.DestroyImmediate(arrow.gameObject);
        }
       


        paintCells(cell, skill);
        //removePaint(cell, skill);

        //busqueda recursiva
         IsoUnity.Cell[] vecinos = cell.Map.getNeightbours(cell);
        //sistema nativo de isounity para bloqueos
            // vecinos[0].isAccesibleBy()
            //can block me es diferente a blocks, bloquea a todo lo que quiera acceder
           //  vecinos[0].Entities[0].mover.blocks = false;
        //SolidBody interfaz para calcular enemigo/amigo bloqueo


                //crear la flecha
               GameObject objeto = cell.addDecoration(cell.transform.position +  new Vector3(0, cell.WalkingHeight, 0), 0, false, true, flechaDecotarion);
              

                arrow = objeto.AddComponent<CellSelectionArrow>();
                IsoUnity.Entities.Entity entidad = objeto.AddComponent<IsoUnity.Entities.Entity>();
                entidad.Position = cell;
                cell.Map.registerEntity(entidad);
    
        //Obtenemos la casilla que queremos pintar con las nuevas posiciones


        //Pintamos la cara de arriba de la celda de color rojo, azul o verde segun el tipo de damage.



    }


    //metodo recursivo


    private void paintCells(IsoUnity.Cell cell, Skill skill)
    {
        Vector2 position = cell.Map.getCoords(cell.gameObject);

        changedCells = new IsoUnity.Cell[((skill.getDistance() * 2) + 1) * ((skill.getDistance() * 2) + 1)];
        oldTextures = new IsoUnity.IsoTexture[((skill.getDistance() * 2) + 1) * ((skill.getDistance() * 2) + 1)];
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
                    celdaAPintar.Properties.faces[celdaAPintar.Properties.faces.Length - 1].TextureMapping = selectingIsoTexture;
                    celdaAPintar.Properties.faces[celdaAPintar.Properties.faces.Length - 1].Texture = selectingIsoTexture.getTexture();
                    celdaAPintar.forceRefresh();
                    contador++;
                }

            }
        }

    }


    private void removePaint(IsoUnity.Cell cell, Skill skill)
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

                IsoUnity.Cell celdaAPintar = cell.Map.getCell(new Vector2(i, j));
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
        Skill habilidad = new Skill("bola de fuego", "", "", "", "", 2200, 2, null, 0);
        SelectCell(celda, habilidad);
    }
}