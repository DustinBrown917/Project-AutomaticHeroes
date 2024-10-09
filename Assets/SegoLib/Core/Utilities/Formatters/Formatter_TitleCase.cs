using System.Globalization;
using UnityEngine;

namespace SegoLib
{
    /// <summary>
    /// Adds a colon to the end of a string.
    /// </summary>
    [CreateAssetMenu(fileName = "Title Case Formatter", menuName = "SegoLib/Formatters/Title Case")]
    public class Formatter_TitleCase : Formatter
    {
        protected static TextInfo m_cultureTextInfo = new CultureInfo("en-US", false).TextInfo;

        public override string Format(object obj)
        {
            string str = Formatter.Default.Format(obj);
            return m_cultureTextInfo.ToTitleCase(str);
        }
    } 
}
