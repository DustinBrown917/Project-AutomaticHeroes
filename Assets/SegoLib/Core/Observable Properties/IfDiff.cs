using System;
using System.Collections.Generic;
using UnityEngine;

namespace SegoLib.Observables
{
    /// <summary>
    /// A helper class for the setting of generic variables based on equality.
    /// "IfDiff.Set(...)" = "If originalItem != newItem, originalItem = newItem"
    /// </summary>
    public static class IfDiff
    {
        public static bool Set<T>(ref T originalItem, T newItem, bool itemsAreEqual)
        {
            if (!itemsAreEqual) {
                originalItem = newItem;
                return true;
            }
            return false;
        }

        public static bool Set<T>(ref T originalItem, T newItem, bool itemsAreEqual, Action onChange)
        {
            if (!itemsAreEqual) {
                originalItem = newItem;
                onChange?.Invoke();
                return true;
            }
            return false;
        }

        public static bool Set<T>(ref T originalItem, T newItem, bool itemsAreEqual, Action<T> onChange)
        {
            if (!itemsAreEqual) {
                originalItem = newItem;
                onChange?.Invoke(newItem);
                return true;
            }
            return false;
        }

        public static bool Set<T>(ref T originalItem, T newItem, bool itemsAreEqual, Action<T, T> onChange)
        {
            if (!itemsAreEqual) {
                T origin = originalItem;
                originalItem = newItem;
                onChange?.Invoke(originalItem, newItem);
                return true;
            }
            return false;
        }

        public static bool Set<T>(ref T originalItem, T newItem) where T : IEquatable<T>
        {
            if (!EqualityComparer<T>.Default.Equals(originalItem, newItem)) {
                originalItem = newItem;
                return true;
            }
            return false;
        }

        public static bool Set<T>(ref T originalItem, T newItem, Action onChange) where T : IEquatable<T>
        {
            if (!EqualityComparer<T>.Default.Equals(originalItem, newItem)) {
                originalItem = newItem;
                onChange?.Invoke();
                return true;
            }
            return false;
        }

        public static bool Set<T>(ref T originalItem, T newItem, Action<T> onChange) where T : IEquatable<T>
        {
            if (!EqualityComparer<T>.Default.Equals(originalItem, newItem)) {
                originalItem = newItem;
                onChange?.Invoke(newItem);
                return true;
            }
            return false;
        }

        public static bool Set<T>(ref T originalItem, T newItem, Action<T, T> onChange) where T : IEquatable<T>
        {
            if (!EqualityComparer<T>.Default.Equals(originalItem, newItem)) {
                T origin = originalItem;
                originalItem = newItem;
                onChange?.Invoke(origin, newItem);
                return true;
            }
            return false;
        }
    } 
}
