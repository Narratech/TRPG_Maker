using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
using UnityEditor;

public class CharacterSheet: ScriptableObject, ISerializationCallbackReceiver
    {
    #region Attributes
    // Basic info
    [SerializeField]
    string _nameId;
    [SerializeField]
    string _description;
        //Sprite _avatar;
    // Attributes (basic, core, derived)
    Dictionary<string,double> _baseAttributes;
    [SerializeField]
    List<string> _keyBaseAttr;
    [SerializeField]
    List<double> _valueBaseAttr;
    Dictionary<string,double> _realAttributes;
    // Class, specialization, items and passives
    [SerializeField]
    SpecTemplate _class;  // Identifies the character class
    [SerializeField]
    SpecTemplate _specialization;  // Identifies the character specialization
    [SerializeField]
    List<ItemTemplate> _items;  // Identifies the item slots (could exist empty slots).
    [SerializeField]
    List<PassiveTemplate> _passives;  // Identifies the passive slots (could exist empty slots)
    // Collected formulas
    List<Formula> _tier1Formulas;  // Formulas that modify same attribute through absolut values [ex: 5 | -3]
    List<Formula> _tier2Formulas;  // Formulas that modify same attribute through percentage [ex: atr*0,1 | atr*(-0,3)] 
    List<Formula> _tier3Formulas;  // Formulas that modify other attribute through precentage [ex: dmd=str*0,1]
    #endregion

    #region Constructor
    public CharacterSheet Init(string nameId, string description, Dictionary<string, AttributeTRPG> attributes, 
        SpecTemplate classs, SpecTemplate specialization, List<ItemTemplate> items, List<PassiveTemplate> passives)
        // Constructs the CharacterSheet
        {
        _nameId=nameId;
        _description=description;
        _baseAttributes=new Dictionary<string,double>();
        foreach (KeyValuePair<string,AttributeTRPG> kv in attributes)
            {
            _baseAttributes[kv.Key]=(double)kv.Value.value;
            }
        /*
        _baseAttributes=new Dictionary<string, IsoUnityBasicType>();
        foreach (var kv in attributes.Values.ToList().ConvertAll(v=>new KeyValuePair<string,object>(v.id,v.value)))
            {
            var value=IsoUnityTypeFactory.Instance.getIsoUnityType(kv.Value);
            _baseAttributes.Add(kv.Key,value as IsoUnityBasicType);
            #if UNITY_EDITOR
            if (Application.isEditor && !Application.isPlaying && (UnityEditor.AssetDatabase.IsMainAsset(this) || UnityEditor.AssetDatabase.IsSubAsset(this)))
                {
	            (value as ScriptableObject).hideFlags = HideFlags.HideInHierarchy;
	            UnityEditor.AssetDatabase.AddObjectToAsset(value as UnityEngine.Object, this);
                }
            #endif
            }
        */
        _class=classs;
        _specialization=specialization;
        _items=items;
        _passives=passives;
        return this;
        }
    #endregion

    #region Properties
    public string NameId
        {
        get { return _nameId; }
        }

    public string Description
        {
        get { return _description; }
        }

    public Dictionary<string,double> BaseAttributes
        {
        get {return _baseAttributes; }
        }

    public Dictionary<string, double> RealAttributes
        {
        get { return _realAttributes; }
        }
    #endregion

    #region Methods
    public void collectFormulas()
        // 1. Search in '_class' for Formulas and store them by their 'Formula.priority' in their tier (demo just tier1)
        // 2. Search in '_specialization' for Formulas and store them by their 'Formula.priority' in their tier (demo just tier1)
        // 3. Search in '_items' for Formulas and store them by their 'Formula.priority' in their tier (demo just tier1)
        // 4. Search in '_passives' for Formulas and store them by their in tier1 if they are 'Permanent', 'Self' (not in demo)
        // 5. Search in '_passives' for the rest of Formulas and treat them like they should be treated (not in demo)
        // [TO DO] Treat tier2 and tier3 formulas. Treat _passives
        {
        _tier1Formulas=new List<Formula>();
        _tier2Formulas=new List<Formula>();
        _tier3Formulas=new List<Formula>();
        foreach (Formula copiedFormula in _class.Formulas)
            {
            Formula newFormula=new Formula(copiedFormula);
            _tier1Formulas.Add(newFormula);  // Just tier1 version
            /*
            if (newFormula.priority==1)
                _tier1Formulas.Add(newFormula);
            else if (newFormula.priority==2)
                _tier2Formulas.Add(newFormula);
            else if (newFormula.priority==3)
                _tier2Formulas.Add(newFormula);
            */
            }
        foreach (Formula copiedFormula in _specialization.Formulas)
            {
            Formula newFormula=new Formula(copiedFormula);
            _tier1Formulas.Add(newFormula);  // Just tier1 version
            /*
            if (newFormula.priority==1)
                _tier1Formulas.Add(newFormula);
            else if (newFormula.priority==2)
                _tier2Formulas.Add(newFormula);
            else if (newFormula.priority==3)
                _tier2Formulas.Add(newFormula);
            */
            }
        foreach (ItemTemplate searchedItem in _items)
            {
            foreach (Formula copiedFormula in searchedItem.Formulas)
                {
                Formula newFormula=new Formula(copiedFormula);
                _tier1Formulas.Add(newFormula);  // Just tier1 version
                /*
                if (newFormula.priority==1)
                    _tier1Formulas.Add(newFormula);
                else if (newFormula.priority==2)
                    _tier2Formulas.Add(newFormula);
                else if (newFormula.priority==3)
                    _tier2Formulas.Add(newFormula);
                */
                }
            }
        }

    public void updateSheet(Dictionary<string,double> modifiedAttributes)
        {
        // Update _baseAttributes with info calculated in the game
        if (modifiedAttributes!=null)
            {
            foreach (KeyValuePair<string,double> keypair in modifiedAttributes)
                {
                _baseAttributes[keypair.Key]=_baseAttributes[keypair.Key]+keypair.Value;
                }
            }
        // Modify _realAttributes starting from the _baseAttributes
        /*
        _realAttributes=new Dictionary<string, double>(); 
        foreach (var kv in _baseAttributes)
            _realAttributes.Add(kv.Key,(double)kv.Value.Value);
        */
        _realAttributes=new Dictionary<string,double>(_baseAttributes); 
        foreach (Formula copiedFormula in _tier1Formulas)
            {
            int formulaValue=0;
            Int32.TryParse(copiedFormula.formula, out formulaValue); 
            _realAttributes[copiedFormula.label]=_realAttributes[copiedFormula.label]+(double)formulaValue;
            }
        }

    public void OnBeforeSerialize()
        {
        _keyBaseAttr =_baseAttributes.Keys.ToList();
        _valueBaseAttr=_baseAttributes.Values.ToList();
        }

    public void OnAfterDeserialize()
        {
        _baseAttributes= new Dictionary<string,double>();
        var skeysIterator = _keyBaseAttr.GetEnumerator();
        var svalsIterator = _valueBaseAttr.GetEnumerator(); 
        while (skeysIterator.MoveNext() && svalsIterator.MoveNext())
            _baseAttributes.Add(skeysIterator.Current, svalsIterator.Current); 
        }

    public void Store(CharacterManager cm)
        {
        if(AssetDatabase.IsMainAsset(cm) && !AssetDatabase.IsSubAsset(this))
            {
            AssetDatabase.AddObjectToAsset(this,cm);
            /*
            foreach (var kv in _baseAttributes)
                {
                if (!AssetDatabase.IsSubAsset(kv.Value))
                    {
                    kv.Value.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
                    AssetDatabase.AddObjectToAsset(kv.Value,cm);
                    }
                }
            */
            }        
        //UnityEditor.AssetDatabase.SaveAssets();
        } 
    #endregion
    }
