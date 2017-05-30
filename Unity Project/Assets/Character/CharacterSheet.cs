using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSheet
    {
    #region Attributes
    Dictionary<string,Attribute> _attributes;
    SpecTemplate _class;  // Identifies the character class
    SpecTemplate _specialization;  // Identifies the character specialization
    List<ItemTemplate> _items;  // Identifies the item slots (could exist empty slots).
    List<PassiveTemplate> _passives;  // Identifies the passive slots (could exist empty slots)
    List<Formula> _tier1Formulas;
    List<Formula> _tier2Formulas;
    List<Formula> _tier3Formulas;
    #endregion

    #region Constructor
    public CharacterSheet(Dictionary<string,Attribute> attributes, SpecTemplate classs, SpecTemplate specialization,
        List<ItemTemplate> items, List<PassiveTemplate> passives)
        // Constructs the CharacterSheet
        {
        _attributes=attributes;
        _class=classs;
        _specialization=specialization;
        _items=items;
        _passives=passives;
        collectFormulas();
        updateSheet();
        }
    #endregion

    // Properties

    // Methods
    private void collectFormulas()
        {
        // 1. Search in '_class' for Formulas and store them by their 'Formula.priority' in their tier
        // 2. Search in '_specialization' for Formulas and store them by their 'Formula.priority' in their tier
        // 3. Search in '_items' for Formulas and store them by their 'Formula.priority' in their tier
        // 4. Search in '_passives' for Formulas and store them by their 'Formula.priority' in their tier just
        //    the passive is 'Permanent' and 'Self'
        }

    private void updateSheet()
        {
        // Modify attributes updating them in the tier order
        
        }
    }
