using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SegoLib
{
    public static class ListExtensions
    {
        public static int OrderedInsert<T>(this List<T> list, T obj)
        {
            int i = list.BinarySearch(obj);

            if (i < 0)
                i = ~i;

            list.Insert(i, obj);
            return i;
        }

        public static int OrderedInsert<T>(this List<T> list, T obj, IComparer<T> comparer)
        {
            int i = list.BinarySearch(obj, comparer);

            if(i < 0) 
                i = ~i;

            list.Insert(i, obj);
            return i;
        }
    }

}