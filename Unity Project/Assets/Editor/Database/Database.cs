using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// this version just uses tags with no hierarchy
// future version should use tags in hierarchy (trees): http://stackoverflow.com/questions/942053/why-is-there-no-treet-class-in-net

class Database
    {
    #region Attributes
    // Database elements
    public Dictionary<string,Attribute> attributes;  // Every Attribute in Database
    public Dictionary<string,SpecTemplate> specs;  // Every SpecTemplate in Database
    Dictionary<string,ItemTemplate> _items;  // Every ItemTemplate in Database
    public Dictionary<string,PassiveTemplate> passives;  // Every PassiveTemplate in Database
    // Info to show and relate database elements
    Dictionary<string,Dictionary<string,string>> _tags;  // Tags per template. Example: _tags["Item"]["Weapon"] returns "Weapon"
    SlotsOpt _slotsOptions;  // Slots options  // Options per template type in slots
    List<string> _templates;  // The types of 'Template' stored in database
    // Singleton
    protected static Database instance;
    #endregion

    #region Constructor
    protected Database()
        {
        // Initializing structures
        attributes=new Dictionary<string, Attribute>();
        specs=new Dictionary<string, SpecTemplate>();
        _items=new Dictionary<string, ItemTemplate>();
        passives=new Dictionary<string, PassiveTemplate>();
        _templates=new List<string>{ "Specialization", "Item", "Passive" };
        // Creating database
        createExampleDatabase(); 
        //createAbbeyDemoDatabase();
        //createEmptyDatabase();
        }
    #endregion

    #region Properties
    public static Database Instance
        {
        get { return instance == null ? instance = new Database() : instance; }
        }

    public Dictionary<string,Attribute> Attributes
        {
        get { return attributes; }
        set { attributes=value; }
        }

    public Dictionary<string,SpecTemplate> Specs
        {
        get { return specs; }
        set { specs=value; }
        }

    public Dictionary<string,ItemTemplate> Items
        {
        get { return _items; }
        set { _items=value; }
        }

    public Dictionary<string,PassiveTemplate> Passives
        {
        get { return passives; }
        set { passives=value; }
        }

    public List<string> Templates
        {
        get { return _templates; }
        set { value=_templates; }
        }

    public Dictionary<string,Dictionary<string,string>> Tags
        {
        get { return _tags; }
        set { value=_tags; }
        }

    public SlotsOpt SlotsOptions
        {
        get { return _slotsOptions; }
        set { value=_slotsOptions; }
        }
    #endregion

    #region Methods
    #region Creating Database: createExampleDatabase(), createAbbeyDemoDatabase(), createEmptyDatabase(), fillTags()
    private void createExampleDatabase()
        // Creates an Example Database for testing purpouses. Typically called from constructor. It follows
        // the way an user would create things from editor.
        {
        // Filling the tags and options
        fillTags();
        fillSlotOptions();
        // Filling core core 'attributes' (exist in every RPG)
        Attribute experience=new Attribute(true,"EXP","Experience","Experience acumulated",1,100);
        Attribute hp=new Attribute(true,"HPS","HealthPoints","Character health points",0,1000);
        Attribute mp=new Attribute(true,"MPS","MagicPoints","Character magic points",0,1000);
        attributes.Add("EXP",experience);
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
        Attribute level=new Attribute(false,"LVL","Level","Character level",1,100);
        Attribute damage=new Attribute(false,"dmg","Damage","Base damage for a weapon",1,10000);
        Attribute critical=new Attribute(false,"cri","Critical","Chance of a crtical strike",1,10000);
        Attribute trading=new Attribute(false,"tra","Trading","Trading capabilities",1,10000);
        Attribute dodging=new Attribute(false,"dod","Dodging","Dodging probability",1,10000);
        Attribute defense=new Attribute(false,"def","Defense","Defense probability",1,10000);
        Attribute fireDamage=new Attribute(false,"fdg","Fire damage","Fire damage for a weapon",1,10000);
        attributes.Add("lvl",level);
        attributes.Add("dmg",damage);
        attributes.Add("cri",critical);
        attributes.Add("tra",trading);
        attributes.Add("dod",dodging);
        attributes.Add("def",defense);
        attributes.Add("fdg",fireDamage);
        // Filling '_items'
        // Sword item
        List<Formula> swordFormulas=new List<Formula>();
        swordFormulas.Add(new Formula("DEX","DEX+3",1));
        swordFormulas.Add(new Formula("dmg","6",1));
        List<string> swordTags=new List<string>();
        swordTags.Add("Wereable");
        swordTags.Add("Weapon");
        swordTags.Add("Melee");
        ItemTemplate sword=new ItemTemplate("Sword","Common sword",swordTags,swordFormulas,null);
        _items.Add("Sword",sword);
        // Fire Sword item
        List<Formula> fireSwordFormulas=new List<Formula>();
        fireSwordFormulas.Add(new Formula("DEX","DEX+3",1));
        fireSwordFormulas.Add(new Formula("dmg","6",1));
        fireSwordFormulas.Add(new Formula("fdg","3",1));
        List<string> fireSwordTags=new List<string>();
        fireSwordTags.Add("Wereable");
        fireSwordTags.Add("Weapon");
        fireSwordTags.Add("Melee");
        ItemTemplate fireSword=new ItemTemplate("Fire sword","Fire sword",fireSwordTags,fireSwordFormulas,null);
        _items.Add("Fire sword",fireSword);
        // Axe item
        List<Formula> axeFormulas=new List<Formula>();
        axeFormulas.Add(new Formula("dmg","8",1));
        List<string> axeTags=new List<string>();
        axeTags.Add("Wereable");
        axeTags.Add("Weapon");
        axeTags.Add("Melee");
        ItemTemplate axe=new ItemTemplate("Axe","Common axe",axeTags,axeFormulas,null);
        _items.Add("Axe",axe);
        // Bow item
        List<Formula> bowFormulas=new List<Formula>();
        bowFormulas.Add(new Formula("dmg","2",1));
        List<string> bowTags=new List<string>();
        bowTags.Add("Wereable");
        bowTags.Add("Weapon");
        bowTags.Add("Ranged");
        ItemTemplate bow=new ItemTemplate("Bow","Common bow",bowTags,bowFormulas,null);
        _items.Add("Bow",bow);
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

    private void createAbbeyDemoDatabase()
        // Creates a Database for the Abbey demo. Called from constructor. It follows
        // the way an user would create things from editor.
        {        
        // Filling the tags and options
        fillTags();
        fillSlotOptions();
        // Filling core core 'attributes' (exist in every RPG)
        Attribute experience=new Attribute(true,"EXP","Experience","Experience acumulated",1,100);
        Attribute hp=new Attribute(true,"HPS","HealthPoints","Character health points",0,1000);
        Attribute mp=new Attribute(true,"MPS","MagicPoints","Character magic points",0,1000);
        attributes.Add("EXP",experience);
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
        Debug.Log("Abbey Demo database created!");
        }

    private void createEmptyDatabase()
        // Creates an empty Database with its core 'attributes'. Called from constructor
        {
        // Filling core core 'attributes' (exist in every RPG)
        Attribute experience=new Attribute(true,"EXP","Experience","Experience acumulated",1,100);
        Attribute hp=new Attribute(true,"HPS","HealthPoints","Character health points",0,1000);
        Attribute mp=new Attribute(true,"MPS","MagicPoints","Character magic points",0,1000);
        attributes.Add("EXP",experience);
        attributes.Add("HPS",hp);
        attributes.Add("MPS",mp);
        Debug.Log("Empty database created!");
        }

    private void fillTags()
        // Fills the database with the tags that can be attached to certain Template (PassiveTemplate, ItemTemplate, 
        // SpecTemplate). The tags for PassiveTemplate mean when the pasive happens and to whom is executed. The tags 
        // for ItemTemplate mean what the item is or where it is equipped. The tags for SpecTemplate ...TO DO...
        {
        // Create tags container      
        _tags=new Dictionary<string,Dictionary<string,string>>();
        // Create tags for PassiveTemplate
        _tags.Add("Passive",new Dictionary<string, string>());
        _tags["Passive"].Add("Permanent","Permanent");
        _tags["Passive"].Add("Turn","Turn");
        _tags["Passive"].Add("Time","Time");
        _tags["Passive"].Add("Self","Self");
        _tags["Passive"].Add("Enemy","Enemy");
        _tags["Passive"].Add("Friend","Friend");
        _tags["Passive"].Add("Enemy Group","Enemy Group");
        _tags["Passive"].Add("Friend Group","Friend Group");
        // Create tags for ItemTemplate
        _tags.Add("Item",new Dictionary<string, string>());
        _tags["Item"].Add("Wearable","Wearable");
        _tags["Item"].Add("Weapon","Weapon");
        _tags["Item"].Add("Melee","Melee");
        _tags["Item"].Add("Ranged","Ranged");
        _tags["Item"].Add("Cloth","Cloth");
        _tags["Item"].Add("Head","Head");
        _tags["Item"].Add("Chest","Chest");
        _tags["Item"].Add("Neck","Neck");
        _tags["Item"].Add("Arms","Arms");
        _tags["Item"].Add("Hands","Hands");
        _tags["Item"].Add("Finger","Finger");
        _tags["Item"].Add("Legs","Legs");
        _tags["Item"].Add("Addon","Addon");
        _tags["Item"].Add("Common","Common");
        _tags["Item"].Add("Special","Special");
        // Create tags for SpecTemplate
        _tags.Add("Specialization",new Dictionary<string, string>());
        // ...TO DO...
        }

    private void fillSlotOptions()
        // Fills the database with the options that are shown when you add a slot to a Template you
        // are editing (PassiveTemplate, ItemTemplate, SpecTemplate)
        { 
        // Create slots options container  
        _slotsOptions=new SlotsOpt();
        // Create slot options for PassiveTemplate
        _slotsOptions.PassiveOptions.Add("When",new Dictionary<string,string>());
        _slotsOptions.PassiveOptions["When"].Add("Permanent","Permanent");
        _slotsOptions.PassiveOptions["When"].Add("Turn","Turn");
        _slotsOptions.PassiveOptions["When"].Add("Time","Time");
        _slotsOptions.PassiveOptions.Add("To whom",new Dictionary<string,string>());
        _slotsOptions.PassiveOptions["To whom"].Add("Self","Self");
        _slotsOptions.PassiveOptions["To whom"].Add("Enemy","Enemy");
        _slotsOptions.PassiveOptions["To whom"].Add("Friend","Friend");
        _slotsOptions.PassiveOptions["To whom"].Add("Enemy Group","Enemy Group");
        _slotsOptions.PassiveOptions["To whom"].Add("Friend Group","Friend Group");
        // Create slot options for ItemTemplate
        // -Not needed (tags consulted)
        // Create slot options for SpecTemplate
        // TO DO
        }
    #endregion
    
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
        // Retrieves the list of names (identifieres) for every ItemTemplate stored in the database Dictionary '_items'
        List<string> itemIdentifiers=new List<string>();
        foreach (KeyValuePair<string,ItemTemplate> result in _items)
            {
            itemIdentifiers.Add(result.Value.nameId);
            }
        return itemIdentifiers;
        }

    public List<Formula> getItemFormulas(string nameId)
        {
        // Retrieves the list of formulas (Formula) for a ItemTemplate stored in the database Dictionary '_items'
        return _items[nameId].formulas;
        }

    public List<Template> getItemSlots(string nameId)
        {
        // Retrieves the list of slots (Template) for a ItemTemplate stored in the database Dictionary 'items'
        return _items[nameId].slots;
        }

    public void loadDatabase()
        // Retrieves the database from file to continue filling it. This file was (obviously) previously 
        // saved by the user when he was editing from Editor
        {
        // AssetDatabase.FindAssets("t:Mitipo");
        }

    public bool addModDelTemplate(Template t, string order)
        // Adds/Modifies/Deletes a Template (SpecTemplate, ItemTemplate, PassiveTemplate) to/from the database. 
        // The idea is that this method will be called from Editor when you CREATE/MODIFY/DELETE a Spec, Item 
        // or Passive
        {
        bool canAct=false;
        if (t is ItemTemplate)
            {
            if (order=="add")
                {
                canAct=_items.ContainsValue(t as ItemTemplate) ? false : true;
                if (canAct)
                    _items.Add(t.nameId,t as ItemTemplate);
                }
            else if (order=="mod")
                {
                canAct=_items.ContainsKey(t.nameId) ? true : false;
                if (canAct)
                    {
                    _items.Remove(t.nameId);
                    _items.Add(t.nameId,t as ItemTemplate);
                    }
                }
            else if (order=="del")
                {
                canAct=_items.ContainsKey(t.nameId) ? true : false;
                if (canAct)            
                    _items.Remove(t.nameId);
                }
            }
        else if (t is PassiveTemplate)
            {
            if (order=="add")
                {
                canAct=passives.ContainsValue(t as PassiveTemplate) ? false : true;
                if (canAct)
                    passives.Add(t.nameId,t as PassiveTemplate);
                }
            else if (order=="mod")
                {
                canAct=passives.ContainsKey(t.nameId) ? true : false;
                if (canAct)
                    {
                    passives.Remove(t.nameId);
                    passives.Add(t.nameId,t as PassiveTemplate);
                    }
                }
            else if (order=="del")
                {
                canAct=passives.ContainsKey(t.nameId) ? true : false;
                if (canAct)            
                    passives.Remove(t.nameId);
                }
            }
        else if (t is SpecTemplate)
            {
            if (order=="add")
                {
                canAct=specs.ContainsValue(t as SpecTemplate) ? false : true;
                if (canAct)
                    specs.Add(t.nameId,t as SpecTemplate);
                }
            else if (order=="mod")
                {
                canAct=specs.ContainsKey(t.nameId) ? true : false;
                if (canAct)
                    {
                    specs.Remove(t.nameId);
                    specs.Add(t.nameId,t as SpecTemplate);
                    }
                }
            else if (order=="del")
                {
                canAct=specs.ContainsKey(t.nameId) ? true : false;
                if (canAct)            
                    specs.Remove(t.nameId);
                }
            }
        return canAct;
        }

    public bool addTemplate(Template t)
        // Adds a Template (SpecTemplate, ItemTemplate, PassiveTemplate) to the database. The idea
        // is that this method will be called from Editor when you CREATE a Spec, Item or Passive
        {
        bool canAdd=false;
        if (t is ItemTemplate)
            {
            canAdd=_items.ContainsValue(t as ItemTemplate) ? false : true;
            if (canAdd)
                _items.Add(t.nameId,t as ItemTemplate);
            }
        else if (t is PassiveTemplate)
            {
            canAdd=passives.ContainsValue(t as PassiveTemplate) ? false : true;
            if (canAdd)
                passives.Add(t.nameId,t as PassiveTemplate);
            }
        else if (t is SpecTemplate)
            {
            canAdd=specs.ContainsValue(t as SpecTemplate) ? false : true;
            if (canAdd)
                specs.Add(t.nameId,t as SpecTemplate);
            }
        return canAdd;
        }

    public bool modifyTemplate(Template t)
        // Modifies a Template (SpecTemplate, ItemTemplate, PassiveTemplate) in the database. The idea
        // is that this method will be called from Editor when you EDIT a Spec, Item or Passive
        {
        bool canModify=false;
        if (t is ItemTemplate)
            {
            canModify=_items.ContainsKey(t.nameId) ? true : false;
            if (canModify)
                {
                _items.Remove(t.nameId);
                _items.Add(t.nameId,t as ItemTemplate);
                }
            }
        else if (t is PassiveTemplate)
            {
            canModify=passives.ContainsKey(t.nameId) ? true : false;
            if (canModify)
                {
                passives.Remove(t.nameId);
                passives.Add(t.nameId,t as PassiveTemplate);
                }
            }
        else if (t is SpecTemplate)
            {
            canModify=specs.ContainsKey(t.nameId) ? true : false;
            if (canModify)
                {
                specs.Remove(t.nameId);
                specs.Add(t.nameId,t as SpecTemplate);
                }
            }
        return canModify;
        }

    public bool deleteTemplate(Template t)
        {
        // Deletes a Template (SpecTemplate, ItemTemplate, PassiveTemplate) from the database. The idea
        // is that this method will be called from Editor when you DELETE a Spec, Item or Passive
        bool canDelete=false;
        if(t is ItemTemplate)
            {
            canDelete=_items.ContainsKey(t.nameId) ? true : false;
            if (canDelete)            
                _items.Remove(t.nameId);
            }
        else if(t is PassiveTemplate)
            {
            canDelete=passives.ContainsKey(t.nameId) ? true : false;
            if (canDelete)            
                passives.Remove(t.nameId);
            }
        else if(t is SpecTemplate)
            {
            canDelete=specs.ContainsKey(t.nameId) ? true : false;
            if (canDelete)            
                specs.Remove(t.nameId);
            }
        return false;
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

    public List<string> getAllowedSlots(Template t)
        {
        if (t is SpecTemplate)
            return new List<string>{ "Specialization", "Item", "Passive" };
        else if (t is ItemTemplate)
            return new List<string>{ "Item", "Passive" };
        else if (t is PassiveTemplate)
            return new List<string>{ "Passives" };
        else
            return new List<string>{ "Template not supported" };
        }

    public void print()
        {
        Debug.Log("I'm a database");
        }
    #endregion
    }