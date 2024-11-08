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
    public class Pronouns : IFormattable
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

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format)) return Subjective;

            switch (format.ToLower()) {
                case "he": return Subjective;
                case "him": return Objective;
                case "his": return Possessive_Adj;
                case "hers": return Possessive_Pro;
                case "self": return Reflexive;
                default: return Subjective;
            }
        }
    }
}
