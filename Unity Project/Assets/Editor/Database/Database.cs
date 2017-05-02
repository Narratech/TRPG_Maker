using System.Collections.Generic;
using UnityEngine;

class Database
    #region Description
    // The 'Database' is the unique object (singleton) that stores all the info that can be used to create a game. This info are 
    // the attributes ('Attribute') and the templates ('Template'). In order to edit, relate and show this info correctly we need
    // extra structures which are basically tags
    #endregion
    #region Future fixes
    // This version just uses tags with no hierarchy, future version could use tags in hierarchy (trees) -> http://stackoverflow.com/questions/942053/why-is-there-no-treet-class-in-net
    #endregion
    {
    #region Attributes
    // Database singleton
    protected static Database instance;
    // Database elements
    Dictionary<string,Attribute> _attributes;  // Every Attribute in Database
    Dictionary<string,SpecTemplate> _specs;  // Every SpecTemplate in Database
    Dictionary<string,ItemTemplate> _items;  // Every ItemTemplate in Database
    Dictionary<string,PassiveTemplate> _passives;  // Every PassiveTemplate in Database
    // Info to show and relate database elements
    Dictionary<string,Dictionary<string,string>> _tags;  // Tags per template. Example: _tags["Item"]["Weapon"] returns "Weapon"
    List<string> _templateTypes;  // The types of 'Template' stored in database
    #endregion

    #region Constructor
    protected Database()
        {
        // Initializing structures
        _attributes=new Dictionary<string, Attribute>();
        _specs=new Dictionary<string, SpecTemplate>();
        _items=new Dictionary<string, ItemTemplate>();
        _passives=new Dictionary<string, PassiveTemplate>();
        _templateTypes=new List<string>{ "Specialization", "Item", "Passive" };
        // Creating database
        //createEmptyDatabase();
        createTestDatabase(); 
        //createAbbeyDemoDatabase();
        }
    #endregion

    #region Properties
    public static Database Instance
        {
        get { return instance == null ? instance = new Database() : instance; }
        }

    public Dictionary<string,Attribute> Attributes
        {
        get { return _attributes; }
        set { _attributes=value; }
        }

    public Dictionary<string,SpecTemplate> Specs
        {
        get { return _specs; }
        set { _specs=value; }
        }

    public Dictionary<string,ItemTemplate> Items
        {
        get { return _items; }
        set { _items=value; }
        }

    public Dictionary<string,PassiveTemplate> Passives
        {
        get { return _passives; }
        set { _passives=value; }
        }

    public List<string> Templates
        {
        get { return _templateTypes; }
        set { value=_templateTypes; }
        }

    public Dictionary<string,Dictionary<string,string>> Tags
        {
        get { return _tags; }
        set { value=_tags; }
        }
    #endregion

    #region Methods
    #region Database management: createExampleDatabase(), createAbbeyDemoDatabase(), createEmptyDatabase(), fillTags(), fillSlotOptions(), loadDatabase(), storeDatabase()
    protected void createEmptyDatabase()
        // Creates an empty Database with its core '_attributes'. Called from constructor
        {
        // Filling the tags and options
        fillTags("empty");
        // Filling core core '_attributes' (exist in every RPG)
        Attribute experience=new Attribute(true,"EXP","Experience","Experience acumulated",1,100000000);
        Attribute hp=new Attribute(true,"HPS","HealthPoints","Character health points",0,1000);
        Attribute mp=new Attribute(true,"MPS","MagicPoints","Character magic points",0,1000);
        _attributes.Add("EXP",experience);
        _attributes.Add("HPS",hp);
        _attributes.Add("MPS",mp);
        Debug.Log("Empty database created!");
        }

    private void createTestDatabase()
        // Creates an Example Database for testing purpouses. Typically called from constructor. It follows
        // the way an user would create things from editor.
        {
        // Filling the tags and options
        fillTags("test");
        // Filling core core '_attributes' (exist in every RPG)
        Attribute experience=new Attribute(true,"EXP","Experience","Experience acumulated",1,100000000);
        Attribute hp=new Attribute(true,"HPS","HealthPoints","Character health points",0,1000);
        Attribute mp=new Attribute(true,"MPS","MagicPoints","Character magic points",0,1000);
        _attributes.Add("EXP",experience);
        _attributes.Add("HPS",hp);
        _attributes.Add("MPS",mp);
        // Filling core '_attributes' (exist in every RPG slitghtly different)
        Attribute strength=new Attribute(true,"STR","Strength","Strength necessary to fisical strikes",1,100);
        Attribute intelligence=new Attribute(true,"INT","Intelligence","Intelligence to trade and find things",1,100);
        Attribute wisdom=new Attribute(true,"WIS","Wisdom","Wisdom helps you interacting and upgrading things",1,100);
        Attribute dexterity=new Attribute(true,"DEX","Dexterity","Dexterity helps in a lot of things",1,100);
        Attribute constitution=new Attribute(true,"CON","Constitution","Constitution is for wearing things",1,100);
        Attribute charisma=new Attribute(true,"CHA","Charisma","Charisma could save you without using a weapon",1,100);
        _attributes.Add("STR",strength);
        _attributes.Add("INT",intelligence);
        _attributes.Add("WIS",wisdom);
        _attributes.Add("DEX",dexterity);
        _attributes.Add("CON",constitution);
        _attributes.Add("CHA",charisma);
        // Filling derived '_attributes' 
        Attribute level=new Attribute(false,"lvl","Level","Character level",1,100);
        Attribute damage=new Attribute(false,"dmg","Damage","Base damage for a weapon",1,10000);
        Attribute critical=new Attribute(false,"cri","Critical","Chance of a crtical strike",1,10000);
        Attribute trading=new Attribute(false,"tra","Trading","Trading capabilities",1,10000);
        Attribute dodging=new Attribute(false,"dod","Dodging","Dodging probability",1,10000);
        Attribute defense=new Attribute(false,"def","Defense","Defense probability",1,10000);
        Attribute fireDamage=new Attribute(false,"fdg","Fire damage","Fire damage for a weapon",1,10000);
        _attributes.Add("lvl",level);
        _attributes.Add("dmg",damage);
        _attributes.Add("cri",critical);
        _attributes.Add("tra",trading);
        _attributes.Add("dod",dodging);
        _attributes.Add("def",defense);
        _attributes.Add("fdg",fireDamage);
        // Filling '_passives'
        /*
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
        */
        // Filling '_items'
        // Club item
        List<Formula> clubFormulas=new List<Formula>();
        clubFormulas.Add(new Formula("dmg","2",1));
        List<string> clubTags=new List<string>();
        clubTags.Add("Wearable");
        clubTags.Add("Weapon");
        clubTags.Add("Melee");
        clubTags.Add("Common");
        SlotsConfig clubSlotsConfig=new SlotsConfig();
        ItemTemplate club=new ItemTemplate("Club","Common melee weapon",clubTags,clubFormulas,clubSlotsConfig);
        _items.Add("Club",club);
        // Sword item
        List<Formula> swordFormulas=new List<Formula>();
        swordFormulas.Add(new Formula("DEX","DEX+3",1));
        swordFormulas.Add(new Formula("dmg","6",1));
        List<string> swordTags=new List<string>();
        swordTags.Add("Wearable");
        swordTags.Add("Weapon");
        swordTags.Add("Melee");
        swordTags.Add("Sword");
        SlotsConfig swordSlotsConfig=new SlotsConfig();
        List<ItemConfig> listSwordIc=new List<ItemConfig>();
        ItemConfig swordIc;
        swordIc.itemIds=new List<string>{ "Wearable", "Weapon", "Melee", "Addon" };
        swordIc.itemMask=4103;
        listSwordIc.Add(swordIc);
        swordSlotsConfig.ItemCfg=listSwordIc;
        ItemTemplate sword=new ItemTemplate("Sword","Normal sword",swordTags,swordFormulas,swordSlotsConfig);
        _items.Add("Sword",sword);
        // Fire Sword item
        List<Formula> fireSwordFormulas=new List<Formula>();
        fireSwordFormulas.Add(new Formula("DEX","DEX+3",1));
        fireSwordFormulas.Add(new Formula("dmg","6",1));
        fireSwordFormulas.Add(new Formula("fdg","3",1));
        List<string> fireSwordTags=new List<string>();
        fireSwordTags.Add("Wearable");
        fireSwordTags.Add("Weapon");
        fireSwordTags.Add("Melee");
        fireSwordTags.Add("Sword");
        SlotsConfig fireSwordSlotsConfig=new SlotsConfig();
        List<ItemConfig> listFireSwordIc=new List<ItemConfig>();
        ItemConfig fireSwordIc;
        fireSwordIc.itemIds=new List<string>{ "Wearable", "Weapon", "Melee", "Addon" };
        fireSwordIc.itemMask=4103;
        listFireSwordIc.Add(fireSwordIc);
        fireSwordSlotsConfig.ItemCfg=listFireSwordIc;
        List<Dictionary<string, string>> listFireSwordPc=new List<Dictionary<string, string>>();
        Dictionary<string,string> fireSwordPc=new Dictionary<string, string>();
        fireSwordPc.Add("When","Turn");
        fireSwordPc.Add("To whom","Enemy");
        listFireSwordPc.Add(fireSwordPc);
        fireSwordSlotsConfig.PassiveCfg=listFireSwordPc;
        ItemTemplate fireSword=new ItemTemplate("Fire sword","Fire sword",fireSwordTags,fireSwordFormulas,fireSwordSlotsConfig);
        _items.Add("Fire sword",fireSword);
        // Axe item
        List<Formula> axeFormulas=new List<Formula>();
        axeFormulas.Add(new Formula("dmg","8",1));
        List<string> axeTags=new List<string>();
        axeTags.Add("Wearable");
        axeTags.Add("Weapon");
        axeTags.Add("Melee");
        axeTags.Add("Axe");
        SlotsConfig axeSlotsConfig=new SlotsConfig();
        List<ItemConfig> listAxeIc=new List<ItemConfig>();
        ItemConfig axeIc;
        axeIc.itemIds=new List<string>{ "Wearable", "Weapon", "Melee", "Addon" };
        axeIc.itemMask=4103;
        listAxeIc.Add(axeIc);
        axeSlotsConfig.ItemCfg=listAxeIc;
        ItemTemplate axe=new ItemTemplate("Axe","Normal axe",axeTags,axeFormulas,axeSlotsConfig);
        _items.Add("Axe",axe);
        // Bow item
        List<Formula> bowFormulas=new List<Formula>();
        bowFormulas.Add(new Formula("dmg","2",1));
        List<string> bowTags=new List<string>();
        bowTags.Add("Wearable");
        bowTags.Add("Weapon");
        bowTags.Add("Ranged");
        bowTags.Add("Bow");
        SlotsConfig bowSlotsConfig=new SlotsConfig();
        List<ItemConfig> listBowIc=new List<ItemConfig>();
        ItemConfig bowIc;
        bowIc.itemIds=new List<string>{ "Wearable", "Weapon", "Ranged", "Addon" };
        bowIc.itemMask=4107;
        listBowIc.Add(bowIc);
        bowSlotsConfig.ItemCfg=listBowIc;
        ItemTemplate bow=new ItemTemplate("Bow","Normal bow",bowTags,bowFormulas,bowSlotsConfig);
        _items.Add("Bow",bow);
        // Filling basic 'specs'
        // Human spec
        List<Formula> humanFormulas=new List<Formula>();
        humanFormulas.Add(new Formula("cri","STR*3",1));
        humanFormulas.Add(new Formula("tra","INT*2+WIS*3+CHA",2));
        SlotsConfig humanSlotsConfig=new SlotsConfig();
        List<ItemConfig> listHumanIc=new List<ItemConfig>();
        ItemConfig humanIc1;
        humanIc1.itemIds=new List<string>{ "Wearable", "Weapon", "Melee" };
        humanIc1.itemMask=7;
        listHumanIc.Add(humanIc1);
        ItemConfig humanIc2;
        humanIc2.itemIds=new List<string>{ "Wearable", "Weapon", "Ranged" };
        humanIc2.itemMask=11;
        listHumanIc.Add(humanIc2);
        ItemConfig humanIc3;
        humanIc3.itemIds=new List<string>{ "Wearable", "Cloth", "Head" };
        humanIc3.itemMask=49;
        listHumanIc.Add(humanIc3);
        ItemConfig humanIc4;
        humanIc4.itemIds=new List<string>{ "Wearable", "Cloth", "Chest" };
        humanIc4.itemMask=81;
        listHumanIc.Add(humanIc4);
        ItemConfig humanIc5;
        humanIc5.itemIds=new List<string>{ "Wearable", "Cloth", "Neck" };
        humanIc5.itemMask=145;
        listHumanIc.Add(humanIc5);
        ItemConfig humanIc6;
        humanIc6.itemIds=new List<string>{ "Wearable", "Cloth", "Arms" };
        humanIc6.itemMask=273;
        listHumanIc.Add(humanIc6);
        ItemConfig humanIc7;
        humanIc7.itemIds=new List<string>{ "Wearable", "Cloth", "Hands" };
        humanIc7.itemMask=529;
        listHumanIc.Add(humanIc7);
        ItemConfig humanIc8;
        humanIc8.itemIds=new List<string>{ "Wearable", "Cloth", "Finger" };
        humanIc8.itemMask=1041;
        listHumanIc.Add(humanIc8);
        ItemConfig humanIc9;
        humanIc9.itemIds=new List<string>{ "Wearable", "Cloth", "Feet" };
        humanIc9.itemMask=2065;
        listHumanIc.Add(humanIc9);
        ItemConfig humanIc10;
        humanIc10.itemIds=new List<string>{ "Wearable", "Cloth", "Legs" };
        humanIc10.itemMask=4113;
        listHumanIc.Add(humanIc10);
        humanSlotsConfig.ItemCfg=listHumanIc;
        SpecTemplate human=new SpecTemplate("Human","Humans are flesh and bones",true,humanFormulas,humanSlotsConfig);
        _specs.Add("Human",human);
        // Elf spec
        List<Formula> elfFormulas=new List<Formula>();
        elfFormulas.Add(new Formula("cri","cri+1",1));
        elfFormulas.Add(new Formula("tra","INT*2+WIS*3+CHA",2));
        SlotsConfig elfSlotsConfig=new SlotsConfig();
        List<ItemConfig> listElfIc=new List<ItemConfig>();
        ItemConfig elfIc1;
        elfIc1.itemIds=new List<string>{ "Wearable", "Weapon", "Melee" };
        elfIc1.itemMask=7;
        listElfIc.Add(elfIc1);
        ItemConfig elfIc2;
        elfIc2.itemIds=new List<string>{ "Wearable", "Weapon", "Ranged" };
        elfIc2.itemMask=11;
        listElfIc.Add(elfIc2);
        ItemConfig elfIc3;
        elfIc3.itemIds=new List<string>{ "Wearable", "Cloth", "Head" };
        elfIc3.itemMask=49;
        listElfIc.Add(elfIc3);
        ItemConfig elfIc4;
        elfIc4.itemIds=new List<string>{ "Wearable", "Cloth", "Chest" };
        elfIc4.itemMask=81;
        listElfIc.Add(elfIc4);
        ItemConfig elfIc5;
        elfIc5.itemIds=new List<string>{ "Wearable", "Cloth", "Neck" };
        elfIc5.itemMask=145;
        listElfIc.Add(elfIc5);
        ItemConfig elfIc6;
        elfIc6.itemIds=new List<string>{ "Wearable", "Cloth", "Arms" };
        elfIc6.itemMask=273;
        listElfIc.Add(elfIc6);
        ItemConfig elfIc7;
        elfIc7.itemIds=new List<string>{ "Wearable", "Cloth", "Hands" };
        elfIc7.itemMask=529;
        listElfIc.Add(elfIc7);
        ItemConfig elfIc8;
        elfIc8.itemIds=new List<string>{ "Wearable", "Cloth", "Finger" };
        elfIc8.itemMask=1041;
        listElfIc.Add(elfIc8);
        ItemConfig elfIc9;
        elfIc9.itemIds=new List<string>{ "Wearable", "Cloth", "Feet" };
        elfIc9.itemMask=2065;
        listElfIc.Add(elfIc9);
        ItemConfig elfIc10;
        elfIc10.itemIds=new List<string>{ "Wearable", "Cloth", "Legs" };
        elfIc10.itemMask=4113;
        listElfIc.Add(elfIc10);
        elfSlotsConfig.ItemCfg=listElfIc;
        SpecTemplate elf=new SpecTemplate("Elf","Elves have pointy ears",true,elfFormulas,elfSlotsConfig);
        _specs.Add("Elf",elf);
        // Orc spec
        List<Formula> orcFormulas=new List<Formula>();
        orcFormulas.Add(new Formula("cri","STR*10",1));
        orcFormulas.Add(new Formula("tra","INT*2+WIS*4+CHA*2",2));
        SlotsConfig orcSlotsConfig=new SlotsConfig();
        List<ItemConfig> listOrcIc=new List<ItemConfig>();
        ItemConfig orcIc1;
        orcIc1.itemIds=new List<string>{ "Wearable", "Weapon", "Melee" };
        orcIc1.itemMask=7;
        listOrcIc.Add(orcIc1);
        ItemConfig orcIc2;
        orcIc2.itemIds=new List<string>{ "Wearable", "Weapon", "Ranged" };
        orcIc2.itemMask=11;
        listOrcIc.Add(orcIc2);
        ItemConfig orcIc3;
        orcIc3.itemIds=new List<string>{ "Wearable", "Cloth", "Head" };
        orcIc3.itemMask=49;
        listOrcIc.Add(orcIc3);
        ItemConfig orcIc4;
        orcIc4.itemIds=new List<string>{ "Wearable", "Cloth", "Chest" };
        orcIc4.itemMask=81;
        listOrcIc.Add(orcIc4);
        ItemConfig orcIc5;
        orcIc5.itemIds=new List<string>{ "Wearable", "Cloth", "Neck" };
        orcIc5.itemMask=145;
        listOrcIc.Add(orcIc5);
        ItemConfig orcIc6;
        orcIc6.itemIds=new List<string>{ "Wearable", "Cloth", "Arms" };
        orcIc6.itemMask=273;
        listOrcIc.Add(orcIc6);
        ItemConfig orcIc7;
        orcIc7.itemIds=new List<string>{ "Wearable", "Cloth", "Hands" };
        orcIc7.itemMask=529;
        listOrcIc.Add(orcIc7);
        ItemConfig orcIc8;
        orcIc8.itemIds=new List<string>{ "Wearable", "Cloth", "Finger" };
        orcIc8.itemMask=1041;
        listOrcIc.Add(orcIc8);
        ItemConfig orcIc9;
        orcIc9.itemIds=new List<string>{ "Wearable", "Cloth", "Feet" };
        orcIc9.itemMask=2065;
        listOrcIc.Add(orcIc9);
        ItemConfig orcIc10;
        orcIc10.itemIds=new List<string>{ "Wearable", "Cloth", "Legs" };
        orcIc10.itemMask=4113;
        listOrcIc.Add(orcIc10);
        orcSlotsConfig.ItemCfg=listOrcIc;
        SpecTemplate orc=new SpecTemplate("Orc","Orcs are typically green and ugly",true,orcFormulas,orcSlotsConfig);
        _specs.Add("Orc",orc);
        // Filling derived 'specs'
        // Swordman spec
        List<Formula> swordmanFormulas=new List<Formula>();
        swordmanFormulas.Add(new Formula("dod","CON*3",1));
        SlotsConfig swordmanSlotsConfig=new SlotsConfig();
        List<ItemConfig> listSwordmanIc=new List<ItemConfig>();
        ItemConfig swordmanIc1;
        swordmanIc1.itemIds=new List<string>{ "Wearable", "Weapon", "Melee", "Sword" };
        swordmanIc1.itemMask=65543;
        listSwordmanIc.Add(swordmanIc1);
        swordmanSlotsConfig.ItemCfg=listSwordmanIc;
        SpecTemplate swordman=new SpecTemplate("Swordman","Swordman is a human/elf spec in which they can use swords",false,swordmanFormulas,swordmanSlotsConfig);
        _specs.Add("Swordman",swordman);
        Debug.Log("Test database created!");
        }

    private void createAbbeyDemoDatabase()
        /////////
        // WIP //
        /////////
        // Creates a Database for the Abbey demo. Called from constructor. It follows
        // the way an user would create things from editor.
        {        
        // Filling the tags and options
        fillTags("abbeyDemo");
        // Filling core core '_attributes' (exist in every RPG)
        Attribute experience=new Attribute(true,"EXP","Experience","Experience acumulated",1,100000000);
        Attribute hp=new Attribute(true,"HPS","HealthPoints","Character health points",0,1000);
        Attribute mp=new Attribute(true,"MPS","MagicPoints","Character magic points",0,1000);
        _attributes.Add("EXP",experience);
        _attributes.Add("HPS",hp);
        _attributes.Add("MPS",mp);
        // Filling core '_attributes' (exist in every RPG slitghtly different)
        Attribute strength=new Attribute(true,"STR","Strength","Strength necessary to fisical strikes",1,100);
        Attribute intelligence=new Attribute(true,"INT","Intelligence","Intelligence to trade and find things",1,100);
        Attribute wisdom=new Attribute(true,"WIS","Wisdom","Wisdom helps you interacting and upgrading things",1,100);
        Attribute dexterity=new Attribute(true,"DEX","Dexterity","Dexterity helps in a lot of things",1,100);
        Attribute constitution=new Attribute(true,"CON","Constitution","Constitution is for wearing things",1,100);
        Attribute charisma=new Attribute(true,"CHA","Charisma","Charisma could save you without using a weapon",1,100);
        _attributes.Add("STR",strength);
        _attributes.Add("INT",intelligence);
        _attributes.Add("WIS",wisdom);
        _attributes.Add("DEX",dexterity);
        _attributes.Add("CON",constitution);
        _attributes.Add("CHA",charisma);
        Debug.Log("Abbey Demo database created!");
        }

    private void fillTags(string option)
        // Fills the database with the tags that can be attached to certain Template (PassiveTemplate, ItemTemplate, 
        // SpecTemplate). The tags for PassiveTemplate mean when the pasive happens and to whom is executed. The tags 
        // for ItemTemplate mean what the item is or where it is equipped. The tags for SpecTemplate // TO DO //
        {
        if (option=="empty")
            {
            // Create tags container      
            _tags=new Dictionary<string,Dictionary<string,string>>();
            // Create tags for PassiveTemplate
            _tags.Add("PassiveWhen",new Dictionary<string, string>());
            _tags["PassiveWhen"].Add("Permanent","Permanent");
            _tags["PassiveWhen"].Add("Turn","Turn");
            _tags["PassiveWhen"].Add("Time","Time");
            _tags.Add("PassiveToWhom",new Dictionary<string, string>());
            _tags["PassiveToWhom"].Add("Self","Self");
            _tags["PassiveToWhom"].Add("Enemy","Enemy");
            _tags["PassiveToWhom"].Add("Friend","Friend");
            _tags["PassiveToWhom"].Add("Enemy Group","Enemy Group");
            _tags["PassiveToWhom"].Add("Friend Group","Friend Group");
            // Create tags for ItemTemplate
            _tags.Add("Item",new Dictionary<string, string>());
            // Create tags for SpecTemplate
            _tags.Add("Specialization",new Dictionary<string, string>());
            }
        else if (option=="test")
            {
            // Create tags container      
            _tags=new Dictionary<string,Dictionary<string,string>>();
            // Create tags for PassiveTemplate
            _tags.Add("PassiveWhen",new Dictionary<string, string>());
            _tags["PassiveWhen"].Add("Permanent","Permanent");
            _tags["PassiveWhen"].Add("Turn","Turn");
            _tags["PassiveWhen"].Add("Time","Time");
            _tags.Add("PassiveToWhom",new Dictionary<string, string>());
            _tags["PassiveToWhom"].Add("Self","Self");
            _tags["PassiveToWhom"].Add("Enemy","Enemy");
            _tags["PassiveToWhom"].Add("Friend","Friend");
            _tags["PassiveToWhom"].Add("Enemy Group","Enemy Group");
            _tags["PassiveToWhom"].Add("Friend Group","Friend Group");
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
            _tags["Item"].Add("Feet","Feet");
            _tags["Item"].Add("Addon","Addon");
            _tags["Item"].Add("Common","Common");
            _tags["Item"].Add("Special","Special");
            _tags["Item"].Add("Sword","Sword");
            _tags["Item"].Add("Axe","Axe");
            _tags["Item"].Add("Bow","Bow");
            // Create tags for SpecTemplate
            // -Not needed
            //_tags.Add("Specialization",new Dictionary<string, string>());
            //_tags["Specialization"].Add("Swordman","Swordman");
            //_tags["Specialization"].Add("Thief","Thief");
            }
        else if (option=="abbeyDemo")
            {
            // Create tags container      
            _tags=new Dictionary<string,Dictionary<string,string>>();
            // Create tags for PassiveTemplate
            _tags.Add("PassiveWhen",new Dictionary<string, string>());
            _tags["PassiveWhen"].Add("Permanent","Permanent");
            _tags["PassiveWhen"].Add("Turn","Turn");
            _tags["PassiveWhen"].Add("Time","Time");
            _tags.Add("PassiveToWhom",new Dictionary<string, string>());
            _tags["PassiveToWhom"].Add("Self","Self");
            _tags["PassiveToWhom"].Add("Enemy","Enemy");
            _tags["PassiveToWhom"].Add("Friend","Friend");
            _tags["PassiveToWhom"].Add("Enemy Group","Enemy Group");
            _tags["PassiveToWhom"].Add("Friend Group","Friend Group");
            // Create tags for ItemTemplate
            _tags.Add("Item",new Dictionary<string, string>());
            //
            //
            // Create tags for SpecTemplate
            _tags.Add("Specialization",new Dictionary<string, string>());
            //
            //
            }
        }

    public void loadDatabase()
        ///////////
        // TO DO //
        ///////////
        // Retrieves the database from file to continue filling it. This file was (obviously) previously 
        // saved by the user when he was editing from Editor
        {
        // AssetDatabase.FindAssets("t:Mitipo");
        }

    public void storeDatabase()
        ///////////
        // TO DO //
        ///////////
        // Stores the database into a file to continue filling it later
        {
        // AssetDatabase.FindAssets("t:Mitipo");
        }
    #endregion

    #region Typical database operations: get*(), addAttribute(), deleteAttribute(), addModDelTemplate()
    public List<string> getAttribIdentifiers()
        // Retrieves the list of identifiers for every Attribute stored in the database Dictionary '_attributes'
        {
        List<string> attribIdentifiers=new List<string>();
        foreach (KeyValuePair<string,Attribute> result in _attributes)
            {
            attribIdentifiers.Add(result.Value.id);
            }
        return attribIdentifiers;
        }

    public List<string> getItemNames()
        // Retrieves the list of names (identifieres) for every ItemTemplate stored in the database Dictionary '_items'
        {
        List<string> itemIdentifiers=new List<string>();
        foreach (KeyValuePair<string,ItemTemplate> result in _items)
            {
            itemIdentifiers.Add(result.Value.NameId);
            }
        return itemIdentifiers;
        }

    public List<string> getSpecNames()
        // Retrieves the list of names (identifieres) for every SpecTemplate stored in the database Dictionary '_specs'
        {
        List<string> specIdentifiers=new List<string>();
        foreach (KeyValuePair<string,SpecTemplate> result in _specs)
            {
            specIdentifiers.Add(result.Value.NameId);
            }
        return specIdentifiers;
        }

    public List<string> getPassiveNames()
        // Retrieves the list of names (identifieres) for every PassiveTemplate stored in the database Dictionary '_passives'
        {
        List<string> passiveIdentifiers=new List<string>();
        foreach (KeyValuePair<string,PassiveTemplate> result in _passives)
            {
            passiveIdentifiers.Add(result.Value.NameId);
            }
        return passiveIdentifiers;
        }

    public List<Formula> getItemFormulas(string nameId)
        // Retrieves the list of formulas (Formula) for a ItemTemplate stored in the database Dictionary '_items'
        {
        return _items[nameId].Formulas;
        }

    public List<Formula> getSpecFormulas(string nameId)
        // Retrieves the list of formulas (Formula) for a SpecTemplate stored in the database Dictionary '_specs'
        {
        return _specs[nameId].Formulas;
        }

    public List<string> getAllowedSlots(Template t)
        // Returns a list with the type of slots the Template t can use. For example an ItemTemplate can only use
        // ItemTemplate ("Item") and PassiveTemplate ("Passive") in its slots 
        {
        if (t is SpecTemplate)
            return new List<string>{ "Specialization", "Item", "Passive" };
        else if (t is ItemTemplate)
            return new List<string>{ "Item", "Passive" };
        else if (t is PassiveTemplate)
            return new List<string>{ "Passive" };
        else
            return new List<string>{ "Template not supported" };
        }

    public bool addAttribute(Attribute atr)
        // Adds a new Attribute to the database. Since every character in the game will need these
        // we can specify some as 'core' _attributes which everyone will have. _attributes which are no
        // 'core' may be limited to certain kind of characters (Spec attached) or simply derived _attributes
        // that everyone can posess but may never use
        {
        bool canAdd=_attributes.ContainsValue(atr) ? false : true;
        if (canAdd)
            _attributes.Add(atr.id,atr);               
        return canAdd;
        }

    public bool deleteAttribute(string atr)
        // Delete an existing Attribute from the database if this attribute exist. KEEP IN MIND that deleting an attribute
        // COULD cause problems if it has dependencies 
        {
        bool canDelete=_attributes.ContainsKey(atr) ? true : false;
        if (canDelete)
            _attributes.Remove(atr);
        return canDelete;
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
                    _items.Add(t.NameId,t as ItemTemplate);
                }
            else if (order=="mod")
                {
                canAct=_items.ContainsKey(t.NameId) ? true : false;
                if (canAct)
                    {
                    _items.Remove(t.NameId);
                    _items.Add(t.NameId,t as ItemTemplate);
                    }
                }
            else if (order=="del")
                {
                canAct=_items.ContainsKey(t.NameId) ? true : false;
                if (canAct)            
                    _items.Remove(t.NameId);
                }
            }
        else if (t is PassiveTemplate)
            {
            if (order=="add")
                {
                canAct=_passives.ContainsValue(t as PassiveTemplate) ? false : true;
                if (canAct)
                    _passives.Add(t.NameId,t as PassiveTemplate);
                }
            else if (order=="mod")
                {
                canAct=_passives.ContainsKey(t.NameId) ? true : false;
                if (canAct)
                    {
                    _passives.Remove(t.NameId);
                    _passives.Add(t.NameId,t as PassiveTemplate);
                    }
                }
            else if (order=="del")
                {
                canAct=_passives.ContainsKey(t.NameId) ? true : false;
                if (canAct)            
                    _passives.Remove(t.NameId);
                }
            }
        else if (t is SpecTemplate)
            {
            if (order=="add")
                {
                canAct=_specs.ContainsValue(t as SpecTemplate) ? false : true;
                if (canAct)
                    _specs.Add(t.NameId,t as SpecTemplate);
                }
            else if (order=="mod")
                {
                canAct=_specs.ContainsKey(t.NameId) ? true : false;
                if (canAct)
                    {
                    _specs.Remove(t.NameId);
                    _specs.Add(t.NameId,t as SpecTemplate);
                    }
                }
            else if (order=="del")
                {
                canAct=_specs.ContainsKey(t.NameId) ? true : false;
                if (canAct)            
                    _specs.Remove(t.NameId);
                }
            }
        return canAct;
        }
    #endregion
    #endregion
    }