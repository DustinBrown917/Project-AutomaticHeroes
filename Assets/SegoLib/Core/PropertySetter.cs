using System;
using System.Runtime.CompilerServices;


namespace SegoLib
{
    public static class PropertySetter
    {
        /// <summary>
        /// Set the value of a property if <paramref name="valuesAreEqual"/> is false, raising <paramref name="onSet"/> if the property is successfully set.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="underlyingField">The underlying field of the property being set.</param>
        /// <param name="newValue">The value to set the underlying field to.</param>
        /// <param name="valuesAreEqual">Whether or <paramref name="underlyingField"/> and <paramref name="newValue"/> are equal. If they are, execution exits early and the property is not set.</param>
        /// <returns>True if <paramref name="underlyingField"/> was set to value. False if not.</returns>
        public static bool Set<T>(ref T underlyingField, T newValue, bool valuesAreEqual)
        {
            if (valuesAreEqual)
                return false;

            underlyingField = newValue;
            return true;
        }

        /// <summary>
        /// Set the value of a property if <paramref name="valuesAreEqual"/> is false, raising <paramref name="onSet"/> if the property is successfully set.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="underlyingField">The underlying field of the property being set.</param>
        /// <param name="newValue">The value to set the underlying field to.</param>
        /// <param name="valuesAreEqual">Whether or <paramref name="underlyingField"/> and <paramref name="newValue"/> are equal. If they are, execution exits early and the property is not set.</param>
        /// <returns>True if <paramref name="underlyingField"/> was set to value. False if not.</returns>
        public static bool Set<T>(ref T underlyingField, T newValue, bool valuesAreEqual, out T oldValue)
        {
            oldValue = underlyingField;
            if (valuesAreEqual)
                return false;

            underlyingField = newValue;
            return true;
        }

        /// <summary>
        /// Set the value of a property if <paramref name="valuesAreEqual"/> is false, raising <paramref name="onSet"/> if the property is successfully set.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="underlyingField">The underlying field of the property being set.</param>
        /// <param name="newValue">The value to set the underlying field to.</param>
        /// <param name="valuesAreEqual">Whether or <paramref name="underlyingField"/> and <paramref name="newValue"/> are equal. If they are, execution exits early and the property is not set.</param>
        /// <param name="onChanging">Raised just before <paramref name="newValue"/> is assigned to <paramref name="underlyingField"/>.</param>
        /// <returns>True if <paramref name="underlyingField"/> was set to value. False if not.</returns>
        public static bool Set<T>(ref T underlyingField, T newValue, bool valuesAreEqual, ValueChangingEvent<T> onChanging)
        {
            if (valuesAreEqual)
                return false;

            onChanging?.Invoke(underlyingField, newValue);

            underlyingField = newValue;
            return true;
        }

        /// <summary>
        /// Set the value of a property if <paramref name="valuesAreEqual"/> is false, raising <paramref name="onSet"/> if the property is successfully set.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="underlyingField">The underlying field of the property being set.</param>
        /// <param name="newValue">The value to set the underlying field to.</param>
        /// <param name="valuesAreEqual">Whether or <paramref name="underlyingField"/> and <paramref name="newValue"/> are equal. If they are, execution exits early and the property is not set.</param>
        /// <param name="onSet">Raised after <paramref name="underlyingField"/> is successfully set with <paramref name="callerMemberName"/> as its parameter.</param>
        /// <param name="callerMemberName"></param>
        /// <returns>True if <paramref name="underlyingField"/> was set to value. False if not.</returns>
        public static bool Set<T>(ref T underlyingField, T newValue, bool valuesAreEqual, Action<string> onSet, [CallerMemberName] string callerMemberName = null)
        {
            if (valuesAreEqual)
                return false;

            underlyingField = newValue;
            onSet?.Invoke(callerMemberName);
            return true;
        }

        /// <summary>
        /// Set the value of a property if <paramref name="valuesAreEqual"/> is false and <paramref name="newValue"/> is not null, raising <paramref name="onSet"/> if the property is successfully set.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="underlyingField">The underlying field of the property being set.</param>
        /// <param name="newValue">The value to set the underlying field to. If this is null, execution exits early and the property is not set.</param>
        /// <param name="valuesAreEqual">Whether or <paramref name="underlyingField"/> and <paramref name="newValue"/> are equal. If they are, execution exits early and the property is not set.</param>
        /// <param name="onSet">Raised after <paramref name="underlyingField"/> is successfully set with <paramref name="callerMemberName"/> as its parameter.</param>
        /// <param name="callerMemberName"></param>
        public static bool SetNotNull<T>(ref T underlyingField, T newValue, bool valuesAreEqual, Action<string> onSet, [CallerMemberName] string callerMemberName = null)
        {
            if (newValue == null)
                return false;

            return Set(ref underlyingField, newValue, valuesAreEqual, onSet, callerMemberName);
        }

