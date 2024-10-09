using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

namespace SegoLib
{
    public static class LineUtilities
    {
        /// <summary>
        /// Check if two line segments intersect.<br/>
        /// https://forum.unity.com/threads/line-intersection.17384/#:~:text=You%20basically%20just%20check%20each,will%20know%20if%20they%20intersect. - see BAST42's post.
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="p4"></param>
        /// <returns></returns>
        public static bool FasterLineSegmentIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
        {

            Vector2 a = p2 - p1;
            Vector2 b = p3 - p4;
            Vector2 c = p1 - p3;

            float alphaNumerator = b.y * c.x - b.x * c.y;
            float alphaDenominator = a.y * b.x - a.x * b.y;
            float betaNumerator = a.x * c.y - a.y * c.x;
            float betaDenominator = a.y * b.x - a.x * b.y;

            bool doIntersect = true;

            if (alphaDenominator == 0 || betaDenominator == 0) {
                doIntersect = false;
            } else {

                if (alphaDenominator > 0) {
                    if (alphaNumerator < 0 || alphaNumerator > alphaDenominator) {
                        doIntersect = false;

                    }
                } else if (alphaNumerator > 0 || alphaNumerator < alphaDenominator) {
                    doIntersect = false;
                }

                if (doIntersect && betaDenominator > 0) {
                    if (betaNumerator < 0 || betaNumerator > betaDenominator) {
                        doIntersect = false;
                    }
                } else if (betaNumerator > 0 || betaNumerator < betaDenominator) {
                    doIntersect = false;
                }
            }

            return doIntersect;
        }

        /// <summary>
        /// Find the distance from a point to a line segment.
        /// <br/>http://www.csharphelper.com/howtos/howto_point_segment_distance.html
        /// </summary>
        /// <param name="pt">The point.</param>
        /// <param name="p1">The start of the line segment.</param>
        /// <param name="p2">The end of the line segment.</param>
        /// <param name="closest"></param>
        /// <returns></returns>
        public static float FindDistanceToSegment(Vector2 pt, Vector2 p1, Vector2 p2, out Vector2 closest)
        {
            float dx = p2.x - p1.x;
            float dy = p2.y - p1.y;
            if ((dx == 0) && (dy == 0)) {
                // It's a point not a line segment.
                closest = p1;
                dx = pt.x - p1.x;
                dy = pt.y - p1.y;
                return Mathf.Sqrt(dx * dx + dy * dy);
            }

            // Calculate the t that minimizes the distance.
            float t = ((pt.x - p1.x) * dx + (pt.y - p1.y) * dy) /
                (dx * dx + dy * dy);

            // See if this represents one of the segment's
            // end points or a point in the middle.
            if (t < 0) {
                closest = new Vector2(p1.x, p1.y);
                dx = pt.x - p1.x;
                dy = pt.y - p1.y;
            } else if (t > 1) {
                closest = new Vector2(p2.x, p2.y);
                dx = pt.x - p2.x;
                dy = pt.y - p2.y;
            } else {
                closest = new Vector2(p1.x + t * dx, p1.y + t * dy);
                dx = pt.x - closest.x;
                dy = pt.y - closest.y;
            }

            return Mathf.Sqrt(dx * dx + dy * dy);
        }

        public static Vector3[] SmoothLine(Vector3[] inputPoints, float segmentSize)
        {
            //create curves
            AnimationCurve curveX = new AnimationCurve();
            AnimationCurve curveY = new AnimationCurve();
            AnimationCurve curveZ = new AnimationCurve();

            //create keyframe sets
            Keyframe[] keysX = new Keyframe[inputPoints.Length];
            Keyframe[] keysY = new Keyframe[inputPoints.Length];
            Keyframe[] keysZ = new Keyframe[inputPoints.Length];

            //set keyframes
            for (int i = 0; i < inputPoints.Length; i++) {
                keysX[i] = new Keyframe(i, inputPoints[i].x);
                keysY[i] = new Keyframe(i, inputPoints[i].y);
                keysZ[i] = new Keyframe(i, inputPoints[i].z);
            }

            //apply keyframes to curves
            curveX.keys = keysX;
            curveY.keys = keysY;
            curveZ.keys = keysZ;

            //smooth curve tangents
            for (int i = 0; i < inputPoints.Length; i++) {
                curveX.SmoothTangents(i, 0);
                curveY.SmoothTangents(i, 0);
                curveZ.SmoothTangents(i, 0);
            }

            //list to write smoothed values to
            List<Vector3> lineSegments = new List<Vector3>();

            //find segments in each section
            for (int i = 0; i < inputPoints.Length; i++) {
                //add first point
                lineSegments.Add(inputPoints[i]);

                //make sure within range of array
                if (i + 1 < inputPoints.Length) {
                    //find distance to next point
                    float distanceToNext = Vector3.Distance(inputPoints[i], inputPoints[i + 1]);

                    //number of segments
                    int segments = (int)(distanceToNext / segmentSize);

                    //add segments
                    for (int s = 1; s < segments; s++) {
                        //interpolated time on curve
                        float time = ((float)s / (float)segments) + (float)i;

                        //sample curves to find smoothed position
                        Vector3 newSegment = new Vector3(curveX.Evaluate(time), curveY.Evaluate(time), curveZ.Evaluate(time));

                        //add to list
                        lineSegments.Add(newSegment);
                    }
                }
            }

            return lineSegments.ToArray();
        }

        public static Vector2[] SmoothLine(IList<Vector2> inputPoints, float segmentSize)
        {
            //create curves
            AnimationCurve curveX = new AnimationCurve();
            AnimationCurve curveY = new AnimationCurve();

            //create keyframe sets
            Keyframe[] keysX = new Keyframe[inputPoints.Count];
            Keyframe[] keysY = new Keyframe[inputPoints.Count];

            //set keyframes
            for (int i = 0; i < inputPoints.Count; i++) {
                keysX[i] = new Keyframe(i, inputPoints[i].x);
                keysY[i] = new Keyframe(i, inputPoints[i].y);
            }

            //apply keyframes to curves
            curveX.keys = keysX;
            curveY.keys = keysY;

            //smooth curve tangents
            for (int i = 0; i < inputPoints.Count; i++) {
                curveX.SmoothTangents(i, 0);
                curveY.SmoothTangents(i, 0);
            }

            //list to write smoothed values to
            List<Vector2> lineSegments = new List<Vector2>();

            //find segments in each section
            for (int i = 0; i < inputPoints.Count; i++) {
                //add first point
                lineSegments.Add(inputPoints[i]);

                //make sure within range of array
                if (i + 1 < inputPoints.Count) {
                    //find distance to next point
                    float distanceToNext = Vector2.Distance(inputPoints[i], inputPoints[i + 1]);

                    //number of segments
                    int segments = (int)(distanceToNext / segmentSize);

                    //add segments
                    for (int s = 1; s < segments; s++) {
                        //interpolated time on curve
                        float time = ((float)s / segments) + i;

                        //sample curves to find smoothed position
                        Vector2 newSegment = new Vector2(curveX.Evaluate(time), curveY.Evaluate(time));

                        //add to list
                        lineSegments.Add(newSegment);
                    }
                }
            }

            return lineSegments.ToArray();
        }
    } 
}
