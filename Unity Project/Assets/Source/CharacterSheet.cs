using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class CharacterSheet
    {
    #region Attributes
    // Basic info
    string _nameId;
    string _description;
        //Sprite _avatar;
    // Attributes (basic, core, derived)
    Dictionary<string,Attribute> _baseAttributes;
    Dictionary<string,Attribute> _realAttributes;
    SpecTemplate _class;  // Identifies the character class
    SpecTemplate _specialization;  // Identifies the character specialization
    List<ItemTemplate> _items;  // Identifies the item slots (could exist empty slots).
    List<PassiveTemplate> _passives;  // Identifies the passive slots (could exist empty slots)
    List<Formula> _tier1Formulas;  // Formulas that modify same attribute through absolut values [ex: 5 | -3]
    List<Formula> _tier2Formulas;  // Formulas that modify same attribute through percentage [ex: atr*0,1 | atr*(-0,3)] 
    List<Formula> _tier3Formulas;  // Formulas that modify other attribute through precentage [ex: dmd=str*0,1]
    #endregion

    #region Constructor
    public CharacterSheet(string nameId, string description, Dictionary<string,Attribute> attributes, 
        SpecTemplate classs, SpecTemplate specialization, List<ItemTemplate> items, List<PassiveTemplate> passives)
        // Constructs the CharacterSheet
        {
        _nameId=nameId;
        _description=description;
        _baseAttributes=attributes;
        _class=classs;
        _specialization=specialization;
        _items=items;
        _passives=passives;
        collectFormulas(); 
        updateSheet(null);
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

    public Dictionary<string,Attribute> Attributes
        {
        get { return _realAttributes; }
        }
    #endregion

    #region Methods
    private void collectFormulas()
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

    public void updateSheet(Dictionary<string,int> modifiedAttributes)
        {
        // Update _baseAttributes with info calculated in the game
        if (modifiedAttributes!=null)
            {
            foreach (KeyValuePair<string,int> keypair in modifiedAttributes)
                {
                _baseAttributes[keypair.Key].value=_baseAttributes[keypair.Key].value+keypair.Value;
                }
            }
        // Modify _realAttributes starting from the _baseAttributes
        _realAttributes=new Dictionary<string, Attribute>(_baseAttributes); 
        foreach (Formula copiedFormula in _tier1Formulas)
            {
            int formulaValue=0;
            Int32.TryParse(copiedFormula.formula, out formulaValue);
            _realAttributes[copiedFormula.label].value=_realAttributes[copiedFormula.label].value+formulaValue;
            }
        }
    #endregion
    }
