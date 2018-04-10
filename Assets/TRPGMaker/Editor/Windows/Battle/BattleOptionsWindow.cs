using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class BattleOptionsWindow : EditorWindow
{
    private int selectedGameType = -1;
    private int selectedTurnStyle = -1;
    private int selectedHealthAttribte = -1;
    private int selectedMovementRange = -1;
    private int selectedMovementHeight = -1;
    private int selectedAttackRange = -1;
    private int selectedAttackHeight = -1;

    [MenuItem("TRPGMaker/Battle options", false, 1)]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(BattleOptionsWindow), false, "Battle options");
    }

    private void OnEnable()
    {
        if (Database.Instance.battleOptions != null)
        {
            this.selectedGameType = (int)Database.Instance.battleOptions.gameType;
            if (selectedGameType == (int)GameTypes.TRPG)
            {
                this.selectedTurnStyle = (int) Database.Instance.battleOptions.turnType;
                this.selectedHealthAttribte = Database.Instance.attributes.IndexOf(Database.Instance.battleOptions.healthAttribute);
                this.selectedMovementRange = Database.Instance.attributes.IndexOf(Database.Instance.battleOptions.moveRange);
                this.selectedMovementHeight = Database.Instance.attributes.IndexOf(Database.Instance.battleOptions.moveHeight);
                this.selectedAttackRange = Database.Instance.attributes.IndexOf(Database.Instance.battleOptions.attackRange);
                this.selectedAttackHeight = Database.Instance.attributes.IndexOf(Database.Instance.battleOptions.attackHeight);
            }
        }
    }

    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("Type of game:");
        selectedGameType = EditorGUILayout.Popup(selectedGameType, System.Enum.GetNames(typeof(GameTypes)));
        EditorGUILayout.EndHorizontal();

        // Case TRPG game type
        if (selectedGameType == (int) GameTypes.TRPG)
        {
            GUILayout.BeginVertical("Box");
            
            if (Database.Instance.battleOptions == null)
                Database.Instance.battleOptions = new TRPGOptions();

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Turn type:");
            selectedTurnStyle = EditorGUILayout.Popup(selectedTurnStyle, System.Enum.GetNames(typeof(TurnTypes)));
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Health attribute name:");
            selectedHealthAttribte = EditorGUILayout.Popup(selectedHealthAttribte, Database.Instance.attributes.Select(o => o.name).ToArray());
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Movement range attribute name:");
            selectedMovementRange = EditorGUILayout.Popup(selectedMovementRange, Database.Instance.attributes.Select(o => o.name).ToArray());
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Movement height attribute name:");
            selectedMovementHeight = EditorGUILayout.Popup(selectedMovementHeight, Database.Instance.attributes.Select(o => o.name).ToArray());
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Attack range attribute name:");
            selectedAttackRange = EditorGUILayout.Popup(selectedAttackRange, Database.Instance.attributes.Select(o => o.name).ToArray());
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Attack height attribute name:");
            selectedAttackHeight = EditorGUILayout.Popup(selectedAttackHeight, Database.Instance.attributes.Select(o => o.name).ToArray());
            EditorGUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck() || GUI.changed)
            {
                if (selectedTurnStyle != -1) Database.Instance.battleOptions.turnType = (TurnTypes) selectedTurnStyle;
                if(selectedHealthAttribte != -1) Database.Instance.battleOptions.healthAttribute = Database.Instance.attributes[selectedHealthAttribte];
                if (selectedMovementRange != -1) Database.Instance.battleOptions.moveRange = Database.Instance.attributes[selectedMovementRange];
                if (selectedMovementHeight != -1) Database.Instance.battleOptions.moveHeight = Database.Instance.attributes[selectedMovementHeight];
                if (selectedAttackRange != -1) Database.Instance.battleOptions.attackRange = Database.Instance.attributes[selectedAttackRange];
                if (selectedAttackHeight != -1) Database.Instance.battleOptions.attackHeight = Database.Instance.attributes[selectedAttackHeight];
                EditorUtility.SetDirty(Database.Instance);
            }

            GUILayout.EndVertical();
        }
    }

    private void OnDestroy()
    {
        EditorUtility.SetDirty(Database.Instance);
        AssetDatabase.SaveAssets();        
        AssetDatabase.Refresh();
    }

}