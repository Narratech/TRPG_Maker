using System.Collections;
using System.Collections.Generic;
using IsoUnity;
using UnityEngine;
using IsoUnity.Entities;

public class Painter{


    private Cell[] changedCells;
    private IsoTexture[] oldTextures;

    public Painter()
    {

    }

    public void paintCells(Cell cell, Skill skill, IsoTexture color, Entity entity = null)
    {
        Vector2 position = cell.Map.getCoords(cell.gameObject);

        changedCells = new Cell[((skill.getDistance() * 2) + 1) * ((skill.getDistance() * 2) + 1)];
        oldTextures = new IsoTexture[((skill.getDistance() * 2) + 1) * ((skill.getDistance() * 2) + 1)];
        int contador = 0;


        //busqueda directa
        float xcentral = position.x;
        float ycentral = position.y;
        //Vamos a marcar todas las casillas que esten en la distancia de la habilidad
        for (float i = position.x - skill.getDistance(); i <= position.x + skill.getDistance(); i++)
        {
            //vamos aumentando el radio de casillas hasta la distancia de la habilidad
            for (float j = position.y - skill.getDistance(); j <= position.y + skill.getDistance(); j++)
            {

                IsoUnity.Cell celdaAPintar = cell.Map.getCell(new Vector2(i, j));
                if (celdaAPintar != null)
                {           
                    if(entity != null){
                        if(RoutePlanifier.planifyRoute(entity.mover, celdaAPintar))
                        {
                            int distance = 0;
                            Cell measuring;
                            while (distance <= skill.getDistance() && (measuring = RoutePlanifier.next(entity.mover)))
                            {
                                entity.Position = measuring;
                                distance++;
                            }
                            entity.Position = cell;
                            
                            RoutePlanifier.cancelRoute(entity.mover);

                            if (distance > skill.getDistance()) continue;
                        }else continue;
                    }

                    if ((int)Mathf.Abs(Mathf.Abs(i - xcentral) + Mathf.Abs(j - ycentral)) <= skill.getDistance()) 
                    {
                        changedCells[contador] = celdaAPintar;
                        oldTextures[contador] = celdaAPintar.Properties.faces[celdaAPintar.Properties.faces.Length - 1].TextureMapping;
                        celdaAPintar.Properties.faces[celdaAPintar.Properties.faces.Length - 1].TextureMapping = color;
                        celdaAPintar.Properties.faces[celdaAPintar.Properties.faces.Length - 1].Texture = color.getTexture();
                        celdaAPintar.forceRefresh();
                        contador++;
                    }
                   
                }

            }
        }
        

    }


    public void removePaint(Cell cell, Skill skill, Entity entity = null)
    {
        Vector2 position = cell.Map.getCoords(cell.gameObject);
        int contador = 0;
        float xcentral = position.x;
        float ycentral = position.y;

        //busqueda directa
        //Vamos a marcar todas las casillas que esten en la distancia de la habilidad
        for (float i = position.x - skill.getDistance(); i <= position.x + skill.getDistance(); i++)
        {
            //vamos aumentando el radio de casillas hasta la distancia de la habilidad
            for (float j = position.y - skill.getDistance(); j <= position.y + skill.getDistance(); j++)
            {

                Cell celdaAPintar = cell.Map.getCell(new Vector2(i, j));

                if(celdaAPintar != null)
                {
                    if (entity != null)
                    {
                        if (RoutePlanifier.planifyRoute(entity.mover, celdaAPintar))
                        {
                            int distance = 0;
                            Cell measuring;
                            while (distance <= skill.getDistance() && (measuring = RoutePlanifier.next(entity.mover)))
                            {
                                entity.Position = measuring;
                                distance++;
                            }
                            entity.Position = cell;

                            RoutePlanifier.cancelRoute(entity.mover);

                            if (distance > skill.getDistance()) continue;
                        }
                        else continue;
                    }

                    if ((int)Mathf.Abs(Mathf.Abs(i - xcentral) + Mathf.Abs(j - ycentral)) <= skill.getDistance())
                    {
                        if(contador < oldTextures.Length && oldTextures[contador] != null)
                        {
                            celdaAPintar.Properties.faces[celdaAPintar.Properties.faces.Length - 1].TextureMapping = oldTextures[contador];
                            celdaAPintar.Properties.faces[celdaAPintar.Properties.faces.Length - 1].Texture = oldTextures[contador].getTexture();
                            celdaAPintar.forceRefresh();
                        }
                        contador++;
                    }
                }
            }
        }
    }


    public bool accesibleCell(Cell cell)
    {
        bool accesible = false;
        int count = 0;

        while(!accesible && count < changedCells.Length)
        {
            if(changedCells[count] != null) { 
                if (cell == changedCells[count]) accesible = true;
            }
            count++;
        }

        return accesible;
    }
}
