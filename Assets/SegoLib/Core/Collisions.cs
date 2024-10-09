using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SegoLib
{
    public static class Collisions 
    {
        /// <summary>
        /// True if the two defined circles overlap.
        /// </summary>
        /// <param name="circle1Pos"></param>
        /// <param name="circle1Radius"></param>
        /// <param name="circle2Pos"></param>
        /// <param name="circle2Radius"></param>
        /// <returns></returns>
        public static bool CircleCircle(Vector2 circle1Pos, float circle1Radius, Vector2 circle2Pos, float circle2Radius)
        {
            float squareDistance = (circle1Pos - circle2Pos).sqrMagnitude;
            float combinedRadius = circle1Radius + circle2Radius;
            float squareRadius = combinedRadius * combinedRadius;

            return squareDistance < squareRadius;
        }

        /// <summary>
        /// True if the defined circle overlaps with any part of the line segment defined by (<paramref name="lineP1"/>, <paramref name="lineP2"/>).
        /// </summary>
        /// <param name="circlePos"></param>
        /// <param name="circleRadius"></param>
        /// <param name="lineP1"></param>
        /// <param name="lineP2"></param>
        /// <returns></returns>
        public static bool CircleLineSegment(Vector2 circlePos, float circleRadius, Vector2 lineP1, Vector2 lineP2)
        {
            float squareRadius = circleRadius * circleRadius;

            return SqrDistanceToLineSegment(lineP1, lineP2, circlePos, out _) <= squareRadius;
        }

        public static bool CircleRect(Vector2 circlePos, float circleRadius, Rect rect)
        {
            if (rect.Contains(circlePos)) {
                return true;
            }

            // Initialize with left edge values
            Vector2 p1 = rect.min;
            Vector2 p2 = new Vector2(rect.xMin, rect.yMax);

            if (circlePos.x >= rect.xMax) { // Circle is right.
                p1 = new Vector2(rect.xMax, rect.yMin);
                p2 = rect.max;
            } else if (circlePos.y >= rect.yMax) { // Circle is above.
                p1 = rect.max;
            } else if(circlePos.y <= rect.yMin) { // Circle is below.
                p2 = new Vector2(rect.xMax, rect.yMin);
            }


            float squareRadius = circleRadius * circleRadius;

            return SqrDistanceToLineSegment(p2, p1, circlePos, out _) <= squareRadius;
        }

        /// <summary>
        /// Get the shortest square distance from a point to a line segment. <br/>
        /// Derived from Grumdig's answer: https://stackoverflow.com/questions/849211/shortest-distance-between-a-point-and-a-line-segment
        /// </summary>
        /// <param name="lineP1"></param>
        /// <param name="lineP2"></param>
        /// <param name="p"></param>
        /// <param name="projection"></param>
        /// <returns></returns>
        public static float SqrDistanceToLineSegment(Vector2 lineP1, Vector2 lineP2, Vector2 p, out Vector2 projection)
        {
            // Return minimum distance between line segment vw and point p
            float l2 = (lineP1 - lineP2).sqrMagnitude;  // i.e. |w-v|^2 -  avoid a sqrt
            if (l2 == 0.0) {
                projection = lineP1;
                return (p - lineP1).sqrMagnitude;   // v == w case
            }

            // Consider the line extending the segment, parameterized as v + t (w - v).
            // We find projection of point p onto the line. 
            // It falls where t = [(p-v) . (w-v)] / |w-v|^2
            // We clamp t from [0,1] to handle points outside the segment vw.
            float t = Mathf.Max(0, Mathf.Min(1, Vector2.Dot(p - lineP1, lineP2 - lineP1) / l2));
            projection = lineP1 + t * (lineP2 - lineP1);  // Projection falls on the segment
            return (p - projection).sqrMagnitude;
        }
    } 
}