        /// <summary>
        /// Set the value of a property if <paramref name="valuesAreEqual"/> is false and <paramref name="newValue"/> is not null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="underlyingField">The underlying field of the property being set.</param>
        /// <param name="newValue">The value to set the underlying field to. If this is null, execution exits early and the property is not set.</param>
        /// <param name="valuesAreEqual">Whether or <paramref name="underlyingField"/> and <paramref name="newValue"/> are equal. If they are, execution exits early and the property is not set.</param>
        /// <returns>True if <paramref name="underlyingField"/> was changed. False if not.</returns>
        public static bool SetNotNull<T>(ref T underlyingField, T newValue, bool valuesAreEqual)
        {
            if (newValue == null)
                return false;

            return Set(ref underlyingField, newValue, valuesAreEqual);
        }

        /// <summary>
        /// Set the value of a property if <paramref name="valuesAreEqual"/> is false and <paramref name="newValue"/> is not null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="underlyingField">The underlying field of the property being set.</param>
        /// <param name="newValue">The value to set the underlying field to. If this is null, execution exits early and the property is not set.</param>
        /// <param name="valuesAreEqual">Whether or <paramref name="underlyingField"/> and <paramref name="newValue"/> are equal. If they are, execution exits early and the property is not set.</param>
        /// <param name="onChanging">Raised just before <paramref name="newValue"/> is assigned to <paramref name="underlyingField"/>.</param>
        /// <returns>True if <paramref name="underlyingField"/> was changed. False if not.</returns>
        public static bool SetNotNull<T>(ref T underlyingField, T newValue, bool valuesAreEqual, ValueChangingEvent<T> onChanging)
        {
            if (newValue == null)
                return false;

            return Set(ref underlyingField, newValue, valuesAreEqual, onChanging);
        }

        /// <summary>
        /// Sets the value of <paramref name="underlyingField"/> to <paramref name="newValue"/> if <paramref name="valuesAreEqual"/> is false and <paramref name="newValue"/> is not null.<br/>
        /// If <paramref name="newValue"/> is null, then <paramref name="underlyingField"/> will be set to <paramref name="nullFallback"/> instead, assuming the two are not equal.<br/>
        /// After successfully setting <paramref name="underlyingField"/>, <paramref name="onSet"/> is raised with <paramref name="callerMemberName"/> as its parameter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="underlyingField"></param>
        /// <param name="newValue"></param>
        /// <param name="nullFallback"></param>
        /// <param name="valuesAreEqual"></param>
        /// <param name="onSet"></param>
        /// <param name="callerMemberName"></param>
        public static bool SetNotNull<T>(ref T underlyingField, T newValue, T nullFallback, Func<T, T, bool> valuesAreEqual, Action<string> onSet, [CallerMemberName] string callerMemberName = null)
        {
            if (newValue == null)
                newValue = nullFallback;

            return Set(ref underlyingField, newValue, valuesAreEqual(underlyingField, newValue), onSet, callerMemberName);
        }

        /// <summary>
        /// Sets the value of <paramref name="underlyingField"/> to <paramref name="newValue"/> if <paramref name="valuesAreEqual"/> is false and <paramref name="newValue"/> is not null.<br/>
        /// If <paramref name="newValue"/> is null, then <paramref name="underlyingField"/> will be set to <paramref name="nullFallback"/> instead, assuming the two are not equal.<br/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="underlyingField"></param>
        /// <param name="newValue"></param>
        /// <param name="nullFallback"></param>
        /// <param name="valuesAreEqual"></param>
        public static bool SetNotNull<T>(ref T underlyingField, T newValue, T nullFallback, Func<T, T, bool> valuesAreEqual)
        {
            if (newValue == null)
                newValue = nullFallback;

            return Set(ref underlyingField, newValue, valuesAreEqual(underlyingField, newValue));
        }
    } 
}
