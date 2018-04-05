using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TRPGOptions  {
    [SerializeField]
    private GameTypes _gameType;

    public GameTypes gameType
    {
        get
        {
            return _gameType;
        }
        set
        {
            _gameType = value;
        }
    }

    [SerializeField]
    private TurnTypes _turnType;

    public TurnTypes turnType
    {
        get
        {
            return _turnType;
        }
        set
        {
            _turnType = value;
        }
    }

    [SerializeField]
    private Attribute _healthAttribute;

    public Attribute healthAttribute
    {
        get
        {
            return _healthAttribute;
        }
        set
        {
            _healthAttribute = value;
        }
    }

    [SerializeField]
    private Attribute _moveRange;

    public Attribute moveRange
    {
        get
        {
            return _moveRange;
        }
        set
        {
            _moveRange = value;
        }
    }
    [SerializeField]
    private Attribute _moveHeight;

    public Attribute moveHeight
    {
        get
        {
            return _moveHeight;
        }
        set
        {
            _moveHeight = value;
        }
    }

    [SerializeField]
    private Attribute _attackRange;

    public Attribute attackRange
    {
        get
        {
            return _attackRange;
        }
        set
        {
            _attackRange = value;
        }
    }

    [SerializeField]
    private Attribute _attackHeight;

    public Attribute attackHeight
    {
        get
        {
            return _attackHeight;
        }
        set
        {
            _attackHeight = value;
        }
    }
}
