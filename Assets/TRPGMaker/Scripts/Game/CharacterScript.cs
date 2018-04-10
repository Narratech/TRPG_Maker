using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

public class CharacterScript : MonoBehaviour {

    private Vector2 pos;
    [SerializeField]
    public Character character;
    public Team team;

    private void Start()
    {

    }

    private void OnGUI()
    {
        if (character != null)
        {
            Attribute attribute = character.attributes.Find(x => x.id == Database.Instance.battleOptions.healthAttribute.id);
            var renderer = gameObject.GetComponent<Renderer>();
            float height = renderer.bounds.size.y * 15;
            pos = Camera.main.WorldToScreenPoint(transform.position);
            pos.y = Screen.height - pos.y - height;
            GUI.Box(new Rect(pos.x - 50, pos.y - 40, 100, 20), attribute.value + "/" + attribute.maxValue);
        }  
    }


    void Update()
    {
        
    }
}

[CustomEditor(typeof(CharacterScript))]
public class CharacterScriptEditor : Editor
{
    private int index = -1;
    private CharacterScript characterScript;

	private void OnEnable()
	{
        characterScript = (CharacterScript)target;
        if (characterScript.team != null) {
            index = Database.Instance.teams.IndexOf(characterScript.team);
        }
	}

	public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty character = serializedObject.FindProperty("character");
        EditorGUILayout.PropertyField(character);
        characterScript = (CharacterScript)target;

        EditorGUI.BeginChangeCheck();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Team");
        index = EditorGUILayout.Popup(index, Database.Instance.teams.Where(x => x.characters.Exists(y => y.name == characterScript.character.name)).Select(item => item.name).ToArray());
        EditorGUILayout.EndHorizontal();

        if (EditorGUI.EndChangeCheck()) {
            characterScript.team = Database.Instance.teams.Where(x => x.characters.Exists(y => y.name == characterScript.character.name)).ToArray()[index];
        }
        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(target);
    }
}