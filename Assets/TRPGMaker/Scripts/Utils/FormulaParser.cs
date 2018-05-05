using UnityEngine;
using NCalc;
using System.Reflection;
using System.Linq;
using System;
using System.Collections.Generic;

[Serializable]
public class FormulaParser {

	private Expression expression;
	private string formula;
	private string paramError;
	private string functionError;
	private object expresionResult;
    private List<AttributeValue> attributes;


    public FormulaParser() : this(string.Empty) { }
	public FormulaParser(string formula)
	{
	    this.Formula = formula;
	}

	public string Formula
	{
	    get
	    {
	        return formula;
	    }
	    set
	    {
	        formula = value;
	        RegenerateExpression();
	    }
	}

	private System.Type desiredReturnType;
	public System.Type DesiredReturnType
	{
	    get
	    {
	        return desiredReturnType;
	    }
	    set
	    {
	        desiredReturnType = value;
	        RegenerateExpression();
	    }
	}

	private void RegenerateExpression()
	{
	    paramError = string.Empty;
	    if (!string.IsNullOrEmpty(formula))
	    {
	        try
	        {
	            expression = new Expression(this.formula);
	            expression.EvaluateParameter += CheckParameter;
	            expression.EvaluateFunction += EvaluateFunction;
	            expresionResult = expression.Evaluate();
	        }
	        catch { }
	    }
	}

	public bool IsValidExpression
	{
	    get
	    {
	        return !string.IsNullOrEmpty(formula) && string.IsNullOrEmpty(paramError) && string.IsNullOrEmpty(functionError) && !expression.HasErrors() && (desiredReturnType == null || expresionResult != null && expresionResult.GetType().Equals(desiredReturnType));
	    }
	}


	public string Error
	{
	    get
	    {
	        return
	            string.IsNullOrEmpty(formula)
	                ? "The formula can't be empty"
	                : !string.IsNullOrEmpty(paramError)
	                    ? paramError
	                    : !string.IsNullOrEmpty(functionError)
	                        ? functionError
	                        : desiredReturnType != null && !(expresionResult.GetType().Equals(desiredReturnType))
	                            ? "The formula doesn't result in a " + desiredReturnType.ToString() + " value."
	                            : expression.Error;
	    }
	}

	private void CheckParameter(string param, ParameterArgs args)
	{
		if (Database.Instance.attributes.Any(x => x.id == param)) 
		{
			args.HasResult = true;
            args.Result = attributes.Find(x => x.attribute.id == param).value;
		} 
	    else
		{
			args.HasResult = false;
			paramError = "Missing parameter \"" + param + "\"";
	    }
	}

	private void EvaluateFunction(string name, FunctionArgs args)
	{
	    
	}



	public object Evaluate(List<AttributeValue> attributes)
	{
        this.attributes = attributes;
	    return expression.Evaluate();
	}
}

public class FormulaException : System.Exception
{

	public FormulaException(string message) : base(message)
	{
	}

}