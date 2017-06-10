using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public struct ItemConfig
    {  
    public int itemMask;
    public List<string> itemIds;  
    }

[System.Serializable]
public struct SpecConfig
    {  
    public int specMask;
    public List<string> specIds;   
    }

[System.Serializable]
public class ListWrapper
    {
    public List<string> myList;
    public ListWrapper(List<string> l) { myList = l; }
    }

public class SlotsConfig : ScriptableObject, ISerializationCallbackReceiver
    #region Info
    #endregion
    {
    #region Attributes
    // Attributes
    List<Dictionary<string,string>> _passiveCfg;
    [SerializeField]
    List<ItemConfig> _itemCfg;
    [SerializeField]
    List<SpecConfig> _specCfg;
    // Wrappers for _passiveCfg
    [SerializeField]
    List<ListWrapper> _passiveCfgKeys;
    [SerializeField]
    List<ListWrapper> _passiveCfgVals;
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

    public void OnBeforeSerialize()
        {
        _passiveCfgKeys = _passiveCfg.ConvertAll(d => new ListWrapper(d.Keys.ToList()));
        _passiveCfgVals = _passiveCfg.ConvertAll(d => new ListWrapper(d.Values.ToList())); 
        }

    public void OnAfterDeserialize()
        {
        var keysIterator = _passiveCfgKeys.GetEnumerator();
        var valsIterator = _passiveCfgVals.GetEnumerator();
        _passiveCfg = new List<Dictionary<string, string>>();

        while(keysIterator.MoveNext() && valsIterator.MoveNext())
            {
            var keyIterator = keysIterator.Current.myList.GetEnumerator();
            var valIterator = valsIterator.Current.myList.GetEnumerator();
            var d = new Dictionary<string, string>();
            _passiveCfg.Add(d);

            while(keyIterator.MoveNext() && valIterator.MoveNext())
                d.Add(keyIterator.Current, valIterator.Current);
            }
        }
    #endregion
    }
