using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CharacterScript : MonoBehaviour {

    private Vector2 pos;
    [SerializeField]
    public Character character;

    private void Start()
    {

    }

    private void OnGUI()
    {
        if (character != null && character.attributes.Any(x => x.id == "HP"))
        {
            Attribute attribute = character.attributes.Find(x => x.id == "HP");
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
