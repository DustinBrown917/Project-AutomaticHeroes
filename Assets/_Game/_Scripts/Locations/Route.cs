using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Route : MonoBehaviour, IRoute
    {
        public ILocation A => m_locationA;

        public ILocation B => m_locationB;

        [SerializeField] private Location m_locationA;
        [SerializeField] private Location m_locationB;
        [SerializeField] private Transform[] m_routePoints;


        public float Length { get; private set; }

        public IReadOnlyList<Vector2> Points => m_points;
        private Vector2[] m_points;

        private float[] m_tAtPoints;

        private void Awake()
        {
            ProcessPoints();

            float[] segmentLengths = GetSegmentLengths(m_points);
            Length = GetSumOfSegmentLengths(segmentLengths);
            m_tAtPoints = GetTAtPoints(segmentLengths, Length);
            m_routePoints = null;

            m_locationA.AddRoute(this);
            m_locationB.AddRoute(this);
        }

        private void OnValidate()
        {
            if(m_locationA != null && m_locationB != null) {
                name = $"{m_locationA.name} to {m_locationB.name}";
            }
        }

        private void OnDrawGizmos()
        {           
            if(m_locationA == null || m_locationB == null) return;

            Gizmos.color = Color.yellow;

            Vector3[] points = new Vector3[m_routePoints.Length + 2];

            points[0] = m_locationA.Position;
            points[^1] = m_locationB.Position;

            for(int i = 0; i < m_routePoints.Length; i++) {
                points[i + 1] = m_routePoints[i].transform.position;
            }

            System.ReadOnlySpan<Vector3> span = new System.ReadOnlySpan<Vector3>(points);
            Gizmos.DrawLineStrip(span, false);
        }

        private void ProcessPoints()
        {
            m_points = new Vector2[m_routePoints.Length + 2];
            m_points[0] = m_locationA.Position;
            m_points[^1] = m_locationB.Position;

            for(int i = 0; i < m_routePoints.Length; i++) {
                m_points[i + 1] = m_routePoints[i].position; 
            }
        }

        private float[] GetSegmentLengths(IReadOnlyList<Vector2> points)
        {
            float[] segmentLengths = new float[points.Count - 1];

            for (int i = 1; i < points.Count; i++) {
                segmentLengths[i - 1] = Vector2.Distance(points[i - 1], points[i]);
            }

            return segmentLengths;
        }

        private float GetSumOfSegmentLengths(float[] segmentLengths)
        {
            float sum = 0;
            for (int i = 0; i < segmentLengths.Length; i++) {
                sum += segmentLengths[i];
            }
            return sum;
        }

        private float[] GetTAtPoints(float[] segmentLengths, float sumOfLengths)
        {
            float[] t = new float[segmentLengths.Length + 1];

            t[0] = 0;
            t[^1] = 1;

            float runningTotal = 0;
            for (int i = 1; i < segmentLengths.Length; i++) {
                runningTotal += segmentLengths[i - 1];
                t[i] = runningTotal / sumOfLengths;
            }

            return t;
        }

        public Vector2 TraverseAToB(float t)
        {
            if (t <= 0)
                return Points[0];

            if (t >= 1)
                return Points[^1];

            for (int i = 1; i < m_tAtPoints.Length; i++) {
                if (t <= m_tAtPoints[i]) {
                    float subT = Mathf.InverseLerp(m_tAtPoints[i - 1], m_tAtPoints[i], t);
                    return Vector2.Lerp(Points[i - 1], Points[i], subT);
                }
            }

            return Points[^1];
        }

        public Vector2 TraverseBToA(float t)
        {
            return TraverseAToB(1.0f - t);
        }
    } 
}
