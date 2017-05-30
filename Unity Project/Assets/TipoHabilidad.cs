using UnityEditor;
using UnityEngine;
using System.Collections;

// Creates an instance of a primitive depending on the option selected by the user.

public class TipoHabilidad : EditorWindow
{
    private bool ventanaAbierta = false;
    public enum TipoObjetivo
{
    uniTargetEnemy = 0,
    uniTargetAlly = 1,
    allEnemies = 2,
    allAllies = 3,
    allMap = 4,
    area = 5
}


    void InstantiatePrimitive(TipoObjetivo op)
    {
        switch (op)
        {
            case TipoObjetivo.uniTargetEnemy:
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = Vector3.zero;
                break;
            case TipoObjetivo.uniTargetAlly:
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.position = Vector3.zero;
                break;
            case TipoObjetivo.allEnemies:
                GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                plane.transform.position = Vector3.zero;
                break;
            case TipoObjetivo.allAllies:
                GameObject plane2 = GameObject.CreatePrimitive(PrimitiveType.Plane);
                plane2.transform.position = Vector3.zero;
                break;
            case TipoObjetivo.allMap:
                GameObject plane3 = GameObject.CreatePrimitive(PrimitiveType.Plane);
                plane3.transform.position = Vector3.zero;
                break;
            case TipoObjetivo.area:
                if (!ventanaAbierta)
                {
                    ventanaAbierta = true;
                    UnityEditor.EditorWindow window = new UnityEditor.EditorWindow();
                    window.Show();

                }
                break;

            default:
                Debug.LogError("Unrecognized Option");
                break;
        }
    }


}