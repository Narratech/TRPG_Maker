using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BHEditor : EditorWindow
{

    private int damage = 0;
    private int distance = 0;
    private int step = 0;
    private int area = 0;
    private string skillName;
    private string skillDescription;

    private bool active = true;

    private string[] objective = new string[] { "Self", "Ally", "Enemy", "Objetc", "All map" };
    private int selectedObjective;
    private string[] skillType = new string[] { "One Target", "All Enemies", "All Allies", "All Map", "Area" };
    private int selectedSkillType;
    private string[] skillType1 = new string[] { "Healing", "Damage", "Both" };
    int selectedSkillType1;

    List<Vector2> activeBoxes = new List<Vector2>();


    private BancoHabilidades skills = new BancoHabilidades();




    public BHEditor()
    {

    }

    // Add menu named "My Window" to the Window menu
    [MenuItem("Window/Banco de Habilidades")]
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
                    
                   
                    if (GUILayout.Button("Nueva Habilidad", GUILayout.Height(40)))
                        step = 1;
                    else if (GUILayout.Button("Borrar/Editar Habilidad", GUILayout.Height(40)))
                        step = 2;
                    
                    break;

                case 1:
                    // Creando una habilidad, definimos nombre, descripcion y tipo de daño que va a hacer 
                    skillName = EditorGUILayout.TextField("Skill name", skillName);
                    skillDescription = EditorGUILayout.TextField("Skill description", skillDescription);
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Skill type");
                    selectedSkillType = EditorGUILayout.Popup(selectedSkillType, skillType);
                    GUILayout.EndHorizontal();
                  
                    //Segun lo que se haya escogido pueden aparecer varias opciones.
                    switch (selectedSkillType)
                    {
                        //Daño/Sanacion a un solo objetivo
                        case 0:

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
                                        if(i == distance && j == distance)
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
                            break;

                        case 1:
                            //Se ha escogido una habilidad que selecciona a todos los enemigos
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Damage:");
                            damage = EditorGUILayout.IntField(damage);
                            EditorGUILayout.EndHorizontal();
                            break;
                        case 2:
                            //Se ha escogido una habilidad que selecciona a todos los aliados
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Healing:");
                            damage = EditorGUILayout.IntField(damage);
                            EditorGUILayout.EndHorizontal();
                            break;
                        case 3:
                            //Se ha escogido una habilidad que selecciona a todos los objetivos (amigos o enemigos del mapa)
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Damage:");
                            damage = EditorGUILayout.IntField(damage);
                            EditorGUILayout.EndHorizontal();
                            break;
                        case 4:
                            GUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Type of damage");
                            selectedSkillType1 = EditorGUILayout.Popup(selectedSkillType1, skillType1);
                            GUILayout.EndHorizontal();

                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField("Damage:");
                            damage = EditorGUILayout.IntField(damage);
                            EditorGUILayout.EndHorizontal();

                            //Mostramos previsualización del rango de la habilidad.
                            if (GUILayout.Button("Draw area range"))
                                area = 1;

                            if (area > 0)
                            {

                                /*
                                 * intento de crear una nueva ventana para definir el area, no funciona
                                area = 0;
                                // Get existing open window or if none, make a new one:
                                AreaEditor window = (AreaEditor)EditorWindow.GetWindow(typeof(AreaEditor));
                                window.Show();
                                */

                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField("Distance:");
                                distance = EditorGUILayout.IntField(distance);
                                EditorGUILayout.EndHorizontal();

                                //Mostramos previsualización del rango de la habilidad.
                                active = EditorGUILayout.Foldout(active,"Previsual");

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

                            break;
                        default:
                            break;
                    }

                    //fin de la creación de habilidad
                    EditorGUILayout.BeginHorizontal("Box");
                    if (GUILayout.Button("Cancel", GUILayout.Width(100), GUILayout.Height(50)))
                    {
                        setDefaults();
                        step = 0;
                    }
                    else if (GUILayout.Button("Save Skill", GUILayout.Width(100), GUILayout.Height(50)))
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
                    EditorGUILayout.LabelField("Skill Type");
                    EditorGUILayout.LabelField("Damage");
                    EditorGUILayout.LabelField("Distance");
                    EditorGUILayout.EndHorizontal();

                    Skill[] savedSkills = SkillsDB.Instance.getSavedSkills();
                    for (int i = 0; i < savedSkills.Length; i++)
                    {
                        Skill skill = savedSkills[i];
                        EditorGUILayout.BeginHorizontal();
                        //el nombre no se puede modificar porque es la clave de la base de datos
                        EditorGUILayout.LabelField(skill.getName());
                        skill.changeName(skillName);

                        string skillDescription1 = EditorGUILayout.TextField(skill.getDescription());
                        skill.changeDescription(skillDescription1);
                        int selectedSkillType1 = EditorGUILayout.Popup(selectedSkillType, skillType);
                        int damage1 = EditorGUILayout.IntField(skill.getDamage());
                        skill.changeSkillDamage(damage1);
                        int distance1 = EditorGUILayout.IntField(skill.getDistance());
                        skill.changeDistance(distance1);


                        SkillsDB.Instance.updateSkill(skill);

                        if (GUILayout.Button("X", GUILayout.Width(30), GUILayout.Height(30)))
                        {
                            //Ha pulsado borrar boton
                            this.skills.deleteSkill(skill);
                            SkillsDB.Instance.deleteSkill(skill);
                        }


                        EditorGUILayout.EndHorizontal();
                    }


                       /* for (int i = 0; i < this.skills.getSize(); i++)
                    {
                        Skill skill = this.skills.getHabilidad(i);
                        EditorGUILayout.BeginHorizontal();
                        skillName = EditorGUILayout.TextField(skill.getName());
                        skill.changeName(skillName);

                        string skillDescription1 = EditorGUILayout.TextField(skill.getDescription());
                        skill.changeDescription(skillDescription1);
                        int selectedSkillType1 =  EditorGUILayout.Popup(selectedSkillType, skillType);
                        int damage1 = EditorGUILayout.IntField(skill.getDamage());
                        skill.changeSkillDamage(damage1);
                        int distance1 = EditorGUILayout.IntField(skill.getDistance());
                        skill.changeDistance(distance1);


                        if (GUILayout.Button("X", GUILayout.Width(30), GUILayout.Height(30)))
                        {
                            //Ha pulsado borrar boton
                            this.skills.deleteSkill(skill);
                            SkillsDB.Instance.deleteSkill(skill);
                        }


                        EditorGUILayout.EndHorizontal();

                    }
                    */

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
    }

    private void saveSkill()
    {
        Skill skill = new Skill(this.skillName, this.skillDescription, this.skillType[this.selectedSkillType], this.damage,this.distance);
        this.skills.add(skill);

        SkillsDB.Instance.addSkill(skill);
    }

   
}

