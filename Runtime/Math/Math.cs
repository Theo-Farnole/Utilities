﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Lortedo.Utilities
{
    public static class Math
    {
        /// <summary>
        /// If int is greater than max, so int=max
        /// Else, if int is minor than min, so int=max 
        /// </summary>
        public static int InverseClamp(int i, int min, int max)
        {
            int value = i;

            if (value > max)
            {
                value = min;
            }

            if (value < min)
            {
                value = max;
            }

            return value;
        }

        /// <summary>
        /// Return an Vector2 from an angle in radians.
        /// </summary>
        /// <param name="angle">In radians</param>
        /// <returns></returns>
        public static Vector2 AngleToVector2(float angle)
        {
            return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        }

        /// <summary>
        /// Returns normalized vector (from; to)
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static Vector3 Direction(Vector3 from, Vector3 to)
        {
            return VectorFromPoints(from, to).normalized;
        }
        
        /// <summary>
        /// Returns vector (from; to)
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static Vector3 VectorFromPoints(Vector3 from, Vector3 to)
        {
            return to - from;
        }

        public static Vector3 Clamp(Vector3 value, Vector3 min, Vector3 max)
        {
            Vector3 o = new Vector3(
                Mathf.Clamp(value.x, min.x, max.x),
                Mathf.Clamp(value.y, min.y, max.y),
                Mathf.Clamp(value.z, min.z, max.z)
            );

            return o;
        }

        public static Vector3 Clamp(Vector3 value, float min, float max)
        {
            Vector3 o = new Vector3(
                Mathf.Clamp(value.x, min, max),
                Mathf.Clamp(value.y, min, max),
                Mathf.Clamp(value.z, min, max)
            );

            return o;
        }

        public static float NearestOfZero(float v1, float v2)
        {
            return Mathf.Abs(v1) < Mathf.Abs(v2) ? v1 : v2;
        }

        public static bool HaveSameSign(float v1, float v2)
        {
            return Mathf.Sign(v1) == Mathf.Sign(v2);
        }

        public static Vector3 GetCentroid(Vector3[] points, float deviance)
        {
            // TODO: Remove points that doesn't respect deviance.
            throw new System.NotImplementedException();

            return GetCentroid(points);
        }

        /// <summary>
        /// Warning, this method use 'bounds.Encapsulate' for each point. It can be performance heavy.
        /// </summary>
        public static Vector3 GetCentroid(Vector3[] points)
        {
            Assert.IsTrue(points.Length > 0, "To GetCentroid, 'points' array must be have at least one element.");

            Bounds bounds = new Bounds(points[0], Vector3.zero);

            foreach (var point in points)
            {
                bounds.Encapsulate(point);
            }

            Vector3 output = bounds.center;

            return output;
        }

        public static bool Approximately(float a, float b, float delta) => Mathf.Abs(a - b) <= delta;
    }
}
