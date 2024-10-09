using System.Collections.Generic;
using UnityEngine;

namespace SegoLib.Tickers
{
    public class TickerManager : MonoBehaviour
    {
        private enum ETickerAction : byte
        {
            Add, Remove
        }


        /// <summary>
        /// The registered tickers.
        /// </summary>
        private readonly Dictionary<Identity, Ticker> m_tickers = new Dictionary<Identity, Ticker>();

        private readonly Queue<ETickerAction> m_queuedActions = new Queue<ETickerAction>();
        private readonly Queue<Ticker> m_actionableTickers = new Queue<Ticker>();


        private void Update()
        {
            ResolveQueuedTickerActions();

            foreach(Ticker ticker in m_tickers.Values) {
                if (ticker.Enabled)
                    ticker.AddTime(Time.deltaTime);
            }
        }

        private void ResolveQueuedTickerActions()
        {
            if (m_queuedActions.Count == 0)
                return;

            while(m_queuedActions.Count > 0) {
                ETickerAction action = m_queuedActions.Dequeue();
                Ticker ticker = m_actionableTickers.Dequeue();

                if(action == ETickerAction.Add) {
                    if (ticker.Decomissioned)
                        continue;

                    m_tickers[ticker.Id] = ticker;
                    ticker.onDecomissioned += OnTickerDecomissioned;
                }
                else if(action == ETickerAction.Remove) {
                    m_tickers.Remove(ticker.Id);
                    ticker.onDecomissioned -= OnTickerDecomissioned;
                }
            }
        }

        /// <summary>
        /// Register a ticker.
        /// </summary>
        /// <param name="ticker"></param>
        public void RegisterTicker(Ticker ticker)
        {
            m_queuedActions.Enqueue(ETickerAction.Add);
            m_actionableTickers.Enqueue(ticker);
        }

        /// <summary>
        /// Unregister a ticker.
        /// </summary>
        /// <param name="ticker"></param>
        /// <returns>True if the ticker was removed, false if the ticker was not found.</returns>
        public void UnregisterTicker(Ticker ticker)
        {
            m_queuedActions.Enqueue(ETickerAction.Remove);
            m_actionableTickers.Enqueue(ticker);
        }

        /// <summary>
        /// Unregister a ticker.
        /// </summary>
        /// <param name="tickerName"></param>
        /// <returns>True if the ticker was removed, false if no ticker was found by that name.</returns>
        public void UnregisterTicker(Identity tickerIdentity)
        {
            if(m_tickers.TryGetValue(tickerIdentity, out Ticker ticker))
                UnregisterTicker(ticker);
        }

        /// <summary>
        /// Try to get a ticker.
        /// </summary>
        /// <param name="tickerName"></param>
        /// <param name="ticker"></param>
        /// <returns>False if a ticker was not found by that name.</returns>
        public bool TryGetTicker(Identity tickerIdentity, out Ticker ticker)
        {
            return m_tickers.TryGetValue(tickerIdentity, out ticker);
        }

        /// <summary>
        /// Get a registered ticker.
        /// </summary>
        /// <param name="tickerName"></param>
        /// <returns></returns>
        public Ticker GetTicker(Identity tickerIdentity)
        {
            return m_tickers[tickerIdentity];
        }

        /// <summary>
        /// Raised when a ticker becomes decomissioned.
        /// </summary>
        /// <param name="ticker"></param>
        private void OnTickerDecomissioned(Ticker ticker)
        {
            UnregisterTicker(ticker);
        }
    } 
}
