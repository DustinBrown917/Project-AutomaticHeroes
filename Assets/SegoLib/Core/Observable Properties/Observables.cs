using System;
using System.Collections.Generic;
using UnityEngine;

namespace SegoLib.Observables
{
    public delegate void ValueChangedEvent<T>(T oldValue, T newValue);

    public interface IReadOnlyObservable<T>
    {
        public T Value { get; }
        public event ValueChangedEvent<T> onChanged;
    }


    public interface IObservable<T> : IReadOnlyObservable<T>
    {
        public new T Value { get; set; }

        public void SetValueWithoutNotify(T value);
        public void SetValue(T value);
        public void SetValue(T value, Func<T, T, bool> areValuesEqual);
    }



    /// <summary>
    /// Fires an event when the wrapped value changes
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [System.Serializable]
    public class Observable<T> : IObservable<T>
    {
        public static implicit operator T(Observable<T> obj) => obj.m_value;


        [SerializeField] protected T m_value;
        public T Value
        {
            get => m_value;
            set => SetValue(value);
        }

        public event ValueChangedEvent<T> onChanged;

        public Observable() : this(default(T)) { }

        public Observable(T defaultValue)
        {
            m_value = defaultValue;
        }

        protected virtual bool ItemsAreEqual(T a, T b)
        {
            return EqualityComparer<T>.Default.Equals(a, b);
        }

        public void SetValueWithoutNotify(T value)
        {
            m_value = value;
        }

        public void SetValue(T newValue)
        {
            if (ItemsAreEqual(m_value, newValue))
                return;

            T oldValue = m_value;
            m_value = newValue;
            onChanged?.Invoke(oldValue, newValue);
        }

        public void SetValue(T newValue, Func<T, T, bool> areValuesEqual)
        {
            if (areValuesEqual(m_value, newValue))
                return;

            T oldValue = m_value;
            m_value = newValue;
            onChanged?.Invoke(oldValue, newValue);
        }
    }










    public class ObservableFloat : Observable<float>
    {
        public ObservableFloat(float defaultValue) : base(defaultValue) { }
        protected override bool ItemsAreEqual(float a, float b) => a == b;
    }
    public class ObservableDouble : Observable<double>
    {
        public ObservableDouble(double defaultValue) : base(defaultValue) { }
        protected override bool ItemsAreEqual(double a, double b) => a == b;
    }

    public class ObservableLong : Observable<long>
    {
        public ObservableLong(long defaultValue) : base(defaultValue) { }
        protected override bool ItemsAreEqual(long a, long b) => a == b;
    }
    public class ObservableUlong : Observable<ulong>
    {
        public ObservableUlong(ulong defaultValue) : base(defaultValue) { }
        protected override bool ItemsAreEqual(ulong a, ulong b) => a == b;
    }
    public class ObservableInt : Observable<int>
    {
        public ObservableInt(int defaultValue) : base(defaultValue) { }
        protected override bool ItemsAreEqual(int a, int b) => a == b;
    }
    public class ObservableUint : Observable<uint>
    {
        public ObservableUint(uint defaultValue) : base(defaultValue) { }
        protected override bool ItemsAreEqual(uint a, uint b) => a == b;
    }
    public class ObservableShort : Observable<short>
    {
        public ObservableShort(short defaultValue) : base(defaultValue) { }
        protected override bool ItemsAreEqual(short a, short b) => a == b;
    }
    public class ObservableUshort : Observable<ushort>
    {
        public ObservableUshort(ushort defaultValue) : base(defaultValue) { }
        protected override bool ItemsAreEqual(ushort a, ushort b) => a == b;
    }
    public class ObservableByte : Observable<byte>
    {
        public ObservableByte(byte defaultValue) : base(defaultValue) { }
        protected override bool ItemsAreEqual(byte a, byte b) => a == b;
    }
    public class ObservableSbyte : Observable<sbyte>
    {
        public ObservableSbyte(sbyte defaultValue) : base(defaultValue) { }
        protected override bool ItemsAreEqual(sbyte a, sbyte b) => a == b;
    }
    public class ObservableString : Observable<string>
    {
        public ObservableString(string defaultValue) : base(defaultValue) { }
        protected override bool ItemsAreEqual(string a, string b) => a == b;
    }


}
