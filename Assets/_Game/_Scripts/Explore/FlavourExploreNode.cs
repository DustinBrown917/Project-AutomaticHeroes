using SegoLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class FlavourExploreNode : ExploreNode
    {
        [SerializeField] private string m_flavourText;
        [SerializeField] private FloatRange m_durationInSeconds = new FloatRange(1800, 3600);

        protected override void OnEnter(IExploreState state)
        {
            state.FlavourText = m_flavourText;
        }

        protected override void OnUpdate(IExploreState state, float deltaSeconds)
        {
            float v = state.WorldState.Rng.Range(m_durationInSeconds);

            if(state.ElapsedTimeInCurrentNode > v) {
                Debug.Log("Done!");
            }
        }
    } 
}
