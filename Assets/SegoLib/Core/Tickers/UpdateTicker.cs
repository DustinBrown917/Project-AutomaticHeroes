using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SegoLib.Tickers
{
    //public class UpdateTicker : Ticker
    //{
    //    /// <summary>
    //    /// How many ticks elapse per second.
    //    /// </summary>
    //    [field: SerializeField] public uint TickRate { get; private set; } = 60;

    //    /// <summary>
    //    /// The fractional remainder of a tick.
    //    /// </summary>
    //    private float m_tickExcess = 0;

    //    private void Update()
    //    {
    //        m_tickExcess += Time.deltaTime * TickRate;
    //        uint deltaTicks = (uint)m_tickExcess;
    //        if (deltaTicks > 0) {
    //            m_tickExcess -= deltaTicks;
    //            Tick(deltaTicks);
    //        }
    //    }
    //} 
}
