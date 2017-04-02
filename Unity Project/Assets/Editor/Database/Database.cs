using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Database
    {
    // Data
    public Dictionary<string,Attribute> attributes;
    public Dictionary<string,SpecTemplate> specs;
    public Dictionary<string,ItemTemplate> items;
    public Dictionary<string,PassiveTemplate> passives;

    // Singleton
    protected static Database instance;

    // Constructor
    protected Database()
        {
        // Initializing structures
        attributes=new Dictionary<string, Attribute>();
        specs=new Dictionary<string, SpecTemplate>();
        items=new Dictionary<string, ItemTemplate>();
        passives=new Dictionary<string, PassiveTemplate>();
        // Creating example database
        createExampleDatabase(); 
        }  
    
    // Properties    
    public static Database Instance
        {
        get
            {
            return instance == null ? instance = new Database() : instance;
            }
        }

    // Methods
    private void createExampleDatabase()
        // Creates an Example Database for testing purpouses. Typically called from constructor. It follows
        // the way an user would create things from editor.
        {
        // Filling core core 'attributes' (exist in every RPG)
        Attribute level=new Attribute(true,"LVL","Level","Character level",1,100);
        Attribute hp=new Attribute(true,"HPS","HealthPoints","Character health points",0,1000);
        Attribute mp=new Attribute(true,"MPS","MagicPoints","Character magic points",0,1000);
        attributes.Add("LVL",level);
        attributes.Add("HPS",hp);
        attributes.Add("MPS",mp);
        // Filling core 'attributes' (exist in every RPG slitghtly different)
        Attribute strength=new Attribute(true,"STR","Strength","Strength necessary to fisical strikes",1,100);
        Attribute intelligence=new Attribute(true,"INT","Intelligence","Intelligence to trade and find things",1,100);
        Attribute wisdom=new Attribute(true,"WIS","Wisdom","Wisdom helps you interacting and upgrading things",1,100);
        Attribute dexterity=new Attribute(true,"DEX","Dexterity","Dexterity helps in a lot of things",1,100);
        Attribute constitution=new Attribute(true,"CON","Constitution","Constitution is for wearing things",1,100);
        Attribute charisma=new Attribute(true,"CHA","Charisma","Charisma could save you without using a weapon",1,100);
        attributes.Add("STR",strength);
        attributes.Add("INT",intelligence);
        attributes.Add("WIS",wisdom);
        attributes.Add("DEX",dexterity);
        attributes.Add("CON",constitution);
        attributes.Add("CHA",charisma);
        // Filling derived 'attributes' 
        Attribute damage=new Attribute(false,"dmg","Damage","Base damage for a weapon",1,10000);
        Attribute critical=new Attribute(false,"cri","Critical","Chance of a crtical strike",1,10000);
        Attribute trading=new Attribute(false,"tra","Trading","Trading capabilities",1,10000);
        Attribute dodging=new Attribute(false,"dod","Dodging","Dodging probability",1,10000);
        Attribute defense=new Attribute(false,"def","Defense","Defense probability",1,10000);
        Attribute fireDamage=new Attribute(false,"fdg","Fire damage","Fire damage for a weapon",1,10000);
        attributes.Add("dmg",damage);
        attributes.Add("cri",critical);
        attributes.Add("tra",trading);
        attributes.Add("dod",dodging);
        attributes.Add("def",defense);
        attributes.Add("fdg",fireDamage);
        // Filling 'items'
        // Sword item
        List<Formula> swordFormulas=new List<Formula>();
        swordFormulas.Add(new Formula("DEX","DEX+3",1));
        swordFormulas.Add(new Formula("dmg","6",1));
        ItemTemplate sword=new ItemTemplate("Sword","Common sword",swordFormulas,null);
        items.Add("Sword",sword);
        // Fire Sword item
        List<Formula> fireSwordFormulas=new List<Formula>();
        fireSwordFormulas.Add(new Formula("DEX","DEX+3",1));
        fireSwordFormulas.Add(new Formula("dmg","6",1));
        fireSwordFormulas.Add(new Formula("fdg","3",1));
        ItemTemplate fireSword=new ItemTemplate("Fire sword","Fire sword",fireSwordFormulas,null);
        items.Add("Fire sword",fireSword);
        // Axe item
        List<Formula> axeFormulas=new List<Formula>();
        ItemTemplate axe=new ItemTemplate("Axe","Common axe",axeFormulas,null);
        axeFormulas.Add(new Formula("dmg","8",1));
        items.Add("Axe",axe);
        // Bow item
        List<Formula> bowFormulas=new List<Formula>();
        ItemTemplate bow=new ItemTemplate("Bow","Common bow",bowFormulas,null);
        bowFormulas.Add(new Formula("dmg","2",1));
        items.Add("Bow",bow);
        /*
        // Filling 'passives'
        // Kindness passive
        List<Formula> kindnessFormula=new List<Formula>();
        kindnessFormula.Add(new Formula("CHA","CHA+3",1));
        PassiveTemplate kindness=new PassiveTemplate("Kindness","More charisma for being kind",kindnessFormula,null);
        passives.Add("kindness",kindness);
        // Doping passive
        List<Formula> dopingFormula=new List<Formula>();
        dopingFormula.Add(new Formula("HPS","HPS+10",1));
        PassiveTemplate doping=new PassiveTemplate("Doping","Gets a health boost",dopingFormula,null);
        passives.Add("doping",doping);
        // Tired passive
        List<Formula> tiredFormula=new List<Formula>();
        tiredFormula.Add(new Formula("HPS","HPS-10",1));
        PassiveTemplate tired=new PassiveTemplate("Tired","Gets tired",tiredFormula,null);
        passives.Add("tired",tired);
        // Tenacity passive
        List<Formula> tenacityFormula=new List<Formula>();
        tenacityFormula.Add(new Formula("def","STR*2+DEX*3",1));
        PassiveTemplate tenacity=new PassiveTemplate("Tenacity","Resilence to endure strikes",tenacityFormula,null);
        passives.Add("tenacity",tenacity);
        // Filling basic 'specs'
        // Human spec
        List<Formula> humanFormulas=new List<Formula>();
        humanFormulas.Add(new Formula("cri","STR*3",1));
        humanFormulas.Add(new Formula("tra","INT*2+WIS*3+CHA",2));
        List<Template> humanSlots=new List<Template>();
        SpecTemplate human=new SpecTemplate("Human","Humans are flesh and bones",humanFormulas,humanSlots);
        specs.Add("human",human);
        // Elf spec
        List<Formula> elfFormulas=new List<Formula>();
        elfFormulas.Add(new Formula("cri","1",1));
        elfFormulas.Add(new Formula("tra","INT*2+WIS*3+CHA",2));
        List<Template> elfSlots=new List<Template>();
        SpecTemplate elf=new SpecTemplate("Elf","Elves have pointy ears",elfFormulas,elfSlots);
        specs.Add("elf",elf);
        // Orc spec
        List<Formula> orcFormulas=new List<Formula>();
        orcFormulas.Add(new Formula("cri","STR*10",1));
        orcFormulas.Add(new Formula("tra","INT*2+WIS*4+CHA*2",2));
        List<Template> orcSlots=new List<Template>();
        SpecTemplate orc=new SpecTemplate("Orc","Orcs are typically green",orcFormulas,orcSlots);
        specs.Add("orc",orc);
        // Filling derived 'specs'
        // Swordman spec
        List<Formula> swordmanFormulas=new List<Formula>();
        swordmanFormulas.Add(new Formula("dod","CON*3",1));
        List<Template> swordmanSlots=new List<Template>();
        swordmanSlots.Add(specs["human"]);
        swordmanSlots.Add(specs["elf"]);
        swordmanSlots.Add(items["sword"]);
        SpecTemplate swordman=new SpecTemplate("Swordman","Swordman is a human/elf spec in which they can use swords",swordmanFormulas,swordmanSlots);
        specs.Add("swordman",swordman);
        */
        // END                
        Debug.Log("Example database created!");
        }

    public List<string> getAttribIdentifiers()
        {
        // Retrieves the list of identifiers for every Attribute stored in the database Dictionary 'attributes'
        List<string> attribIdentifiers=new List<string>();
        foreach (KeyValuePair<string,Attribute> result in attributes)
            {
            attribIdentifiers.Add(result.Value.id);
            }
        return attribIdentifiers;
        }

    public List<string> getItemNames()
        {
        // Retrieves the list of names (identifieres) for every ItemTemplate stored in the database Dictionary 'items'
        List<string> itemIdentifiers=new List<string>();
        foreach (KeyValuePair<string,ItemTemplate> result in items)
            {
            itemIdentifiers.Add(result.Value.nameId);
            }
        return itemIdentifiers;
        }

    public List<Formula> getItemFormulas(string nameId)
        {
        // Retrieves the list of formulas (Formula) for a ItemTemplate stored in the database Dictionary 'items'
        return items[nameId].formulas;
        }

    public List<Template> getItemSlots(string nameId)
        {
        // Retrieves the list of slots (Template) for a ItemTemplate stored in the database Dictionary 'items'
        return items[nameId].slots;
        }

    public void loadDatabase()
        // Retrieves the database from file to continue filling it. This file was (obviously) previously 
        // saved by the user when he was editing from Editor
        {
        // AssetDatabase.FindAssets("t:Mitipo");
        }

    public bool addItemTemplate(ItemTemplate it)
        // Adds a ItemTemplate to the database. The idea is that this method will be called from 
        // Editor when you CREATE an Item
        {  
        bool canAdd=false;
        canAdd=items.ContainsValue(it) ? false : true;
        if (canAdd)
            items.Add(it.nameId,it);
        return canAdd;
        }

    public bool modifyItemTemplate(ItemTemplate it)
        // Adds a ItemTemplate to the database. The idea is that this method will be called from 
        // Editor when you MODIFY an Item
        {  
        bool canModify=false;
        canModify=items.ContainsKey(it.nameId) ? true : false;
        if (canModify)
            {
            items.Remove(it.nameId);
            items.Add(it.nameId,it);
            }
        return canModify;
        }

    public bool deleteItemTemplate(string nameId)
        {
        // Deletes an ItemTemplate. This method will be called from Editor when you DELETE an Item
        items.Remove(nameId);
        return false;
        }

    public bool addTemplate(Template t)
        // Adds a Template (SpecTemplate, ItemTemplate, PassiveTemplate) to the database. The idea
        // is that this method will be called from Editor when you CREATE a Spec, Item or Passive
        //
        // NOT WORKING. Inheritance noob programming issue
        {
        bool canAdd=false;
        if (t is ItemTemplate)
            {
            canAdd=items.ContainsValue(t as ItemTemplate) ? false : true;
            if (canAdd)
                items.Add(t.nameId,t as ItemTemplate);
            }
        else if (t is PassiveTemplate)
            {

            }
        else if (t is SpecTemplate)
            {

            }
        return canAdd;
        }

    public bool deleteTemplate()
        {
        // Deletes a Template (SpecTemplate, ItemTemplate, PassiveTemplate) from the database. The idea
        // is that this method will be called from Editor when you DELETE a Spec, Item or Passive
        // TO DO
        return false;
        }

    public void modifyTemplate()
        // Modifies a Template (SpecTemplate, ItemTemplate, PassiveTemplate) in the database. The idea
        // is that this method will be called from Editor when you EDIT a Spec, Item or Passive
        {
        }

    public bool addAttribute(Attribute atr)
        // Adds a new Attribute to the database. Since every character in the game will need these
        // we can specify some as 'core' attributes which everyone will have. Attributes which are no
        // 'core' may be limited to certain kind of characters (Spec attached) or simply derived attributes
        // that everyone can posess but may never use
        {
        bool canAdd=attributes.ContainsValue(atr) ? false : true;
        if (canAdd)
            attributes.Add(atr.id,atr);               
        return canAdd;
        }

    public bool deleteAttribute(string atr)
        // Delete an existing Attribute from the database if this attribute exist. KEEP IN MIND that deleting an attribute
        // COULD cause problems if it has dependencies 
        {
        bool canDelete=attributes.ContainsKey(atr) ? true : false;
        if (canDelete)
            attributes.Remove(atr);
        return canDelete;
        }

    public void print()
        {
        Debug.Log("I'm a database");
        }

    }