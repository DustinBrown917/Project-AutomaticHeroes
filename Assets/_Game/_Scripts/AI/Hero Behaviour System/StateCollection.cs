using System.Collections.Generic;

namespace Game
{
    public partial interface IHeroBehaviour
    {
        /// <summary>
        /// Handles the indexed management of several states. Ensures that states in the collection always have sequential indeces with no gaps.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        protected class StateCollection<T> where T : IState_Internal
        {
            /// <summary>
            /// Get the state at <paramref name="index"/>.
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            public T this[int index] => m_states[index];

            /// <summary>
            /// The states in this collection.
            /// </summary>
            private readonly List<T> m_states = new();

            /// <summary>
            /// The number of states in this collection.
            /// </summary>
            public int Count => m_states.Count;

            /// <summary>
            /// Add a state to the collection.
            /// </summary>
            /// <param name="item"></param>
            public void Add(T item)
            {
                m_states.Add(item);
                item.UpdateIndex(Count - 1);
            }

            /// <summary>
            /// Remove the state at <paramref name="index"/> from this collection.
            /// </summary>
            /// <param name="index"></param>
            public void RemoveAt(int index)
            {
                if (index < 0 || index >= m_states.Count)
                    return;

                m_states.RemoveAt(index);

                UpdateIndeces(index);
            }

            /// <summary>
            /// Update the indeces of all states in this collection. Used when a state is removed to ensure all state indeces are sequential with no gaps.
            /// </summary>
            /// <param name="fromIndex"></param>
            private void UpdateIndeces(int fromIndex = 0)
            {
                for (int i = fromIndex; i < m_states.Count; i++) {
                    m_states[i].UpdateIndex(i);
                }
            }
        }
    }
}
