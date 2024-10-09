using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SegoLib;

namespace SegoLib.Tickers
{
    public class Ticker : IIdentifiable<Identity>
    {
        public delegate void OnTickDelegate(uint deltaTicks);

        public abstract class TickerKey
        {
            public Ticker Ticker { get; }
            public int Key { get; protected set; }
            public bool IsDecomissioned { get; protected set; }

            protected TickerKey(Ticker ticker, int key)
            {
                this.Ticker = ticker;
                Key = key;
            }

            public void Unregister()
            {
                Ticker.Unregister(this);
            }
        }

        private class InternalTickerKey : TickerKey
        {
            public event Action<InternalTickerKey> onDecomission;

            public InternalTickerKey(Ticker ticker, int key) : base(ticker, key)
            {
            }

            public void OnKeyAdded(int keyNum)
            {
                if (keyNum <= Key)
                    Key++;
            }

            public void OnKeyRemoved(int keyNum)
            {
                if (keyNum > Key)
                    Key--;
            }

            public void Decomission()
            {
                IsDecomissioned = true;
                onDecomission?.Invoke(this);
                Key = -1;
                onDecomission = null;
            }
        }

        /// <summary>
        /// Identity of this ticker.
        /// </summary>
        public Identity Id { get; }
        public float TicksPerSecond { get; }
        private readonly float m_secondsPerTick;
        private float m_time = 0;

        /// <summary>
        /// How many times has this ticker ticked.
        /// </summary>
        public ulong ElapsedTicks { get; private set; }
        public bool Enabled { get; set; } = false;
        public bool Decomissioned { get; private set; }

        private readonly List<InternalTickerKey> m_tickerKeys = new List<InternalTickerKey>();
        private readonly List<OnTickDelegate> m_delegates = new List<OnTickDelegate>();

        private readonly List<InternalTickerKey> m_keysToDecomission = new List<InternalTickerKey>();

        private event Action<int> listenerKeyAdded;
        private event Action<int> listenerKeyRemoved;

        public event Action<Ticker> onDecomissioned;

        public Ticker(Identity<Ticker> id, float ticksPerSecond)
        {
            if(id == null)
                throw new ArgumentNullException(nameof(id));

            if (ticksPerSecond <= 0)
                throw new ArgumentException($"{nameof(ticksPerSecond)} must be greater than zero.", nameof(ticksPerSecond));

            Id = id;
            TicksPerSecond = ticksPerSecond;
            m_secondsPerTick = 1.0f / ticksPerSecond;
        }

        public void AddTime(float deltaTime)
        {
            if (!Enabled)
                return;

            m_time += deltaTime;

            uint totalTicks = 0;
            while(m_time > m_secondsPerTick) {
                totalTicks++;
                m_time -= m_secondsPerTick;
            }

            if(totalTicks != 0)
                Tick(totalTicks);
        }

        private void Tick(uint deltaTicks = 1)
        {
            DecomissionKeys();

            ElapsedTicks += deltaTicks;

            for (int i = 0; i < m_delegates.Count; i++)
                m_delegates[i](deltaTicks);
        }

        public TickerKey Register(OnTickDelegate onTick)
        {
            if(onTick == null)
                throw new ArgumentNullException(nameof(onTick));

            InternalTickerKey key = new InternalTickerKey(this, m_tickerKeys.Count);

            m_tickerKeys.Add(key);
            m_delegates.Add(onTick);

            listenerKeyAdded?.Invoke(key.Key);

            listenerKeyAdded += key.OnKeyAdded;
            listenerKeyRemoved += key.OnKeyRemoved;
            key.onDecomission += OnTickerKeyDecomission;

            return key;
        }

        public void Unregister(TickerKey key)
        {
            if (key is InternalTickerKey internalKey)
                m_keysToDecomission.Add(internalKey);
        }

        private void DecomissionKeys()
        {
            for (int i = 0; i < m_keysToDecomission.Count; i++)
                m_keysToDecomission[i].Decomission();

            m_keysToDecomission.Clear();
        }

        private void OnTickerKeyDecomission(InternalTickerKey key)
        {
            m_tickerKeys.RemoveAt(key.Key);
            m_delegates.RemoveAt(key.Key);

            listenerKeyAdded -= key.OnKeyAdded;
            listenerKeyRemoved -= key.OnKeyRemoved;

            listenerKeyRemoved?.Invoke(key.Key);
        }

        public void Decomission()
        {
            m_keysToDecomission.Clear();

            m_tickerKeys.Clear();
            m_delegates.Clear();

            listenerKeyRemoved = null;
            listenerKeyAdded = null;

            Decomissioned = true;

            onDecomissioned?.Invoke(this);
            onDecomissioned = null;
        }
    }
}


