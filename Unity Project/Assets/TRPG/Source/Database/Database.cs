using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Database : ScriptableObject, ISerializationCallbackReceiver
    #region Description
    // The 'Database' is the unique object (singleton) that stores all the info that can be used to create a game. This info includes 
    // attributes, templates (specializations, items and passives). In order to edit, relate and show this info correctly we need
    // extra structures which are basically tags
    #endregion
    {
    #region Attributes
    // Database singleton
    protected static Database _instance;
    // Database elements
    Dictionary<string, AttributeTRPG> _Attributes;  // Every AttributeTRPG in Database
    Dictionary<string, SpecTemplate> _specs;  // Every SpecTemplate in Database
    Dictionary<string, ItemTemplate> _items;  // Every ItemTemplate in Database
    Dictionary<string, PassiveTemplate> _passives;  // Every PassiveTemplate in Database
    // Info to show and relate database elements
    Dictionary<string, List<string>> _tags;  // Tags per template
    List<string> _templateTypes;  // The types of 'Template' stored in database
    // Serializable
    [SerializeField]
    private List<AttributeTRPG> _AttributesList;  // Every AttributeTRPG in Database in List format
    [SerializeField]
    private List<string> _specsKeys;  // Every SpecTemplate in Database in List format
    [SerializeField]
    private List<SpecTemplate> _specsValues;  // Every SpecTemplate in Database in List format
    [SerializeField]
    private List<string> _itemsKeys;  // Every ItemTemplate in Database in List format
    [SerializeField]
    private List<ItemTemplate> _itemsValues;  // Every ItemTemplate in Database in List format
    [SerializeField]
    private List<string> _passivesKeys;  // Every PassiveTemplate in Database in List format
    [SerializeField]
    private List<PassiveTemplate> _passivesValues;  // Every PassiveTemplate in Database in List format
    [SerializeField]
    private List<string> _tagKeys;
    [SerializeField]
    private List<ListWrapper> _tagValues;
    #endregion

    #region Constructor
    protected Database()
        {
        _Attributes=new Dictionary<string, AttributeTRPG>();
        _specs=new Dictionary<string, SpecTemplate>();
        _items=new Dictionary<string, ItemTemplate>();
        _passives=new Dictionary<string, PassiveTemplate>();
        _tags=new Dictionary<string, List<string>>();
        _templateTypes=new List<string> { "Specialization", "Item", "Passive" };
        //Debug.Log("Database Instance created!");
        }
    #endregion

    #region Properties
    public static Database Instance
        {
        //get { return _instance == null ? _instance = new Database() : _instance; }
        get { return _instance == null ? _instance = UnityEditor.AssetDatabase.LoadAssetAtPath<Database>("Assets/TRPG/database.asset") : _instance; }
        }

    public Dictionary<string, AttributeTRPG> Attributes
        {
        get { return _Attributes; }
        set { _Attributes = value; }
        }

    public Dictionary<string, SpecTemplate> Specs
        {
        get { return _specs; }
        set { _specs = value; }
        }

    public Dictionary<string, ItemTemplate> Items
        {
        get { return _items; }
        set { _items = value; }
        }

    public Dictionary<string, PassiveTemplate> Passives
        {
        get { return _passives; }
        set { _passives = value; }
        }

    public List<string> Templates
        {
        get { return _templateTypes; }
        set { value = _templateTypes; }
        }

    public Dictionary<string, List<string>> Tags
        {
        get { return _tags; }
        set { value = _tags; }
        }
    #endregion

    #region Methods
    #region Database management: creation, load, store and serialization of database
    public void createEmptyDatabase()
        // Creates an empty Database with its basic '_Attributes'. Called from constructor
        {
        #region Tags
        // Filling the tags and options
        fillTags("empty");
        #endregion
        #region Attributes
        // Filling basic '_Attributes' (exist in every RPG)
        AttributeTRPG experience = new AttributeTRPG(true, "EXP", "Experience", "Experience acumulated", 0, 100000000);
        AttributeTRPG hp = new AttributeTRPG(true, "HPS", "HealthPoints", "Character health points", 0, 1000);
        AttributeTRPG mp = new AttributeTRPG(true, "MPS", "MagicPoints", "Character magic points", 0, 1000);
        _Attributes.Add("EXP", experience);
        _Attributes.Add("HPS", hp);
        _Attributes.Add("MPS", mp);
        #endregion
        UnityEditor.AssetDatabase.SaveAssets();
        //Debug.Log("Database filled as 'Empty Database'!");
        }

    public void createDemoDatabase()
        // Creates a Database for the TFG demo. Called from constructor. It follows
        // the way an user would create things from editor
        {
        #region Tags
        // Filling the tags and options
        fillTags("demo");
        #endregion
        #region Attributes
        // Filling basic '_Attributes' (exist in every RPG)
        AttributeTRPG experience = new AttributeTRPG(true, "EXP", "Experience", "Experience acumulated", 0, 100000000);
        AttributeTRPG hp = new AttributeTRPG(true, "HPS", "HealthPoints", "Character health points", 0, 1000);
        AttributeTRPG mp = new AttributeTRPG(true, "MPS", "MagicPoints", "Character magic points", 0, 1000);
        _Attributes.Add("EXP", experience);
        _Attributes.Add("HPS", hp);
        _Attributes.Add("MPS", mp);
        // Filling core '_Attributes' (exist in every RPG slitghtly different)
        AttributeTRPG strength = new AttributeTRPG(true, "STR", "Strength", "Strength necessary to fisical strikes", 1, 100);
        AttributeTRPG intelligence = new AttributeTRPG(true, "INT", "Intelligence", "Intelligence to trade and find things", 1, 100);
        AttributeTRPG wisdom = new AttributeTRPG(true, "WIS", "Wisdom", "Wisdom helps you interacting and upgrading things", 1, 100);
        AttributeTRPG dexterity = new AttributeTRPG(true, "DEX", "Dexterity", "Dexterity helps in a lot of things", 1, 100);
        AttributeTRPG constitution = new AttributeTRPG(true, "CON", "Constitution", "Constitution is for wearing things", 1, 100);
        AttributeTRPG charisma = new AttributeTRPG(true, "CHA", "Charisma", "Charisma could save you without using a weapon", 1, 100);
        _Attributes.Add("STR", strength);
        _Attributes.Add("INT", intelligence);
        _Attributes.Add("WIS", wisdom);
        _Attributes.Add("DEX", dexterity);
        _Attributes.Add("CON", constitution);
        _Attributes.Add("CHA", charisma);
        // Filling derived '_Attributes' 
        AttributeTRPG level = new AttributeTRPG(false, "lvl", "Level", "Character level", 1, 100);
        AttributeTRPG damage = new AttributeTRPG(false, "dmg", "Damage", "Base damage points", 1, 10000);
        AttributeTRPG fireDamage = new AttributeTRPG(false, "fdg", "Fire damage", "Fire damage points", 1, 10000);
        AttributeTRPG iceDamage = new AttributeTRPG(false, "idg", "Ice damage", "Ice damage points", 1, 10000);
        AttributeTRPG defense = new AttributeTRPG(false, "def", "Defense", "Defense points", 1, 10000);
        AttributeTRPG fireDefense = new AttributeTRPG(false, "fdf", "Fire defense", "Fire defense points", 1, 10000);
        AttributeTRPG iceDefense = new AttributeTRPG(false, "idf", "Ice defense", "Ice defense points", 1, 10000);
        _Attributes.Add("lvl", level);
        _Attributes.Add("dmg", damage);
        _Attributes.Add("fdg", fireDamage);
        _Attributes.Add("idg", iceDamage);
        _Attributes.Add("def", defense);
        _Attributes.Add("fdf", fireDefense);
        _Attributes.Add("idf", iceDefense);
        #endregion
        #region Passives
        // Extra Attack passive
        List<Formula> extraAttackFormulas = new List<Formula>();
        extraAttackFormulas.Add(new Formula("dmg", "6", 1));
        List<string> extraAttackTags = new List<string>();
        extraAttackTags.Add("Turn");
        extraAttackTags.Add("Enemy");
        SlotsConfig extraAttackSlotsConfig = new SlotsConfig();
        PassiveTemplate extraAttack = ScriptableObject.CreateInstance<PassiveTemplate>().Init("Extra Attack", "The character will make an extra attack at the end of every turn", extraAttackTags, extraAttackFormulas, extraAttackSlotsConfig);
        _passives.Add("Extra Attack", extraAttack);
        extraAttack.store(this);
        // Regeneration passive
        List<Formula> regenerationFormulas = new List<Formula>();
        regenerationFormulas.Add(new Formula("HPS", "10", 1));
        List<string> regenerationTags = new List<string>();
        regenerationTags.Add("Turn");
        regenerationTags.Add("Self");
        SlotsConfig regenerationSlotsConfig = new SlotsConfig();
        PassiveTemplate regeneration = ScriptableObject.CreateInstance<PassiveTemplate>().Init("Regeneration", "The character will increase its HPs at the end of every turn", regenerationTags, regenerationFormulas, regenerationSlotsConfig);
        _passives.Add("Regeneration", regeneration);
        regeneration.store(this);
        #endregion
        #region Shields
        // Shield item
        List<Formula> shieldFormulas = new List<Formula>();
        shieldFormulas.Add(new Formula("def", "5", 1));
        List<string> shieldTags = new List<string>();
        shieldTags.Add("Wearable");
        shieldTags.Add("Shield");
        shieldTags.Add("No Magic");
        shieldTags.Add("Size For All");
        shieldTags.Add("No Weak");
        SlotsConfig shieldSlotsConfig = new SlotsConfig();
        ItemTemplate shield = ScriptableObject.CreateInstance<ItemTemplate>().Init("Shield", "Common shield", shieldTags, shieldFormulas, shieldSlotsConfig);
        _items.Add("Shield", shield);
        shield.store(this);
        #endregion
        #region Weapons
        // Filling '_items'
        // Club item
        List<Formula> clubFormulas = new List<Formula>();
        clubFormulas.Add(new Formula("dmg", "2", 1));
        List<string> clubTags = new List<string>();
        clubTags.Add("Wearable");
        clubTags.Add("Weapon");
        clubTags.Add("Melee");
        clubTags.Add("No Addon");
        clubTags.Add("No Magic");
        clubTags.Add("No Weapon Spec");
        clubTags.Add("Size For All");
        clubTags.Add("One Hand");
        SlotsConfig clubSlotsConfig = new SlotsConfig();
        ItemTemplate club = ScriptableObject.CreateInstance<ItemTemplate>().Init("Club", "Common melee weapon", clubTags, clubFormulas, clubSlotsConfig);
        _items.Add("Club", club);
        club.store(this);
        // Big Club item
        List<Formula> bigClubFormulas = new List<Formula>();
        bigClubFormulas.Add(new Formula("dmg", "5", 1));
        List<string> bigClubTags = new List<string>();
        bigClubTags.Add("Wearable");
        bigClubTags.Add("Weapon");
        bigClubTags.Add("Melee");
        bigClubTags.Add("No Addon");
        bigClubTags.Add("No Magic");
        bigClubTags.Add("No Weapon Spec");
        bigClubTags.Add("Size For All");
        bigClubTags.Add("Two Hand");
        SlotsConfig bigClubSlotsConfig = new SlotsConfig();
        ItemTemplate bigClub = ScriptableObject.CreateInstance<ItemTemplate>().Init("Big Club", "Common melee weapon for two hands", bigClubTags, bigClubFormulas, bigClubSlotsConfig);
        _items.Add("Big Club", bigClub);
        bigClub.store(this);
        // Stone item
        List<Formula> stoneFormulas = new List<Formula>();
        stoneFormulas.Add(new Formula("dmg", "1", 1));
        List<string> stoneTags = new List<string>();
        stoneTags.Add("Wearable");
        stoneTags.Add("Weapon");
        stoneTags.Add("Ranged");
        stoneTags.Add("No Addon");
        stoneTags.Add("No Magic");
        stoneTags.Add("No Weapon Spec");
        stoneTags.Add("Size For All");
        stoneTags.Add("One Hand");
        SlotsConfig stoneSlotsConfig = new SlotsConfig();
        ItemTemplate stone = ScriptableObject.CreateInstance<ItemTemplate>().Init("Stone", "Common ranged weapon", stoneTags, stoneFormulas, stoneSlotsConfig);
        _items.Add("Stone", stone);
        stone.store(this);
        // Sword item
        List<Formula> swordFormulas = new List<Formula>();
        swordFormulas.Add(new Formula("dmg", "6", 1));
        List<string> swordTags = new List<string>();
        swordTags.Add("Wearable");
        swordTags.Add("Weapon");
        swordTags.Add("Melee");
        swordTags.Add("No Addon");
        swordTags.Add("No Magic");
        swordTags.Add("Sword");
        swordTags.Add("Size For All");
        swordTags.Add("One Hand");
        SlotsConfig swordSlotsConfig = new SlotsConfig();
        ItemTemplate sword = ScriptableObject.CreateInstance<ItemTemplate>().Init("Sword", "Normal sword for swordmen", swordTags, swordFormulas, swordSlotsConfig);
        _items.Add("Sword", sword);
        sword.store(this);
        // Fire Sword item
        List<Formula> fireSwordFormulas = new List<Formula>();
        fireSwordFormulas.Add(new Formula("dmg", "6", 1));
        fireSwordFormulas.Add(new Formula("fdg", "3", 1));
        List<string> fireSwordTags = new List<string>();
        fireSwordTags.Add("Wearable");
        fireSwordTags.Add("Weapon");
        fireSwordTags.Add("Melee");
        fireSwordTags.Add("No Addon");
        fireSwordTags.Add("Fire");
        fireSwordTags.Add("Sword");
        fireSwordTags.Add("Size For All");
        fireSwordTags.Add("One Hand");
        SlotsConfig fireSwordSlotsConfig = new SlotsConfig();
        ItemTemplate fireSword = ScriptableObject.CreateInstance<ItemTemplate>().Init("Fire sword", "Fire sword for swordmen", fireSwordTags, fireSwordFormulas, fireSwordSlotsConfig);
        _items.Add("Fire sword", fireSword);
        fireSword.store(this);
        // Broadsword item
        List<Formula> broadswordFormulas = new List<Formula>();
        broadswordFormulas.Add(new Formula("dmg", "10", 1));
        List<string> broadswordTags = new List<string>();
        broadswordTags.Add("Wearable");
        broadswordTags.Add("Weapon");
        broadswordTags.Add("Melee");
        broadswordTags.Add("No Addon");
        broadswordTags.Add("No Magic");
        broadswordTags.Add("Sword");
        broadswordTags.Add("Heavy");
        broadswordTags.Add("Two Hand");
        SlotsConfig broadswordSlotsConfig = new SlotsConfig();
        ItemTemplate broadsword = ScriptableObject.CreateInstance<ItemTemplate>().Init("Broadsword", "Broadsword for heavy, two-handed sword specialists", broadswordTags, broadswordFormulas, broadswordSlotsConfig);
        _items.Add("Broadsword", broadsword);
        broadsword.store(this);
        // Bow item
        List<Formula> bowFormulas = new List<Formula>();
        bowFormulas.Add(new Formula("dmg", "4", 1));
        List<string> bowTags = new List<string>();
        bowTags.Add("Wearable");
        bowTags.Add("Weapon");
        bowTags.Add("Ranged");
        bowTags.Add("No Addon");
        bowTags.Add("No Magic");
        bowTags.Add("Bow");
        bowTags.Add("Size For All");
        bowTags.Add("Two Hand");
        SlotsConfig bowSlotsConfig = new SlotsConfig();
        ItemTemplate bow = ScriptableObject.CreateInstance<ItemTemplate>().Init("Bow", "Normal bow for bowmen", bowTags, bowFormulas, bowSlotsConfig);
        _items.Add("Bow", bow);
        bow.store(this);
        // Ice Bow item
        List<Formula> iceBowFormulas = new List<Formula>();
        iceBowFormulas.Add(new Formula("dmg", "4", 1));
        iceBowFormulas.Add(new Formula("idg", "2", 1));
        List<string> iceBowTags = new List<string>();
        iceBowTags.Add("Wearable");
        iceBowTags.Add("Weapon");
        iceBowTags.Add("Ranged");
        iceBowTags.Add("No Addon");
        iceBowTags.Add("Ice");
        iceBowTags.Add("Bow");
        iceBowTags.Add("Size For All");
        iceBowTags.Add("Two Hand");
        SlotsConfig iceBowSlotsConfig = new SlotsConfig();
        ItemTemplate iceBow = ScriptableObject.CreateInstance<ItemTemplate>().Init("Ice bow", "Ice bow for bowmen", iceBowTags, iceBowFormulas, iceBowSlotsConfig);
        _items.Add("Ice bow", iceBow);
        iceBow.store(this);
        #endregion
        #region Cloths
        // Trousers item
        List<Formula> trousersFormulas = new List<Formula>();
        trousersFormulas.Add(new Formula("def", "2", 1));
        List<string> trousersTags = new List<string>();
        trousersTags.Add("Wearable");
        trousersTags.Add("Cloth");
        trousersTags.Add("Legs");
        trousersTags.Add("No Addon");
        trousersTags.Add("No Magic");
        trousersTags.Add("Size For All");
        trousersTags.Add("No Weak");
        SlotsConfig trousersSlotsConfig = new SlotsConfig();
        ItemTemplate trousers = ScriptableObject.CreateInstance<ItemTemplate>().Init("Trousers", "Common trousers", trousersTags, trousersFormulas, trousersSlotsConfig);
        _items.Add("Trousers", trousers);
        trousers.store(this);
        // Shirt item
        List<Formula> shirtFormulas = new List<Formula>();
        shirtFormulas.Add(new Formula("def", "3", 1));
        List<string> shirtTags = new List<string>();
        shirtTags.Add("Wearable");
        shirtTags.Add("Cloth");
        shirtTags.Add("Chest");
        shirtTags.Add("No Addon");
        shirtTags.Add("No Magic");
        shirtTags.Add("Size For All");
        shirtTags.Add("No Weak");
        SlotsConfig shirtSlotsConfig = new SlotsConfig();
        ItemTemplate shirt = ScriptableObject.CreateInstance<ItemTemplate>().Init("Shirt", "Common shirt", shirtTags, shirtFormulas, shirtSlotsConfig);
        _items.Add("Shirt", shirt);
        shirt.store(this);
        // Boots item
        List<Formula> bootsFormulas = new List<Formula>();
        bootsFormulas.Add(new Formula("def", "1", 1));
        List<string> bootsTags = new List<string>();
        bootsTags.Add("Wearable");
        bootsTags.Add("Cloth");
        bootsTags.Add("Feet");
        bootsTags.Add("No Addon");
        bootsTags.Add("No Magic");
        bootsTags.Add("Size For All");
        bootsTags.Add("No Weak");
        SlotsConfig bootsSlotsConfig = new SlotsConfig();
        ItemTemplate boots = ScriptableObject.CreateInstance<ItemTemplate>().Init("Boots", "Common boots", bootsTags, bootsFormulas, bootsSlotsConfig);
        _items.Add("Boots", boots);
        boots.store(this);
        // Iron Helmet item
        List<Formula> ironHelmetFormulas = new List<Formula>();
        ironHelmetFormulas.Add(new Formula("def", "5", 1));
        List<string> ironHelmetTags = new List<string>();
        ironHelmetTags.Add("Wearable");
        ironHelmetTags.Add("Cloth");
        ironHelmetTags.Add("Head");
        ironHelmetTags.Add("No Addon");
        ironHelmetTags.Add("No Magic");
        ironHelmetTags.Add("Heavy");
        ironHelmetTags.Add("No Weak");
        SlotsConfig ironHelmetSlotsConfig = new SlotsConfig();
        ItemTemplate ironHelmet = ScriptableObject.CreateInstance<ItemTemplate>().Init("Iron helmet", "Heavy helmet", ironHelmetTags, ironHelmetFormulas, ironHelmetSlotsConfig);
        _items.Add("Iron helmet", ironHelmet);
        ironHelmet.store(this);
        // Iron Armor item
        List<Formula> ironArmorFormulas = new List<Formula>();
        ironArmorFormulas.Add(new Formula("def", "10", 1));
        List<string> ironArmorTags = new List<string>();
        ironArmorTags.Add("Wearable");
        ironArmorTags.Add("Cloth");
        ironArmorTags.Add("Chest");
        ironArmorTags.Add("No Addon");
        ironArmorTags.Add("No Magic");
        ironArmorTags.Add("Heavy");
        ironArmorTags.Add("No Weak");
        SlotsConfig ironArmorSlotsConfig = new SlotsConfig();
        ItemTemplate ironArmor = ScriptableObject.CreateInstance<ItemTemplate>().Init("Iron armor", "Heavy armor", ironArmorTags, ironArmorFormulas, ironArmorSlotsConfig);
        _items.Add("Iron armor", ironArmor);
        ironArmor.store(this);
        // Iron Armbands item
        List<Formula> ironArmbandsFormulas = new List<Formula>();
        ironArmbandsFormulas.Add(new Formula("def", "3", 1));
        List<string> ironArmbandsTags = new List<string>();
        ironArmbandsTags.Add("Wearable");
        ironArmbandsTags.Add("Cloth");
        ironArmbandsTags.Add("Arms");
        ironArmbandsTags.Add("No Addon");
        ironArmbandsTags.Add("No Magic");
        ironArmbandsTags.Add("Heavy");
        ironArmbandsTags.Add("No Weak");
        SlotsConfig ironArmbandsSlotsConfig = new SlotsConfig();
        ItemTemplate ironArmbands = ScriptableObject.CreateInstance<ItemTemplate>().Init("Iron armbands", "Heavy armbands", ironArmbandsTags, ironArmbandsFormulas, ironArmbandsSlotsConfig);
        _items.Add("Iron armbands", ironArmbands);
        ironArmbands.store(this);
        // Iron Gloves item
        List<Formula> ironGlovesFormulas = new List<Formula>();
        ironGlovesFormulas.Add(new Formula("def", "2", 1));
        List<string> ironGlovesTags = new List<string>();
        ironGlovesTags.Add("Wearable");
        ironGlovesTags.Add("Cloth");
        ironGlovesTags.Add("Hands");
        ironGlovesTags.Add("No Addon");
        ironGlovesTags.Add("No Magic");
        ironGlovesTags.Add("Heavy");
        ironGlovesTags.Add("No Weak");
        SlotsConfig ironGlovesSlotsConfig = new SlotsConfig();
        ItemTemplate ironGloves = ScriptableObject.CreateInstance<ItemTemplate>().Init("Iron gloves", "Heavy gloves", ironGlovesTags, ironGlovesFormulas, ironGlovesSlotsConfig);
        _items.Add("Iron gloves", ironGloves);
        ironGloves.store(this);
        // Iron Legbands item
        List<Formula> ironLegbandsFormulas = new List<Formula>();
        ironLegbandsFormulas.Add(new Formula("def", "2", 1));
        List<string> ironLegbandsTags = new List<string>();
        ironLegbandsTags.Add("Wearable");
        ironLegbandsTags.Add("Cloth");
        ironLegbandsTags.Add("Legs");
        ironLegbandsTags.Add("No Addon");
        ironLegbandsTags.Add("No Magic");
        ironLegbandsTags.Add("Heavy");
        ironLegbandsTags.Add("No Weak");
        SlotsConfig ironLegbandsSlotsConfig = new SlotsConfig();
        ItemTemplate ironLegbands = ScriptableObject.CreateInstance<ItemTemplate>().Init("Iron legbands", "Heavy legbands", ironLegbandsTags, ironLegbandsFormulas, ironLegbandsSlotsConfig);
        _items.Add("Iron legbands", ironLegbands);
        ironLegbands.store(this);
        // Iron Boots item
        List<Formula> ironBootsFormulas = new List<Formula>();
        ironBootsFormulas.Add(new Formula("def", "3", 1));
        List<string> ironBootsTags = new List<string>();
        ironBootsTags.Add("Wearable");
        ironBootsTags.Add("Cloth");
        ironBootsTags.Add("Feet");
        ironBootsTags.Add("No Addon");
        ironBootsTags.Add("No Magic");
        ironBootsTags.Add("Heavy");
        ironBootsTags.Add("No Weak");
        SlotsConfig ironBootsSlotsConfig = new SlotsConfig();
        ItemTemplate ironBoots = ScriptableObject.CreateInstance<ItemTemplate>().Init("Iron boots", "Heavy boots", ironBootsTags, ironBootsFormulas, ironBootsSlotsConfig);
        _items.Add("Iron boots", ironBoots);
        ironBoots.store(this);
        // Light Helmet item
        List<Formula> lightHelmetFormulas = new List<Formula>();
        lightHelmetFormulas.Add(new Formula("def", "3", 1));
        List<string> lightHelmetTags = new List<string>();
        lightHelmetTags.Add("Wearable");
        lightHelmetTags.Add("Cloth");
        lightHelmetTags.Add("Head");
        lightHelmetTags.Add("No Addon");
        lightHelmetTags.Add("No Magic");
        lightHelmetTags.Add("Light");
        lightHelmetTags.Add("No Weak");
        SlotsConfig lightHelmetSlotsConfig = new SlotsConfig();
        ItemTemplate lightHelmet = ScriptableObject.CreateInstance<ItemTemplate>().Init("Light helmet", "Light helmet", lightHelmetTags, lightHelmetFormulas, lightHelmetSlotsConfig);
        _items.Add("Light helmet", lightHelmet);
        lightHelmet.store(this);
        // Iron Armor item
        List<Formula> lightArmorFormulas = new List<Formula>();
        lightArmorFormulas.Add(new Formula("def", "7", 1));
        List<string> lightArmorTags = new List<string>();
        lightArmorTags.Add("Wearable");
        lightArmorTags.Add("Cloth");
        lightArmorTags.Add("Chest");
        lightArmorTags.Add("No Addon");
        lightArmorTags.Add("No Magic");
        lightArmorTags.Add("Light");
        lightArmorTags.Add("No Weak");
        SlotsConfig lightArmorSlotsConfig = new SlotsConfig();
        ItemTemplate lightArmor = ScriptableObject.CreateInstance<ItemTemplate>().Init("Light armor", "Light armor", lightArmorTags, lightArmorFormulas, lightArmorSlotsConfig);
        _items.Add("Light armor", lightArmor);
        lightArmor.store(this);
        // Light Armbands item
        List<Formula> lightArmbandsFormulas = new List<Formula>();
        lightArmbandsFormulas.Add(new Formula("def", "3", 1));
        List<string> ligthArmbandsTags = new List<string>();
        ligthArmbandsTags.Add("Wearable");
        ligthArmbandsTags.Add("Cloth");
        ligthArmbandsTags.Add("Arms");
        ligthArmbandsTags.Add("No Addon");
        ligthArmbandsTags.Add("No Magic");
        ligthArmbandsTags.Add("Light");
        ligthArmbandsTags.Add("No Weak");
        SlotsConfig lightArmbandsSlotsConfig = new SlotsConfig();
        ItemTemplate lightArmbands = ScriptableObject.CreateInstance<ItemTemplate>().Init("Light armbands", "Light armbands", ligthArmbandsTags, lightArmbandsFormulas, lightArmbandsSlotsConfig);
        _items.Add("Light armbands", lightArmbands);
        lightArmbands.store(this);
        // Light Gloves item
        List<Formula> lightGlovesFormulas = new List<Formula>();
        lightGlovesFormulas.Add(new Formula("def", "1", 1));
        List<string> lightGlovesTags = new List<string>();
        lightGlovesTags.Add("Wearable");
        lightGlovesTags.Add("Cloth");
        lightGlovesTags.Add("Hands");
        lightGlovesTags.Add("No Addon");
        lightGlovesTags.Add("No Magic");
        lightGlovesTags.Add("Light");
        lightGlovesTags.Add("No Weak");
        SlotsConfig lightGlovesSlotsConfig = new SlotsConfig();
        ItemTemplate lightGloves = ScriptableObject.CreateInstance<ItemTemplate>().Init("Light gloves", "Light gloves", lightGlovesTags, lightGlovesFormulas, lightGlovesSlotsConfig);
        _items.Add("Light gloves", lightGloves);
        lightGloves.store(this);
        // Light Legbands item
        List<Formula> lightLegbandsFormulas = new List<Formula>();
        lightLegbandsFormulas.Add(new Formula("def", "1", 1));
        List<string> lightLegbandsTags = new List<string>();
        lightLegbandsTags.Add("Wearable");
        lightLegbandsTags.Add("Cloth");
        lightLegbandsTags.Add("Legs");
        lightLegbandsTags.Add("No Addon");
        lightLegbandsTags.Add("No Magic");
        lightLegbandsTags.Add("Light");
        lightLegbandsTags.Add("No Weak");
        SlotsConfig lightLegbandsSlotsConfig = new SlotsConfig();
        ItemTemplate lightLegbands = ScriptableObject.CreateInstance<ItemTemplate>().Init("Light legbands", "Light legbands", lightLegbandsTags, lightLegbandsFormulas, lightLegbandsSlotsConfig);
        _items.Add("Light legbands", lightLegbands);
        lightLegbands.store(this);
        // Light Boots item
        List<Formula> lightBootsFormulas = new List<Formula>();
        lightBootsFormulas.Add(new Formula("def", "1", 1));
        List<string> lightBootsTags = new List<string>();
        lightBootsTags.Add("Wearable");
        lightBootsTags.Add("Cloth");
        lightBootsTags.Add("Feet");
        lightBootsTags.Add("No Addon");
        lightBootsTags.Add("No Magic");
        lightBootsTags.Add("Light");
        lightBootsTags.Add("No Weak");
        SlotsConfig lightBootsSlotsConfig = new SlotsConfig();
        ItemTemplate lightBoots = ScriptableObject.CreateInstance<ItemTemplate>().Init("Light boots", "Light boots", lightBootsTags, lightBootsFormulas, lightBootsSlotsConfig);
        _items.Add("Light boots", lightBoots);
        lightBoots.store(this);
        #endregion
        #region Specializations
        // Filling derived 'specs'
        // Swordman spec
        List<Formula> swordmanFormulas = new List<Formula>();
        swordmanFormulas.Add(new Formula("CON", "5", 1));  // Swordmen have better constitution
        SlotsConfig swordmanSlotsConfig = new SlotsConfig();
        List<ItemConfig> listSwordmanIc = new List<ItemConfig>();
        ItemConfig swordmanIc1;
        swordmanIc1.itemIds = new List<string> { "Wearable", "Weapon", "Melee", "No Addon", "Sword", "No Magic", "Size For All", "One Hand" };
        swordmanIc1.itemMask = 85098522;
        listSwordmanIc.Add(swordmanIc1);
        swordmanSlotsConfig.ItemCfg = listSwordmanIc;
        SpecTemplate swordman = ScriptableObject.CreateInstance<SpecTemplate>().Init("Swordman", "Swordman is a spec in which you can use swords", false, swordmanFormulas, swordmanSlotsConfig);
        _specs.Add("Swordman", swordman);
        swordman.store(this);
        // Broadswordman spec
        List<Formula> broadswordmanFormulas = new List<Formula>();
        broadswordmanFormulas.Add(new Formula("CON", "10", 1));  // Swordmen have much better constitution
        SlotsConfig broadswordmanSlotsConfig = new SlotsConfig();
        List<ItemConfig> listBroadswordmanIc = new List<ItemConfig>();
        ItemConfig broadswordmanIc1;
        broadswordmanIc1.itemIds = new List<string> { "Wearable", "Weapon", "Melee", "No Addon", "Sword", "No Magic", "Heavy", "Two Hand" };
        broadswordmanIc1.itemMask = 153255962;
        listBroadswordmanIc.Add(broadswordmanIc1);
        broadswordmanSlotsConfig.ItemCfg = listBroadswordmanIc;
        SpecTemplate broadswordman = ScriptableObject.CreateInstance<SpecTemplate>().Init("Broadswordman", "Broadswordman is a spec in which you can use broadswords", false, broadswordmanFormulas, broadswordmanSlotsConfig);
        _specs.Add("Broadswordman", broadswordman);
        broadswordman.store(this);
        // Bowman spec
        List<Formula> bowmanFormulas = new List<Formula>();
        bowmanFormulas.Add(new Formula("DEX", "5", 1));  // Bowmen have better dexterity
        SlotsConfig bowmanSlotsConfig = new SlotsConfig();
        List<ItemConfig> listBowmanIc = new List<ItemConfig>();
        ItemConfig bowmanIc1;
        bowmanIc1.itemIds = new List<string> { "Wearable", "Weapon", "Ranged", "No Addon", "Bow", "No Magic", "Size For All", "Two Hand" };
        bowmanIc1.itemMask = 168984618;
        listBowmanIc.Add(bowmanIc1);
        bowmanSlotsConfig.ItemCfg = listBowmanIc;
        SpecTemplate bowman = ScriptableObject.CreateInstance<SpecTemplate>().Init("Bowman", "Bowman is a spec in which you can use bows", false, bowmanFormulas, bowmanSlotsConfig);
        _specs.Add("Bowman", bowman);
        bowman.store(this);
        // Heavy carrier spec
        List<Formula> heavyFormulas = new List<Formula>();
        heavyFormulas.Add(new Formula("CON", "5", 1));  // Heavy specs have better constitution
        SlotsConfig heavySlotsConfig = new SlotsConfig();
        List<ItemConfig> listHeavyIc = new List<ItemConfig>();
        ItemConfig heavyIc1;
        heavyIc1.itemIds = new List<string> { "Wearable", "Cloth", "Head", "No Addon", "No Magic", "Heavy", "No Weak" };
        heavyIc1.itemMask = 270696642;
        listHeavyIc.Add(heavyIc1);
        ItemConfig heavyIc2;
        heavyIc2.itemIds = new List<string> { "Wearable", "Cloth", "Chest", "No Addon", "No Magic", "Heavy", "No Weak" };
        heavyIc2.itemMask = 270696770;
        listHeavyIc.Add(heavyIc2);
        ItemConfig heavyIc3;
        heavyIc3.itemIds = new List<string> { "Wearable", "Cloth", "Arms", "No Addon", "No Magic", "Heavy", "No Weak" };
        heavyIc3.itemMask = 270697538;
        listHeavyIc.Add(heavyIc3);
        ItemConfig heavyIc4;
        heavyIc4.itemIds = new List<string> { "Wearable", "Cloth", "Hands", "No Addon", "No Magic", "Heavy", "No Weak" };
        heavyIc4.itemMask = 270698562;
        listHeavyIc.Add(heavyIc4);
        ItemConfig heavyIc5;
        heavyIc5.itemIds = new List<string> { "Wearable", "Cloth", "Legs", "No Addon", "No Magic", "Heavy", "No Weak" };
        heavyIc5.itemMask = 270704706;
        listHeavyIc.Add(heavyIc5);
        ItemConfig heavyIc6;
        heavyIc6.itemIds = new List<string> { "Wearable", "Cloth", "Feet", "No Addon", "No Magic", "Heavy", "No Weak" };
        heavyIc6.itemMask = 270712898;
        listHeavyIc.Add(heavyIc6);
        heavySlotsConfig.ItemCfg = listHeavyIc;
        SpecTemplate heavyCarrier = ScriptableObject.CreateInstance<SpecTemplate>().Init("Heavy Carrier", "Heavy Carrier is a spec in which you can wear heavy objects", false, heavyFormulas, heavySlotsConfig);
        _specs.Add("Heavy Carrier", heavyCarrier);
        heavyCarrier.store(this);
        // Light Carrier spec
        List<Formula> lightFormulas = new List<Formula>();
        lightFormulas.Add(new Formula("CON", "2", 1));  // Heavy specs have slightly better constitution
        SlotsConfig lightSlotsConfig = new SlotsConfig();
        List<ItemConfig> listLightIc = new List<ItemConfig>();
        ItemConfig lightIc1;
        lightIc1.itemIds = new List<string> { "Wearable", "Cloth", "Head", "No Addon", "No Magic", "Light", "No Weak" };
        lightIc1.itemMask = 272793794;
        listLightIc.Add(lightIc1);
        ItemConfig lightIc2;
        lightIc2.itemIds = new List<string> { "Wearable", "Cloth", "Chest", "No Addon", "No Magic", "Light", "No Weak" };
        lightIc2.itemMask = 272793922;
        listLightIc.Add(lightIc2);
        ItemConfig lightIc3;
        lightIc3.itemIds = new List<string> { "Wearable", "Cloth", "Arms", "No Addon", "No Magic", "Light", "No Weak" };
        lightIc3.itemMask = 272794690;
        listLightIc.Add(lightIc3);
        ItemConfig lightIc4;
        lightIc4.itemIds = new List<string> { "Wearable", "Cloth", "Hands", "No Addon", "No Magic", "Light", "No Weak" };
        lightIc4.itemMask = 272795714;
        listLightIc.Add(lightIc4);
        ItemConfig lightIc5;
        lightIc5.itemIds = new List<string> { "Wearable", "Cloth", "Legs", "No Addon", "No Magic", "Light", "No Weak" };
        lightIc5.itemMask = 272801858;
        listLightIc.Add(lightIc5);
        ItemConfig lightIc6;
        lightIc6.itemIds = new List<string> { "Wearable", "Cloth", "Feet", "No Addon", "No Magic", "Light", "No Weak" };
        lightIc6.itemMask = 272810050;
        listLightIc.Add(lightIc6);
        lightSlotsConfig.ItemCfg = listLightIc;
        SpecTemplate lightCarrier = ScriptableObject.CreateInstance<SpecTemplate>().Init("Light Carrier", "Light Carrier is a spec in which you can wear slightly heavy objects", false, lightFormulas, lightSlotsConfig);
        _specs.Add("Light Carrier", lightCarrier);
        lightCarrier.store(this);
        #endregion
        #region Classes
        // Filling basic 'specs'
        // Human spec
        List<Formula> humanFormulas = new List<Formula>();
        humanFormulas.Add(new Formula("INT", "5", 1));  // Humans are an intelligent race
        SlotsConfig humanSlotsConfig = new SlotsConfig();
        List<ItemConfig> listHumanIc = new List<ItemConfig>();
        ItemConfig humanIc0;
        humanIc0.itemIds = new List<string> { "Wearable", "Shield", "No Magic", "Size For All", "No Weak" };
        humanIc0.itemMask = 269615110;
        listHumanIc.Add(humanIc0);
        ItemConfig humanIc1;
        humanIc1.itemIds = new List<string> { "Wearable", "Weapon", "Melee", "No Addon", "No Magic", "No Weapon Spec", "Size For All", "One Hand" };
        humanIc1.itemMask = 76709914;
        listHumanIc.Add(humanIc1);
        ItemConfig humanIc2;
        humanIc2.itemIds = new List<string> { "Wearable", "Weapon", "Ranged", "No Addon", "No Magic", "No Weapon Spec", "Size For All", "One Hand" };
        humanIc2.itemMask = 76709930;
        listHumanIc.Add(humanIc2);
        ItemConfig humanIc3;
        humanIc3.itemIds = new List<string> { "Wearable", "Cloth", "Head", "No Addon", "No Magic", "Size For All", "No Weak" };
        humanIc3.itemMask = 269648066;
        listHumanIc.Add(humanIc3);
        ItemConfig humanIc4;
        humanIc4.itemIds = new List<string> { "Wearable", "Cloth", "Chest", "No Addon", "No Magic", "Size For All", "No Weak" };
        humanIc4.itemMask = 269648194;
        listHumanIc.Add(humanIc4);
        ItemConfig humanIc5;
        humanIc5.itemIds = new List<string> { "Wearable", "Cloth", "Neck", "No Addon", "No Magic", "Size For All", "No Weak" };
        humanIc5.itemMask = 269648450;
        listHumanIc.Add(humanIc5);
        ItemConfig humanIc6;
        humanIc6.itemIds = new List<string> { "Wearable", "Cloth", "Arms", "No Addon", "No Magic", "Size For All", "No Weak" };
        humanIc6.itemMask = 269648962;
        listHumanIc.Add(humanIc6);
        ItemConfig humanIc7;
        humanIc7.itemIds = new List<string> { "Wearable", "Cloth", "Hands", "No Addon", "No Magic", "Size For All", "No Weak" };
        humanIc7.itemMask = 269649986;
        listHumanIc.Add(humanIc7);
        ItemConfig humanIc8;
        humanIc8.itemIds = new List<string> { "Wearable", "Cloth", "Finger", "No Addon", "No Magic", "Size For All", "No Weak" };
        humanIc8.itemMask = 269652034;
        listHumanIc.Add(humanIc8);
        ItemConfig humanIc9;
        humanIc9.itemIds = new List<string> { "Wearable", "Cloth", "Feet", "No Addon", "No Magic", "Size For All", "No Weak" };
        humanIc9.itemMask = 269664322;
        listHumanIc.Add(humanIc9);
        ItemConfig humanIc10;
        humanIc10.itemIds = new List<string> { "Wearable", "Cloth", "Legs", "No Addon", "No Magic", "Size For All", "No Weak" };
        humanIc10.itemMask = 269656130;
        listHumanIc.Add(humanIc10);
        humanSlotsConfig.ItemCfg = listHumanIc;
        //
        List<SpecConfig> listHumanSc = new List<SpecConfig>();
        SpecConfig humanSc1;
        humanSc1.specIds = new List<string> { "Swordman", "Bowman" };
        humanSc1.specMask = 5;
        listHumanSc.Add(humanSc1);
        humanSlotsConfig.SpecCfg=listHumanSc;
        //
        SpecTemplate human = ScriptableObject.CreateInstance<SpecTemplate>().Init("Human", "Humans are flesh and bones", true, humanFormulas, humanSlotsConfig);
        _specs.Add("Human", human);
        human.store(this);
        // Orc spec
        List<Formula> orcFormulas = new List<Formula>();
        orcFormulas.Add(new Formula("STR", "5", 1));  // Orcs are a strong race
        SlotsConfig orcSlotsConfig = new SlotsConfig();
        List<ItemConfig> listOrcIc = new List<ItemConfig>();
        ItemConfig orcIc1;
        orcIc1.itemIds = new List<string> { "Wearable", "Weapon", "Melee", "No Addon", "No Magic", "No Weapon Spec", "Size For All", "Two Hand" };
        orcIc1.itemMask = 143818778;
        listOrcIc.Add(orcIc1);
        ItemConfig orcIc2;
        orcIc2.itemIds = new List<string> { "Wearable", "Weapon", "Ranged", "No Addon", "No Magic", "No Weapon Spec", "Size For All", "One Hand" };
        orcIc2.itemMask = 76709930;
        listOrcIc.Add(orcIc2);
        ItemConfig orcIc3;
        orcIc3.itemIds = new List<string> { "Wearable", "Cloth", "Head", "No Addon", "No Magic", "Size For All", "No Weak" };
        orcIc3.itemMask = 269648066;
        listOrcIc.Add(orcIc3);
        ItemConfig orcIc4;
        orcIc4.itemIds = new List<string> { "Wearable", "Cloth", "Chest", "No Addon", "No Magic", "Size For All", "No Weak" };
        orcIc4.itemMask = 269648194;
        listOrcIc.Add(orcIc4);
        ItemConfig orcIc5;
        orcIc5.itemIds = new List<string> { "Wearable", "Cloth", "Neck", "No Addon", "No Magic", "Size For All", "No Weak" };
        orcIc5.itemMask = 269648450;
        listOrcIc.Add(orcIc5);
        ItemConfig orcIc6;
        orcIc6.itemIds = new List<string> { "Wearable", "Cloth", "Arms", "No Addon", "No Magic", "Size For All", "No Weak" };
        orcIc6.itemMask = 269648962;
        listOrcIc.Add(orcIc6);
        ItemConfig orcIc7;
        orcIc7.itemIds = new List<string> { "Wearable", "Cloth", "Hands", "No Addon", "No Magic", "Size For All", "No Weak" };
        orcIc7.itemMask = 269649986;
        listOrcIc.Add(orcIc7);
        ItemConfig orcIc10;
        orcIc10.itemIds = new List<string> { "Wearable", "Cloth", "Legs", "No Addon", "No Magic", "Size For All", "No Weak" };
        orcIc10.itemMask = 269656130;
        listOrcIc.Add(orcIc10);
        orcSlotsConfig.ItemCfg = listOrcIc;
        SpecTemplate orc = ScriptableObject.CreateInstance<SpecTemplate>().Init("Orc", "Orcs are typically green, ugly and do not wear any shoes, nor rings, nor shields. Use two handed weapons", true, orcFormulas, orcSlotsConfig);
        _specs.Add("Orc", orc);
        orc.store(this);
        // Dummy spec
        List<Formula> dummyFormulas = new List<Formula>();
        SlotsConfig dummySlotsConfig = new SlotsConfig();
        //
        List<SpecConfig> listDummySc = new List<SpecConfig>();
        SpecConfig dummySc1;
        dummySc1.specIds = new List<string> { "Heavy Carrier", "Light Carrier" };
        dummySc1.specMask = 24;
        listDummySc.Add(dummySc1);
        dummySlotsConfig.SpecCfg=listDummySc;
        //
        SpecTemplate dummy = ScriptableObject.CreateInstance<SpecTemplate>().Init("Dummy", "Dummies are empty training objects", true, dummyFormulas, dummySlotsConfig);
        _specs.Add("Dummy", dummy);
        dummy.store(this);
        #endregion
        UnityEditor.AssetDatabase.SaveAssets();
        //Debug.Log("Database filled as 'Demo Database'!");
        }

    private void fillTags(string option)
        // Fills the database with the tags that can be attached to certain Template (PassiveTemplate, ItemTemplate, 
        // SpecTemplate). The tags for PassiveTemplate mean when the pasive happens and to whom is executed. The tags 
        // for ItemTemplate mean what the item is or where it is equipped
        {
        #region Empty database
        if (option == "empty")
            {
            // Create tags container      
            _tags = new Dictionary<string, List<string>>();
            // Create tags for PassiveTemplate
            _tags.Add("PassiveWhen", new List<string>());
            _tags["PassiveWhen"].Add("Permanent");
            _tags["PassiveWhen"].Add("Turn");
            _tags["PassiveWhen"].Add("Time");
            _tags.Add("PassiveToWhom", new List<string>());
            _tags["PassiveToWhom"].Add("Self");
            _tags["PassiveToWhom"].Add("Enemy");
            _tags["PassiveToWhom"].Add("Friend");
            _tags["PassiveToWhom"].Add("Enemy Group");
            _tags["PassiveToWhom"].Add("Friend Group");
            // Create tags for ItemTemplate
            _tags.Add("Item", new List<string>());
            // Create tags for SpecTemplate
            _tags.Add("Specialization", new List<string>());
            }
        #endregion
        #region Demo database
        else if (option == "demo")
        {
            // Create tags container      
            _tags = new Dictionary<string, List<string>>();
            // Create tags for PassiveTemplate
            _tags.Add("PassiveWhen", new List<string>());
            _tags["PassiveWhen"].Add("Permanent");
            _tags["PassiveWhen"].Add("Turn");
            _tags["PassiveWhen"].Add("Time");
            _tags.Add("PassiveToWhom", new List<string>());
            _tags["PassiveToWhom"].Add("Self");
            _tags["PassiveToWhom"].Add("Enemy");
            _tags["PassiveToWhom"].Add("Friend");
            _tags["PassiveToWhom"].Add("Enemy Group");
            _tags["PassiveToWhom"].Add("Friend Group");
            // Create tags for ItemTemplate
            _tags.Add("Item", new List<string>());
            _tags["Item"].Add("Carryable");
            _tags["Item"].Add("Wearable");
            _tags["Item"].Add("Shield");
            _tags["Item"].Add("Weapon");
            _tags["Item"].Add("Melee");
            _tags["Item"].Add("Ranged");
            _tags["Item"].Add("Cloth");
            _tags["Item"].Add("Head");
            _tags["Item"].Add("Chest");
            _tags["Item"].Add("Neck");
            _tags["Item"].Add("Arms");
            _tags["Item"].Add("Hands");
            _tags["Item"].Add("Finger");
            _tags["Item"].Add("Legs");
            _tags["Item"].Add("Feet");
            _tags["Item"].Add("No Addon");  // Meaning: item is not addon for another item with same tags except "Addon" tag
            _tags["Item"].Add("Addon");  // Meaning: item is an addon for another item with same tags except "Addon" tag
            _tags["Item"].Add("No Magic");  // Meaning: weapon is an addon for another item with same tags except "Addon" tag
            _tags["Item"].Add("Fire");  // Meaning: weapons that make fire damage or cloths that block fire damage
            _tags["Item"].Add("Ice");  // Meaning: weapons that make ice damage or cloths that block ice damage
            _tags["Item"].Add("Size For All");  // Meaning: items can be worn by those specs with tag "Size For All"
            _tags["Item"].Add("Heavy");  // Meaning: items can be worn by those specs with tag "Heavy"
            _tags["Item"].Add("Light");  // Meaning: items can be worn by those specs with tag "Light"
            _tags["Item"].Add("No Weapon Spec");  // Meaning: weapon can be used by every class/spec with tag "No Weapon Spec"
            _tags["Item"].Add("Sword");  // Meaning: sword type melee weapons can be used by those specs with tag "Sword"
            _tags["Item"].Add("Bow");  // Meaning: bow ranged weapons can be used by those specs with tag "Bow"
            _tags["Item"].Add("One Hand");  // Meaning: weapon uses one hand (shield can be used) 
            _tags["Item"].Add("Two Hand");  // Meaning: weapon uses two hands (shield can not be used)
            _tags["Item"].Add("No Weak");  // Meaning: items aren't affected by any magic
            _tags["Item"].Add("Weak Fire");  // Meaning: items that are more affected by fire damage
            _tags["Item"].Add("Weak Ice");  // Meaning: items that are more affected by ice damage
            // Create tags for SpecTemplate
            _tags.Add("Specialization", new List<string>());
        }
        #endregion
        }

    public void loadDatabase()
        // TO DO | Retrieves the database from file to continue filling it. This file was (obviously) previously 
        // saved by the user when he was editing from Editor
        {
        }

    public void storeDatabase()
        // TO DO | Stores the database into a file to continue filling it later
        {
        }

    public void OnBeforeSerialize()
        // Stores Attributes Dictionary and every Template Dictionary in List format. Then info can be serialized
        {
        _AttributesList = _Attributes.Values.ToList();
        _specsKeys = _specs.Keys.ToList();
        _specsValues = _specs.Values.ToList();
        _itemsKeys = _items.Keys.ToList();
        _itemsValues = _items.Values.ToList();
        _passivesKeys = _passives.Keys.ToList();
        _passivesValues = _passives.Values.ToList();
        _tagKeys = _tags.Keys.ToList();
        _tagValues = _tags.Values.ToList().ConvertAll(l => new ListWrapper(l));
        }

    public void OnAfterDeserialize()
        // Restores Attributes Dictionary and every Template Dictionary from their stored info in Lists
        {
        _Attributes = new Dictionary<string, AttributeTRPG>();
        _AttributesList.ForEach(a => _Attributes.Add(a.id, a));

        _specs = new Dictionary<string, SpecTemplate>();
        var skeysIterator = _specsKeys.GetEnumerator();
        var svalsIterator = _specsValues.GetEnumerator();
        while (skeysIterator.MoveNext() && svalsIterator.MoveNext())
            _specs.Add(skeysIterator.Current, svalsIterator.Current);

        _items = new Dictionary<string, ItemTemplate>();
        var ikeysIterator = _itemsKeys.GetEnumerator();
        var ivalsIterator = _itemsValues.GetEnumerator();
        while (ikeysIterator.MoveNext() && ivalsIterator.MoveNext())
            _items.Add(ikeysIterator.Current, ivalsIterator.Current);

        _passives = new Dictionary<string, PassiveTemplate>();
        var pkeysIterator = _passivesKeys.GetEnumerator();
        var pvalsIterator = _passivesValues.GetEnumerator();
        while (pkeysIterator.MoveNext() && pvalsIterator.MoveNext())
            _passives.Add(pkeysIterator.Current, pvalsIterator.Current);

        _tags = new Dictionary<string, List<string>>();
        var keysIterator = _tagKeys.GetEnumerator();
        var valsIterator = _tagValues.GetEnumerator();
        while (keysIterator.MoveNext() && valsIterator.MoveNext())
            {
            _tags.Add(keysIterator.Current, valsIterator.Current.myList);
            }

        }
    #endregion

    #region Database elemements management: get, add, modify, and delete database elements
    public List<string> getTagKeys()
        // Retrieves the list of keys in the Dictionary '_tags'
        {
        List<string> tagKeys=new List<string>();
        foreach (string key in _tags.Keys)
            tagKeys.Add(key);
        return tagKeys;
        }

    public List<string> getTagValues(string key)
        // Retrieves the list of values for the key 'key' in the Dictionary '_tags'
        {
        List<string> tagValues=new List<string>();
        foreach (string value in _tags[key])
            {
            tagValues.Add(value);
            }
        return tagValues;
        }
    
    public List<string> getAttribIdentifiers()
        // Retrieves the list of identifiers for every AttributeTRPG stored in the database Dictionary '_Attributes'
        {
        List<string> attribIdentifiers = new List<string>();
        foreach (KeyValuePair<string, AttributeTRPG> result in _Attributes)
            {
            attribIdentifiers.Add(result.Value.id);
            }
        return attribIdentifiers;
        }

    public List<string> getItemNames()
        // Retrieves the list of names (identifieres) for every ItemTemplate stored in the database Dictionary '_items'
        {
        List<string> itemIdentifiers = new List<string>();
        foreach (KeyValuePair<string, ItemTemplate> result in _items)
            {
            itemIdentifiers.Add(result.Value.NameId);
            }
        return itemIdentifiers;
        }

    public List<string> getSpecNames()
        // Retrieves the list of names (identifieres) for every SpecTemplate stored in the database Dictionary '_specs'
        {
        List<string> specIdentifiers = new List<string>();
        foreach (KeyValuePair<string, SpecTemplate> result in _specs)
            {
            specIdentifiers.Add(result.Value.NameId);
            }
        return specIdentifiers;
        }

    public List<string> getPassiveNames()
        // Retrieves the list of names (identifieres) for every PassiveTemplate stored in the database Dictionary '_passives'
        {
        List<string> passiveIdentifiers = new List<string>();
        foreach (KeyValuePair<string, PassiveTemplate> result in _passives)
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

    public List<Formula> getTemplateFormulas(string nameId, Template t)
        // TO DO. Sustituir por getItemFormulas y getSpecFormulas | Returns the list of formulas (Formula) of a Template of type 't' identified by 'nameId'
        {
        if (t is SpecTemplate)
            return _specs[nameId].Formulas;
        else if (t is ItemTemplate)
            return _items[nameId].Formulas;
        else if (t is PassiveTemplate)
            return _passives[nameId].Formulas;
        else
            return null;
        }

    public List<string> getAllowedSlots(Template t)
        // Returns a list with the type of slots the Template t can use
        {
        if (t is SpecTemplate)
            return new List<string> { "Specialization", "Item", "Passive" };
        else if (t is ItemTemplate)
            return new List<string> { "Item", "Passive" };
        else if (t is PassiveTemplate)
            return new List<string> { "Passive" };
        else
            return new List<string> { "Template not supported" };
        }

    public bool addAttribute(AttributeTRPG atr)
        // Tries to add a new AttributeTRPG to the database and informs about it
        {
        bool canAdd = _Attributes.ContainsValue(atr) ? false : true;
        if (canAdd)
            _Attributes.Add(atr.id, atr);
        return canAdd;
        }

    public bool deleteAttribute(string atr)
        // Tries to delete an existing AttributeTRPG from the database and informs about it
        {
        bool canDelete = _Attributes.ContainsKey(atr) ? true : false;
        if (canDelete)
            _Attributes.Remove(atr);
        return canDelete;
        }

    public void modifyAttribute(string atrId, int value)
        // Modifies the value of an stored AttributeTRPG
        {
        _Attributes[atrId].value = value;
        }

    public bool addModDelTemplate(Template t, string order)
        // Tries to Add/Modify/Delete a Template (SpecTemplate, ItemTemplate, PassiveTemplate) to/from the database and informs about it
        {
        bool canAct = false;
        if (order == "add")
            {
            t.store(this);
            }

        if (t is ItemTemplate)
            {
            if (order == "add")
                {
                canAct = _items.ContainsValue(t as ItemTemplate) ? false : true;
                if (canAct)
                    _items.Add(t.NameId, t as ItemTemplate);
                }
            else if (order == "mod")
                {
                canAct = _items.ContainsKey(t.NameId) ? true : false;
                if (canAct)
                    {
                    _items.Remove(t.NameId);
                    _items.Add(t.NameId, t as ItemTemplate);
                    }
                }
            else if (order == "del")
                {
                canAct = _items.ContainsKey(t.NameId) ? true : false;
                if (canAct)
                    _items.Remove(t.NameId);
                }
            }
        else if (t is PassiveTemplate)
            {
            if (order == "add")
                {
                canAct = _passives.ContainsValue(t as PassiveTemplate) ? false : true;
                if (canAct)
                    _passives.Add(t.NameId, t as PassiveTemplate);
                }
            else if (order == "mod")
                {
                canAct = _passives.ContainsKey(t.NameId) ? true : false;
                if (canAct)
                    {
                    _passives.Remove(t.NameId);
                    _passives.Add(t.NameId, t as PassiveTemplate);
                    }
                }
            else if (order == "del")
                {
                canAct = _passives.ContainsKey(t.NameId) ? true : false;
                if (canAct)
                    _passives.Remove(t.NameId);
                }
            }
        else if (t is SpecTemplate)
            {
            if (order == "add")
                {
                canAct = _specs.ContainsValue(t as SpecTemplate) ? false : true;
                if (canAct)
                    _specs.Add(t.NameId, t as SpecTemplate);
                }
            else if (order == "mod")
                {
                canAct = _specs.ContainsKey(t.NameId) ? true : false;
                if (canAct)
                    {
                    _specs.Remove(t.NameId);
                    _specs.Add(t.NameId, t as SpecTemplate);
                    }
                }
            else if (order == "del")
                {
                canAct = _specs.ContainsKey(t.NameId) ? true : false;
                if (canAct)
                    _specs.Remove(t.NameId);
                }
            }
        return canAct;
        }
        #endregion
    #endregion

    // TODO

    [Serializable]
    public class SkillAnimation
    {
        public string name;
        public IsoUnity.IsoDecoration castAnimation;
        public IsoUnity.IsoDecoration receiveAnimation;
    }

    public List<SkillAnimation> animations;
}