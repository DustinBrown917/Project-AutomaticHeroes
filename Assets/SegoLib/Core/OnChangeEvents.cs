using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SegoLib
{
    public delegate void ValueUpdatedEvent<T>(T oldValue);
    public delegate void ValueUpdatedWithSourceEvent<TSource, TValue>(TSource source, TValue newValue);

    public delegate void ValueChangingEvent<T>(T oldValue, T newValue);
    public delegate void ValueChangingWithSourceEvent<TSource, TValue>(TSource source, TValue oldValue, TValue newValue);

    public delegate void ValueChangedEvent<T>(T oldValue, T newValue);
    public delegate void ValueChangedWithSourceEvent<TSource, TValue>(TSource source, TValue oldValue, TValue newValue);
}
