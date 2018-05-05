using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

static class Extensions
{
    public static IList<AttributeValue> Clone<AttributeValue>(this IList<Attribute> listToClone) 
    {
        return listToClone.Select(item => (AttributeValue)item.Clone()).ToList();
    }
}
