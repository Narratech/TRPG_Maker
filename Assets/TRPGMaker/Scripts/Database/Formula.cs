using UnityEngine;
using NCalc;
using System.Reflection;

public class Formula : ScriptableObject {

	public static Formula Create(string formula)
	{
	    var r = ScriptableObject.CreateInstance<Formula>();
	    r.formula = formula;
	    return r;
	}

	[SerializeField]
	private string _formula = "";
	public string formula
	{
	    get { return _formula; }
	    set
	    {
	        name = value;
	        _formula = value;
            FormulaParser.Formula = _formula;
	    }
	}

	public FormulaParser FormulaParser { get; private set; }

	void Awake()
	{
        FormulaParser = new FormulaParser();
	}

	void OnEnable()
	{
        FormulaParser = new FormulaParser();
        FormulaParser.Formula = _formula;
	}


	public bool check()
	{
	    var r = FormulaParser.Evaluate();
	    return r is bool ? (bool)r : false;
	}

	public override string ToString()
	{
	    return this._formula;
	}
}