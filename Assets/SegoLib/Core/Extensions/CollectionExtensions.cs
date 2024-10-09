using System;
using System.Collections.Generic;
using System.Linq;

namespace SegoLib
{
    /// <summary>
    /// Extensions specifically for collections.
    /// </summary>
    public static class CollectionExtensions
    {



        /// <summary>
        /// Get a random item from an indexed collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T Random<T>(this IReadOnlyList<T> list)
        {
            return list[UnityEngine.Random.Range(0, list.Count)];
        }




        /// <summary>
        /// Determine if an index is valid within an indexed collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static bool IndexIsValid<T>(this IReadOnlyList<T> list, int index)
        {
            return index >= 0 && index < list.Count;
        }




        /// <summary>
        /// Get a random subset of items of size <paramref name="count"/> from an indexed collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="count">A number greater than 0.</param>
        /// <returns></returns>
        public static T[] RandomSubset<T>(this IReadOnlyList<T> list, int count)
        {
            if (list.Count == 0) throw new ArgumentException($"{nameof(list)} must have at least one element.");
            if (count <= 0) throw new ArgumentException($"{nameof(count)} must be greater than 0.");
            if (count >= list.Count) return list.ToArray();
            

            //Get a list of possible indeces in the collection
            List<int> availableIndeces = new List<int>();
            for(int i = 0; i < list.Count; i++) {
                availableIndeces.Add(i);
            }

            //Randomly <count> indeces from the list of available indeces, removing them from the available indeces as they're chosen.
            SortedSet<int> selectedIndeces = new SortedSet<int>();
            while (selectedIndeces.Count < count) {
                int i = UnityEngine.Random.Range(0, availableIndeces.Count);

                selectedIndeces.Add(availableIndeces[i]);
                availableIndeces.RemoveAt(i);
            }

            //Put the selected items into a new array of size <count>.
            T[] newArray = new T[count];
            int newIndex = 0;
            foreach (int oldIndex in selectedIndeces) {
                newArray[newIndex++] = list[oldIndex];
            }
            return newArray;
        }
    } 
}
