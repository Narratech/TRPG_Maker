using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BHEditor : EditorWindow
{
    private static int MAX_REQUIREMENTS = 50;


    private int damage = 0;
    private int distance = 0;
    private double manaCost = 0;
    private int step = 0;
    private int area = 0;
    private string skillName;
    private string skillDescription;

    private bool active = true;

    private string[] spelltype = new string[] { "Linear Projectile", "Parabolic Projectile", "On Ground" };
    private int selectedSpellType;
    private string[] skillType = new string[] { "Self Character", "Self Character with direction", "Ranged place", "Global"};
    private int selectedSkillType;
    private string[] skillEffect = new string[] { "Single target", "Area", "Area in objective", "Global" };
    private int selectedEffect;
    private string[] skillType1 = new string[] { "Healing", "Damage", "Both" };
    //para la creacion de habilidad
    int selectedSkillType1;
    //para la edicion de habilidades
    int[] selectedSkillTypes;
    int[] selectedCastTypes;
    int[] selectedSpellEffect;




    private string[] requirements = new string[] { "Level Requirement", "Specialization Requirement", "Class Requirement" };
    private int[] selectedRequirement = new int[MAX_REQUIREMENTS];
    private List<SkillRequirement> skillRequirements = new List<SkillRequirement>();
    int numberRequirements = 0;
    bool activeRequirements = true;
    string[] requisitos = new string[MAX_REQUIREMENTS];

    List<Vector2> activeBoxes = new List<Vector2>();


    private BancoHabilidades skills = new BancoHabilidades();




    public BHEditor()
    {

    }

    // Add menu named "My Window" to the Window menu
    [MenuItem("TRPG/Abilities",false,6)]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        BHEditor window = (BHEditor)EditorWindow.GetWindow(typeof(BHEditor));
        window.Show();
    }

    void OnGUI()
    {
        {
            switch (step)
            {
                case 0:
                    
                   
                    if (GUILayout.Button("New Skill", GUILayout.Height(40)))
                        step = 1;
                    else if (GUILayout.Button("Delete/Edit Skill", GUILayout.Height(40)))
                        step = 2;
                    
                    break;

                case 1:
                    // Creando una habilidad, definimos nombre, descripcion y tipo de daño que va a hacer 
                    skillName = EditorGUILayout.TextField("Skill name", skillName);
                    skillDescription = EditorGUILayout.TextField("Skill description", skillDescription);
                    manaCost = EditorGUILayout.DoubleField("Mana Cost", manaCost);

                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Type of skill");
                    selectedSpellType = EditorGUILayout.Popup(selectedSpellType, spelltype);
                    GUILayout.EndHorizontal();


                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Skill cast type");
                    selectedSkillType = EditorGUILayout.Popup(selectedSkillType, skillType);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Skill effect type");
                    selectedEffect = EditorGUILayout.Popup(selectedEffect, skillEffect);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Type of damage");
                    selectedSkillType1 = EditorGUILayout.Popup(selectedSkillType1, skillType1);
                    GUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Damage:");
                    damage = EditorGUILayout.IntField(damage);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Distance:");
                    distance = EditorGUILayout.IntField(distance);
                    EditorGUILayout.EndHorizontal();

                    /*
                    //Mostramos previsualización del rango de la habilidad.
                    active = EditorGUILayout.Foldout(active, "Previsual");
                    if (distance > 0 && active)
                    {
                        EditorGUILayout.BeginHorizontal(GUILayout.Width(1));

                        for (int i = 0; i < distance + distance + 1; i++)
                        {
                            EditorGUILayout.BeginVertical(GUILayout.Width(1));
                            for (int j = 0; j < distance + distance + 1; j++)
                            {
                                if (i == distance && j == distance)
                                {
                                    GUI.backgroundColor = Color.white;
                                    GUILayout.Box("", GUILayout.Width(15), GUILayout.Height(15));
                                }
                                else
                                {
                                    var box = new Vector2(i - distance, j - distance);
                                    selectColor(activeBoxes.Contains(box));
                                    GUILayout.Box("", GUILayout.Width(15), GUILayout.Height(15));
                                }
                            }
                            EditorGUILayout.EndVertical();
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    GUI.backgroundColor = Color.white;
                    */


                    
                    //Mostramos previsualización del rango de la habilidad.
                    if (GUILayout.Button("Draw area range"))
                        area = 1;

                    if (area > 0)
                    {

                        //Mostramos previsualización del rango de la habilidad.
                        active = EditorGUILayout.Foldout(active, "Previsual");

                        if (distance > 0 && active)
                        {

                            EditorGUILayout.BeginHorizontal(GUILayout.Width(1));

                            for (int i = 0; i < distance + distance + 1; i++)
                            {
                                EditorGUILayout.BeginVertical(GUILayout.Width(1));
                                for (int j = 0; j < distance + distance + 1; j++)
                                {
                                    if (i == distance && j == distance)
                                    {
                                        GUI.backgroundColor = Color.white;
                                        GUILayout.Box("", GUILayout.Width(15), GUILayout.Height(15));
                                    }
                                    else
                                    {
                                        var box = new Vector2(i - distance, j - distance);
                                        selectColor(activeBoxes.Contains(box));
                                        GUILayout.Box("", GUILayout.Width(15), GUILayout.Height(15));
                                    }

                                    if (Event.current.type == EventType.mouseDown
                                        && Event.current.button == 0
                                        && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
                                    {
                                        var box = new Vector2(i - distance, j - distance);
                                        if (activeBoxes.Contains(box)) activeBoxes.Remove(box);
                                        else activeBoxes.Add(box);
                                        Repaint();
                                    }
                                }
                                EditorGUILayout.EndVertical();
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                        GUI.backgroundColor = Color.white;
                    }

                    


                    //requisitos para la habilidad
                    activeRequirements = EditorGUILayout.Foldout(activeRequirements, "Requirements");

                    if (activeRequirements)
                    {

                        if (GUILayout.Button("Add Requirement"))
                            numberRequirements = numberRequirements + 1;

                        if (numberRequirements > 0)
                        {

                            for (int i = 0; i < numberRequirements; i++)
                            {
                                EditorGUILayout.BeginHorizontal();
                                selectedRequirement[i] = EditorGUILayout.Popup(selectedRequirement[i], requirements);
                                requisitos[i] = EditorGUILayout.TextField(requisitos[i]);

                                SkillRequirement require = new SkillRequirement(selectedRequirement[i], requisitos[i]);


                                if (GUILayout.Button("X", GUILayout.Width(30), GUILayout.Height(30)))
                                {
                                    //Ha pulsado borrar boton
                                    skillRequirements.Remove(require);
                                    numberRequirements--;
                                }

                                if (GUILayout.Button("Save", GUILayout.Width(50), GUILayout.Height(30)))
                                {
                                    skillRequirements.Add(require);

                                }



                                EditorGUILayout.EndHorizontal();

                            }
                        }

                    }


                    //fin de la creación de habilidad
                    EditorGUILayout.BeginHorizontal("Box");
                    if (GUILayout.Button("Cancel", GUILayout.Width(100), GUILayout.Height(50)))
                    {
                        setDefaults();
                        step = 0;
                    }
                    else if (GUILayout.Button("Create Skill", GUILayout.Width(100), GUILayout.Height(50)))
                    {
                        // Mensaje de salvado! -> aceptar -> step 1
                        saveSkill();
                        setDefaults();
                        step = 0;
                    }

                    
                    break;

                
                //SELECCIONA EDITAR UNA HABILIDAD  
                case 2:
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Name");
                    EditorGUILayout.LabelField("Description");
                    EditorGUILayout.LabelField("Type of skill");
                    EditorGUILayout.LabelField("Skill cast type");
                    EditorGUILayout.LabelField("Spell Effect");
                    EditorGUILayout.LabelField("Damage");
                    EditorGUILayout.LabelField("Distance");
                    EditorGUILayout.EndHorizontal();

                    Skill[] savedSkills = SkillsDB.Instance.getSavedSkills();
                    selectedSkillTypes = new int[savedSkills.Length];
                    selectedCastTypes = new int[savedSkills.Length];
                    selectedSpellEffect = new int[savedSkills.Length];

                    int[,] selectedRequirements = new int[savedSkills.Length, MAX_REQUIREMENTS];
                    string[,] descriptionRequirements = new string[savedSkills.Length, MAX_REQUIREMENTS];
                    for (int i = 0; i < savedSkills.Length; i++)
                    {
                        Skill skill = savedSkills[i];
                        EditorGUILayout.BeginHorizontal();
                        //el nombre no se puede modificar porque es la clave de la base de datos
                        EditorGUILayout.LabelField(skill.getName());

                        //changing the description
                        string skillDescription1 = EditorGUILayout.TextField(skill.getDescription());
                        skill.changeDescription(skillDescription1);
                      
                        //changing type of skill type
                        selectedSkillTypes[i] = EditorGUILayout.Popup(skill.getTypeCast(), spelltype);
                        skill.changeTypeSkill(selectedSkillTypes[i]);

                        //changing casting character
                        selectedCastTypes[i] = EditorGUILayout.Popup(skill.getCastCharacter(), skillType);
                        skill.changeCastSkill(selectedCastTypes[i]);

                        //changing the spell effect:
                        selectedSpellEffect[i] = EditorGUILayout.Popup(skill.getSkillEffect(), skillEffect);
                        skill.changeSkillEffect(selectedSpellEffect[i]);


                        //changing amount of damage
                        int damage1 = EditorGUILayout.IntField(skill.getDamage());
                        skill.changeSkillDamage(damage1);
                        //changing distance
                        int distance1 = EditorGUILayout.IntField(skill.getDistance());
                        skill.changeDistance(distance1);



                        //requisitos para la habilidad
                        EditorGUILayout.BeginVertical();
                        activeRequirements = EditorGUILayout.Foldout(activeRequirements, "Requirements");


                        if (activeRequirements)
                        {
                            if (skill.numberRequirements() > 0)
                            {
                                
                                for (int j = 0; j < skill.numberRequirements(); j++)
                                {
                                    EditorGUILayout.BeginHorizontal();
                                   
                                    selectedRequirements[i,j] = skill.getTypeRQ(j);
                                    selectedRequirements[i,j] = EditorGUILayout.Popup(selectedRequirements[i,j], requirements); 
                                    skill.changeTypeRQ(selectedRequirements[i,j], j);

                                    descriptionRequirements[i, j] = skill.getDescRQ(j);
                                    descriptionRequirements[i, j] = EditorGUILayout.TextField(skill.getDescRQ(j));
                                    skill.changeDescRQ(descriptionRequirements[i, j], j);

                                    SkillRequirement require = new SkillRequirement(skill.getTypeRQ(j), skill.getDescRQ(j));
                                   
                                    if (GUILayout.Button("X", GUILayout.Width(30), GUILayout.Height(30)))
                                    {

                                        skill.removeRQ(require);
                                        numberRequirements--;
                                        SkillsDB.Instance.updateSkill(skill);

                                    }

                                    EditorGUILayout.EndHorizontal();

                                }
                            }

                        }


                        SkillsDB.Instance.updateSkill(skill);


                        EditorGUILayout.EndVertical();

                        if (GUILayout.Button("X", GUILayout.Width(30), GUILayout.Height(30)))
                        {
                            //Ha pulsado borrar boton
                            this.skills.deleteSkill(skill);
                            SkillsDB.Instance.deleteSkill(skill);

                        }


                        EditorGUILayout.EndHorizontal();

                        

                    }

                    EditorGUILayout.BeginHorizontal("Box");
                    if (GUILayout.Button("Back to Menu", GUILayout.Width(100), GUILayout.Height(50)))
                    {
                        setDefaults();
                        step = 0;
                    }

                    break;

                default:
                    break;
            }
        }

    }



private void selectColor(bool active)
    {
        if (selectedSkillType1 == 0)
            GUI.backgroundColor = Color.green;
        else if (selectedSkillType1 == 1)
            GUI.backgroundColor = Color.red;
        else
            GUI.backgroundColor = Color.blue;

        if (active) GUI.backgroundColor = Color.Lerp(GUI.backgroundColor, Color.white, .8f);
    }


    private void setDefaults()
    {
        // Limpia todas las variables y estructuras temporales para crear la spec y las vuelve a dejar a sus
        // defaults
        damage = 0;
        distance = 0;
        step = 0;
        area = 0;
        skillName = "";
        skillDescription = "";
        selectedSpellType = 0;
        selectedSkillType = 0;
        selectedEffect = 0;
        selectedSkillType1 = 0;

        selectedSkillTypes = new int[0];
        requisitos = new string[MAX_REQUIREMENTS];
        selectedRequirement = new int[MAX_REQUIREMENTS];
        this.skillRequirements = new List<SkillRequirement>();



        numberRequirements = 0;
        activeRequirements = true;
    }

    private void saveSkill()
    {
        Skill skill = new Skill(this.skillName, this.skillDescription, this.manaCost, this.spelltype[this.selectedSpellType], this.skillType[this.selectedSkillType], this.skillEffect[this.selectedEffect], this.selectedSkillType1, this.damage,this.distance, this.skillRequirements, numberRequirements);
        this.skills.add(skill);
        SkillsDB.Instance.addSkill(skill);
    }

   
}

