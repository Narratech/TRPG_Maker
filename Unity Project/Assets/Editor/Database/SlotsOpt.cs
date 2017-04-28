using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PassiveOpt
    {
    Dictionary<string,Dictionary<string,string>> _passiveOptions;
    public int passiveMask;
    public List<string> passiveIds;
    }

public struct ItemOpt
    {  
    public int itemMask;
    public List<string> itemIds;  
    }

public struct SpecOpt
    {  
    public int specMask;
    public List<string> specIds;  
    }

public class SlotsOpt
    {
    #region Attributes
    Dictionary<string,Dictionary<string,string>> _passiveOptions;  // _passiveOptions["When"]["Permanent"] returns "Permanent"
    ItemOpt _itemOptions;
    SpecOpt _specOptions;
    #endregion
    
    #region Constructor
    public SlotsOpt()
        {
        _passiveOptions=new Dictionary<string,Dictionary<string,string>>();
        _itemOptions.itemMask=0;
        _itemOptions.itemIds=new List<string>();
        _specOptions.specMask=0;
        _specOptions.specIds=new List<string>();
        }
    #endregion

    #region Properties
    public Dictionary<string,Dictionary<string,string>> PassiveOptions
        {
        get { return _passiveOptions; }
        set { value=_passiveOptions; }
        }

    public ItemOpt ItemOptions
        {
        get { return _itemOptions; }
        set { value=_itemOptions; }
        }

    public SpecOpt SpecOptions
        {
        get { return _specOptions; }
        set { value=_specOptions; }
        }
    #endregion
    
    #region Methods
    #endregion
    }
