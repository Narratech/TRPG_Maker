using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using IsoUnity;
using UnityEngine;

public class CharacterManager: ScriptableObject
    {
    #region Attributes
    // CharacterManager singleton
    public static CharacterManager _instance;
    // Every character in game
    [SerializeField]
    List<CharacterSheet> _everyone;
    #endregion

    #region Constructor
    public CharacterManager()
        {
        _everyone=new List<CharacterSheet>();
        //Debug.Log("Character Database Instance created!");
        }
    #endregion

    #region Properties
    public static CharacterManager Instance
        {
        //get { return _instance == null ? _instance = new CharacterManager() : _instance; }
        get { return _instance == null ? _instance=UnityEditor.AssetDatabase.LoadAssetAtPath<CharacterManager>("Assets/TRPG/characters.asset") : _instance; }
        }

    public List<CharacterSheet> Everyone
        {
        get { return _everyone; }
        set { _everyone=value; }
        }
    #endregion

    #region Methods
    public void addDemoCharacters()
        {
        #region Player 1
        Dictionary<string,AttributeTRPG> actualAttributes1=new Dictionary<string, AttributeTRPG>(Database.Instance.Attributes); 
        // Resetting all attributes to 0
        foreach (KeyValuePair<string, AttributeTRPG> attr in actualAttributes1)
            {
            attr.Value.value=0;
            }
        // Filling attributes
        actualAttributes1["EXP"].value=0; 
        actualAttributes1["HPS"].value=100;
        actualAttributes1["MPS"].value=50;  
        actualAttributes1["STR"].value=1; 
        actualAttributes1["INT"].value=1;
        actualAttributes1["WIS"].value=1; 
        actualAttributes1["DEX"].value=1; 
        actualAttributes1["CON"].value=1; 
        actualAttributes1["CHA"].value=1;
        // Filling the class and specialization     
        SpecTemplate actualClass1=Database.Instance.Specs["Human"];
        SpecTemplate actualSpec1=Database.Instance.Specs["Swordman"];
        // Filling the items
        List<ItemTemplate> actualItems1=new List<ItemTemplate>();
        actualItems1.Add(Database.Instance.Items["Shield"]); 
        actualItems1.Add(Database.Instance.Items["Trousers"]); 
        actualItems1.Add(Database.Instance.Items["Shirt"]); 
        actualItems1.Add(Database.Instance.Items["Boots"]);
        actualItems1.Add(Database.Instance.Items["Sword"]);  
        // Filling the passives
        List<PassiveTemplate> actualPassives1=new List<PassiveTemplate>();
        CharacterSheet actualSheet1=ScriptableObject.CreateInstance<CharacterSheet>().Init("Aragorn","King of the humans",actualAttributes1,actualClass1,actualSpec1,actualItems1,actualPassives1);
        addCharacter(actualSheet1);
        //Debug.Log("Aragorn added");
        #endregion
        #region Player 2
        Dictionary<string,AttributeTRPG> actualAttributes2=new Dictionary<string, AttributeTRPG>(Database.Instance.Attributes);
        // Resetting all attributes to 0
        foreach (KeyValuePair<string, AttributeTRPG> attr in actualAttributes2)
            {
            attr.Value.value=0;
            }
        // Filling attributes
        actualAttributes2["EXP"].value=0;
        actualAttributes2["HPS"].value=100;
        actualAttributes2["MPS"].value=50; 
        actualAttributes2["STR"].value=2;
        actualAttributes2["INT"].value=2;
        actualAttributes2["WIS"].value=2; 
        actualAttributes2["DEX"].value=2; 
        actualAttributes2["CON"].value=2; 
        actualAttributes2["CHA"].value=2;
        // Filling the class and specialization     
        SpecTemplate actualClass2=Database.Instance.Specs["Human"];
        SpecTemplate actualSpec2=Database.Instance.Specs["Bowman"];
        // Filling the items
        List<ItemTemplate> actualItems2=new List<ItemTemplate>();
        actualItems2.Add(Database.Instance.Items["Trousers"]); 
        actualItems2.Add(Database.Instance.Items["Shirt"]); 
        actualItems2.Add(Database.Instance.Items["Boots"]);
        actualItems2.Add(Database.Instance.Items["Bow"]);  
        // Filling the passives
        List<PassiveTemplate> actualPassives2=new List<PassiveTemplate>();
        CharacterSheet actualSheet2=ScriptableObject.CreateInstance<CharacterSheet>().Init("Robin Hood","Most famous archer",actualAttributes2,actualClass2,actualSpec2,actualItems2,actualPassives2);
        addCharacter(actualSheet2);
        //Debug.Log("Robin Hood added");
        #endregion
        #region Player 3
        Dictionary<string,AttributeTRPG> actualAttributes3=new Dictionary<string, AttributeTRPG>(Database.Instance.Attributes);
        // Resetting all attributes to 0
        foreach (KeyValuePair<string, AttributeTRPG> attr in actualAttributes3)
            {
            attr.Value.value=0;
            }
        // Filling attributes
        actualAttributes3["EXP"].value=0;
        actualAttributes3["HPS"].value=100;
        actualAttributes3["MPS"].value=0; 
        actualAttributes3["STR"].value=0;
        actualAttributes3["INT"].value=0;
        actualAttributes3["WIS"].value=0; 
        actualAttributes3["DEX"].value=0; 
        actualAttributes3["CON"].value=0; 
        actualAttributes3["CHA"].value=0;
        // Filling the class and specialization     
        SpecTemplate actualClass3=Database.Instance.Specs["Dummy"];
        SpecTemplate actualSpec3=Database.Instance.Specs["Light Carrier"];
        // Filling the items
        List<ItemTemplate> actualItems3=new List<ItemTemplate>();
        actualItems3.Add(Database.Instance.Items["Light helmet"]); 
        actualItems3.Add(Database.Instance.Items["Light armor"]); 
        actualItems3.Add(Database.Instance.Items["Light armbands"]);
        actualItems3.Add(Database.Instance.Items["Light gloves"]);  
        actualItems3.Add(Database.Instance.Items["Light legbands"]);  
        actualItems3.Add(Database.Instance.Items["Light boots"]);  
        // Filling the passives
        List<PassiveTemplate> actualPassives3=new List<PassiveTemplate>();
        CharacterSheet actualSheet3=ScriptableObject.CreateInstance<CharacterSheet>().Init("Light dummy","Ready to be killed",actualAttributes3,actualClass3,actualSpec3,actualItems3,actualPassives3);
        addCharacter(actualSheet3);
        //Debug.Log("Light dummy added");
        #endregion
        #region Player 4
        Dictionary<string,AttributeTRPG> actualAttributes4=new Dictionary<string, AttributeTRPG>(Database.Instance.Attributes);
        // Resetting all attributes to 0
        foreach (KeyValuePair<string, AttributeTRPG> attr in actualAttributes4)
            {
            attr.Value.value=0;
            }
        // Filling attributes
        actualAttributes4["EXP"].value=0;
        actualAttributes4["HPS"].value=100;
        actualAttributes4["MPS"].value=0; 
        actualAttributes4["STR"].value=0;
        actualAttributes4["INT"].value=0;
        actualAttributes4["WIS"].value=0; 
        actualAttributes4["DEX"].value=0; 
        actualAttributes4["CON"].value=0; 
        actualAttributes4["CHA"].value=0;
        // Filling the class and specialization     
        SpecTemplate actualClass4=Database.Instance.Specs["Dummy"];
        SpecTemplate actualSpec4=Database.Instance.Specs["Heavy Carrier"];
        // Filling the items
        List<ItemTemplate> actualItems4=new List<ItemTemplate>();
        actualItems4.Add(Database.Instance.Items["Iron helmet"]); 
        actualItems4.Add(Database.Instance.Items["Iron armor"]); 
        actualItems4.Add(Database.Instance.Items["Iron armbands"]);
        actualItems4.Add(Database.Instance.Items["Iron gloves"]);  
        actualItems4.Add(Database.Instance.Items["Iron legbands"]);  
        actualItems4.Add(Database.Instance.Items["Iron boots"]);  
        // Filling the passives
        List<PassiveTemplate> actualPassives4=new List<PassiveTemplate>();
        CharacterSheet actualSheet4=ScriptableObject.CreateInstance<CharacterSheet>().Init("Heavy dummy","Ready to be killed too",actualAttributes4,actualClass4,actualSpec4,actualItems4,actualPassives4);
        addCharacter(actualSheet4);
        //Debug.Log("Heavy dummy added");
        #endregion
        UnityEditor.AssetDatabase.SaveAssets();
        //Debug.Log("Character database filled as 'Demo Character Database'!");
        }

    public void addCharacter(CharacterSheet character)
        {
        _everyone.Add(character);
        character.Store(this);
        }

    public CharacterSheet getCharacter(string nameId)
        {
        CharacterSheet returnSheet=null;
        int i=0;
        bool found=false;
        while (!found && i<_everyone.Count)
            {
            if (_everyone[i].NameId==nameId)
                {
                found=true;
                returnSheet=_everyone[i];
                }
            i++;
            }
        return returnSheet;
        }

    public void updateSheets()
        {
        foreach (var result in Everyone)
            {
            result.collectFormulas();
            result.updateSheet(null);
            }
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
        // -Hacer la accion que sea (ataque, defensa, habilidaDatabase.Instance...)
        // -Guardar nuevos valores del atributo en un diccionario estilo Dictionary<string,int> y hacer update
        // en las fichas que sean necesarias
        charList[indice_de_la_ficha_modificada].updateSheet(diccionario_con_atributos_modificados) 
        cm.Everyone=charList;
        }
    }
*/