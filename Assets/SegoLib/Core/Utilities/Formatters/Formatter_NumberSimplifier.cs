using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SegoLib
{
    [CreateAssetMenu(fileName = "Number Formatter", menuName = "SegoLib/Formatters/Number Simplifier")]
    public class Formatter_NumberSimplifier : Formatter
    {
        public override string Format(object obj)
        {
            try {
                return NumberSimplifier.GetSimplifiedNumber(Convert.ToDouble(obj));
            } catch(FormatException) {
                return obj.ToString();
            }
        }
    }
}
