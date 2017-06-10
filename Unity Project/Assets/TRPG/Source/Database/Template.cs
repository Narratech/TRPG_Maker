using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class Template : ScriptableObject
    {
    #region Attributes
    [SerializeField]
    protected Sprite _image;  // Image that represents the Attribute
    [SerializeField]
    protected string _nameId;  // The name and unique identifier of the Template
    [SerializeField]
    protected string _description;  // The description of the Template
    [SerializeField]
    protected List<string> _tags;  // Every tag this Template has
    [SerializeField]
    protected List<Formula> _formulas;  // Every formula that modifies attributes for this Template
    [SerializeField]
    protected SlotsConfig _allowedSlots;  // Every slot allowed in this Template
    #endregion

    #region Constructor
    protected Template()
        {
        }

    #endregion

    protected Template Init(string nameId, string description, List<string> tags, List<Formula> formulas, SlotsConfig allowedSlots)
        {
        // Defined in templates
        name = _nameId = nameId;
        _description=description;
        _tags=tags;
        _formulas=formulas;
        _allowedSlots=allowedSlots;
        return this;
        }

    #region Properties    
    public string NameId
        {
        get { return _nameId; }
        set { name = _nameId = value; } 
        }

    public string Description
        {
        get { return _description; }
        set { _description=value; }
        }

    public List<string> Tags
        {
        get { return _tags; }
        set { _tags=value; }
        }

    public List<Formula> Formulas
        {
        get { return _formulas; }
        set { _formulas=value; }
        }

    public SlotsConfig AllowedSlots
        {
        get { return _allowedSlots; }
        set { _allowedSlots=value; } 
        }
    #endregion

    #region Methods
    public void store(Database database)
        {
        if(AssetDatabase.IsMainAsset(database) && !AssetDatabase.IsSubAsset(this))
            {
            AssetDatabase.AddObjectToAsset(this,database);
            _allowedSlots.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
            AssetDatabase.AddObjectToAsset(_allowedSlots, this);
            }
        //UnityEditor.AssetDatabase.SaveAssets(); 
        }
    #endregion
    }