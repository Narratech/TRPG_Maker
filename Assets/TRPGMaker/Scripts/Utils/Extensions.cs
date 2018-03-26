﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

static class Extensions
{
    public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
    {
        return listToClone.Select(item => (T)item.Clone()).ToList();
    }
}
