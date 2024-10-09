using System.Collections.Generic;

namespace Game.YAGNI
{
    public class Journey
    {
        private List<IRouteTraversal> m_traversals;
        private Hero m_hero;
        private float m_totalJourneyDistance;
        private float m_distanceTravelled;
        private float m_progress;

        private int m_currentTraversalIndex = 0;
        private IRouteTraversal m_currentRouteTraversal;
        private float m_distanceTravelledOnThisRoute;

        public Journey(Hero hero, List<IRouteTraversal> traversals)
        {
            m_traversals = traversals;
            m_totalJourneyDistance = GetJourneyDistance();
            UpdateCurrentTraversal(traversals[0]);
        }

        public void Progress(float deltaTime)
        {
            float distanceTravelledThisStep = deltaTime * m_hero.Movement.MoveSpeed;

            m_distanceTravelledOnThisRoute += distanceTravelledThisStep;

            float t = m_distanceTravelledOnThisRoute / m_currentRouteTraversal.Length;

            bool segmentEndReached = false;

            m_hero.transform.position = m_currentRouteTraversal.Traverse(t);

            // Reached end of current segment.
            if (t >= 1) {
                distanceTravelledThisStep -= (m_distanceTravelledOnThisRoute - m_currentRouteTraversal.Length);
                segmentEndReached = true;
            }

            m_distanceTravelled += distanceTravelledThisStep;

            if (segmentEndReached) {
                m_currentTraversalIndex++;
                if (m_traversals.Count > m_currentTraversalIndex) {
                    UpdateCurrentTraversal(m_traversals[m_currentTraversalIndex]);
                }

                // Enter new location!

                if (m_currentTraversalIndex >= m_traversals.Count) {
                    // Journey done!
                }
            }
        }

        private void UpdateCurrentTraversal(IRouteTraversal newTraversal)
        {
            m_currentRouteTraversal = newTraversal;
            m_distanceTravelledOnThisRoute = 0;
        }

        private float GetJourneyDistance()
        {
            float sum = 0;
            for (int i = 0; i < m_traversals.Count; i++) {
                sum += m_traversals[i].Length;
            }

            return sum;
        }
    }

}