using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SegoLib
{
    public static class StringExtensions
    {
        //private static readonly HashSet<char> m_charsBeforeTitle = new HashSet<char>() {
        //    ' ', '\n', '\t', '\r', '-'
        //};

        //public static string ToTitle(this string str)
        //{
        //    char[] newStr = new char[str.Length];

        //    if (char.IsLetter(str[0])) {
        //        newStr[0] = char.ToUpper(str[0]);
        //    } else {
        //        newStr[0] = str[0];
        //    }

        //    for(int i = 1; i < str.Length; i++) {
        //        if (m_charsBeforeTitle.Contains(str[i-1]) && char.IsLetter(str[i])){
        //            newStr[i] = char.ToUpper(str[i]);
        //        } else {
        //            newStr[i] = str[i];
        //        }
        //    }

        //    return new string(newStr);
        //}
    } 
}
