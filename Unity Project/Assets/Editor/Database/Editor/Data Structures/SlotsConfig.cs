using System.Collections.Generic;

public struct PassiveConfig
    // En principio no se usa
    {
    List<Dictionary<string,string>> _passivePairs;
    }

public struct ItemConfig
    {  
    public int itemMask;
    public List<string> itemIds;  
    }

public struct SpecConfig
    {  
    public int specMask;
    public List<string> specIds;  
    }

public class SlotsConfig
    #region Info
    #endregion
    {
    #region Attributes
    List<Dictionary<string,string>> _passiveCfg;
    List<ItemConfig> _itemCfg;
    List<SpecConfig> _specCfg;
    #endregion
    
    #region Constructor
    public SlotsConfig()
        {
        _passiveCfg=new List<Dictionary<string,string>>();
        _itemCfg=new List<ItemConfig>();
        _specCfg=new List<SpecConfig>();
        }
    #endregion

    #region Properties
    public List<Dictionary<string,string>> PassiveCfg
        {
        get { return _passiveCfg; }
        set { _passiveCfg=value; }
        }

    public List<ItemConfig> ItemCfg
        {
        get { return _itemCfg; }
        set { _itemCfg=value; }
        }

    public List<SpecConfig> SpecCfg
        {
        get { return _specCfg; }
        set { _specCfg=value; }
        }
    #endregion
    }
