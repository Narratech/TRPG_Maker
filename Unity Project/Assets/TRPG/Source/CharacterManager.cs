using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IsoUnity;

public class CharacterManager
    {
    #region Attributes
    // CharacterManager singleton
    public static CharacterManager _instance; 
    // Every character in game
    List<CharacterSheet> _everyone;
    #endregion

    #region Constructor
    public CharacterManager()
        {
        // Initializing structures
        _everyone=new List<CharacterSheet>();
        }
    #endregion

    #region Properties
    public static CharacterManager Instance
        {
        get { return _instance == null ? _instance = new CharacterManager() : _instance; }
        //get { return instance == null ? instance=UnityEditor.AssetDatabase.LoadAssetAtPath<CharacterManager>("Assets/character_database.asset") : _instance; }
        }

    public List<CharacterSheet> Everyone
        {
        get { return _everyone; }
        set { _everyone=value; }
        }
    #endregion

    #region Methods
    public void addCharacter(CharacterSheet character)
        {
        _everyone.Add(character); 
        }
    #endregion
    }
/*
class Example
    {
    void method()
        { 
        CharacterManager cm;  // Obtiene el gestor de 'CharaceterSheet' que es un singleton (solo hay uno)
        List<CharacterSheet> charList=cm.Everyone;  // Obtiene la lista de 'CharacterSheet'
        // -Buscar el o las fichas de personaje que intervienen el la accion que estes haciendo. Las fichas se
        // buscan por su nameId
        // -Hacer la accion que sea (ataque, defensa, habilidad...)
        // -Guardar nuevos valores del atributo en un diccionario estilo Dictionary<string,int> y hacer update
        // en las fichas que sean necesarias
        charList[indice_de_la_ficha_modificada].updateSheet(diccionario_con_atributos_modificados) 
        cm.Everyone=charList;
        }
    }
*/