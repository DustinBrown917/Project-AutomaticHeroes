using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Causes
{
    public interface ICauseFavour
    {
        public CauseDef Cause { get; }
        public float Favour { get; }
    }

    [System.Serializable]
    public class CauseFavourTracker : SegoLib.IInitializable
    {

        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/
        #region ========================================== Subclasses ==================================================
        /*vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv*/

        [System.Serializable]
        private class CauseFavour : ICauseFavour, IComparable<CauseFavour>
        {
            [field: SerializeField] public CauseDef Cause { get; private set; }
            [field: SerializeField] public float Favour { get; set; }

            [System.NonSerialized] public int order;

            public CauseFavour(CauseDef cause)
            {
                Cause = cause;
            }

            public int CompareTo(CauseFavour other)
            {
                return other.Favour.CompareTo(Favour);
            }
        }

        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        #endregion ===================================== End Subclasses ================================================
        /*^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/


        [field: SerializeField] public int TotalAvailableFavour { get; private set; } = 100;

        public bool IsInitialized { get; private set; }

        public IReadOnlyList<ICauseFavour> CauseFavours => m_causeFavours;
        [SerializeField] private CauseFavour[] m_causeFavours = Array.Empty<CauseFavour>();

        public IReadOnlyList<ICauseFavour> CauseFavoursSortedByHighest => m_sorted;
        [System.NonSerialized] private CauseFavour[] m_sorted = Array.Empty<CauseFavour>();

        public event Action<CauseFavourTracker> Updated;

        public void Initialize()
        {
            RefreshCauseFavourArray();
            IsInitialized = true;
        }

        /// <summary>
        /// Rebuild the cause favours array based on the causes and their runtime indeces. 
        /// Preserves any favour for each cause that might've been set up in editor.
        /// </summary>
        private void RefreshCauseFavourArray()
        {
            CauseFavour[] oldCauseFavour = m_causeFavours;

            m_causeFavours = new CauseFavour[CauseDef.All.Count];
            m_sorted = new CauseFavour[CauseDef.All.Count];

            // Create new favour for each cause
            foreach (CauseDef causeDef in CauseDef.All) {
                m_causeFavours[causeDef.RuntimeIndex] = m_sorted[causeDef.RuntimeIndex] = new CauseFavour(causeDef);
            }

            float sumOfFavour = 0;

            // Apply favour values that were previously set up in editor.
            if (oldCauseFavour != null) {
                foreach (CauseFavour causeFavour in oldCauseFavour) {
                    if (causeFavour.Cause == null)
                        continue;

                    m_causeFavours[causeFavour.Cause.RuntimeIndex].Favour += causeFavour.Favour;
                    sumOfFavour += causeFavour.Favour;
                }
            }

            // If the values set up in editor don't fill the TotalAvailableFavour, add the missing amount to causeless.
            if (sumOfFavour < TotalAvailableFavour) {
                m_causeFavours[0].Favour += TotalAvailableFavour - sumOfFavour;
                // If the values set up in editor sum to greater than TotalAvailableFavour, proportionally subtract the excess from each cause.
            } else if (sumOfFavour > TotalAvailableFavour) {
                float surplus = sumOfFavour - TotalAvailableFavour;
                foreach (CauseFavour causeFavour in m_causeFavours) {
                    causeFavour.Favour -= GetFavourPercent(causeFavour.Cause) * surplus;
                }
            }

            Sort();
            Updated?.Invoke(this);
        }


        /// <summary>
        /// Add favour to <paramref name="cause"/>, subtracting it porportionally from the other causes.
        /// </summary>
        /// <param name="cause"></param>
        /// <param name="amount"></param>
        public void AddFavour(CauseDef cause, float amount)
        {
            // Get how much available favour there is outside of what's already owned by cause
            float totalFavourLessTargetCause = TotalAvailableFavour - m_causeFavours[cause.RuntimeIndex].Favour;

            // The amount we're trying to add must not be greater than the amount that's available.
            if (amount > totalFavourLessTargetCause)
                amount = totalFavourLessTargetCause;

            if (amount <= 0)
                return;

            // Subtract from each other favour an amount proportional to how much of the available favour they currently hold.
            foreach (CauseFavour favour in m_causeFavours) {
                if (favour.Cause == cause)
                    continue;

                favour.Favour -= amount * (favour.Favour / totalFavourLessTargetCause);
            }

            // Finally, add amount to the target cause.
            m_causeFavours[cause.RuntimeIndex].Favour += amount;

            Sort();
            Updated?.Invoke(this);
        }

        /// <summary>
        /// Remove favour from <paramref name="cause"/> and move it to causeless.
        /// </summary>
        /// <param name="cause"></param>
        /// <param name="amount"></param>
        public void SubtractFavour(CauseDef cause, float amount)
        {
            if (cause.RuntimeIndex == 0 || amount <= 0)
                return;

            if (amount > m_causeFavours[cause.RuntimeIndex].Favour)
                amount = m_causeFavours[cause.RuntimeIndex].Favour;

            m_causeFavours[cause.RuntimeIndex].Favour -= amount;

            m_causeFavours[0].Favour += amount;

            Sort();
            Updated?.Invoke(this);
        }


        public float GetFavourPercent(CauseDef cause)
        {
            if (TotalAvailableFavour == 0)
                return 0;

            return m_causeFavours[cause.RuntimeIndex].Favour / TotalAvailableFavour;
        }

        public int GetFavourRank(CauseDef cause)
        {
            return m_causeFavours[cause.RuntimeIndex].order;
        }


        private void Sort()
        {
            Array.Sort(m_sorted);

            for (int i = 0; i < m_sorted.Length; i++) {
                m_sorted[i].order = i;
            }
        }
    }
}
