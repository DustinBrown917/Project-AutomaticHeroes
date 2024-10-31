using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "New Gender Definition", menuName = "Game/Gender" )]
    public class GenderDef : ScriptableObject
    {
        [field: SerializeField] public Pronouns Pronouns { get; private set; } = new Pronouns();
    }

    [System.Serializable]
    public class Pronouns
    {
        [field: Tooltip("He/She/They")]
        [field: SerializeField] public string Subjective { get; private set; } = "He/She/They";
        [field: Tooltip("Him/Her/Them")]
        [field: SerializeField] public string Objective { get; private set; } = "Him/Her/Them";
        [field: Tooltip("His/Hers/Theirs")]
        [field: SerializeField] public string Possessive_Pro { get; private set; } = "His/Hers/Theirs";
        [field: Tooltip("His/Her/Their")]
        [field: SerializeField] public string Possessive_Adj { get; private set; } = "His/Her/Their";
        [field: Tooltip("Himself/Herself/Themself")]
        [field: SerializeField] public string Reflexive { get; private set; } = "Himself/Herself/Themself";
    }

    public class HeroFormatProvider : IFormatProvider, ICustomFormatter
    {
        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (arg is not Hero hero)
                return HandleOtherFormats(format, arg);

            switch (format.ToLower()) {
                case "he": return hero.Core.Gender.Pronouns.Subjective;
                case "him": return hero.Core.Gender.Pronouns.Objective;
                case "his": return hero.Core.Gender.Pronouns.Possessive_Adj;
                case "hers": return hero.Core.Gender.Pronouns.Possessive_Pro;
                case "himself": return hero.Core.Gender.Pronouns.Reflexive;
                default: return hero.ToString();
            }
        }

        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(ICustomFormatter))
                return this;
            else
                return null;
        }

        private string HandleOtherFormats(string format, object arg)
        {
            if (arg is IFormattable)
                return ((IFormattable)arg).ToString(format, CultureInfo.CurrentCulture);
            else if (arg != null)
                return arg.ToString();
            else
                return String.Empty;
        }
    }
}
