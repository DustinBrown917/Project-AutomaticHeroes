using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;

namespace SegoLib
{
    public static class StringUtils
    {
        private static TextInfo m_textInfo = CultureInfo.CurrentCulture.TextInfo;

        public static string ToTitleCase(this string str)
        {
            return m_textInfo.ToTitleCase(str);
        }
    } 
}
