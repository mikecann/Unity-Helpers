using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UnityHelpers
{
    public class GizmoUtils
    {
        /// <summary>
        /// Thanks to: http://forum.unity3d.com/threads/debug-drawarrow.85980/
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="direction"></param>
        /// <param name="arrowHeadLength"></param>
        /// <param name="arrowHeadAngle"></param>
        public static void DrawArrow(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
        {
            Gizmos.DrawRay(pos, direction);
            DrawArrowEnd(true, pos, direction, Gizmos.color, arrowHeadLength, arrowHeadAngle);
        }

        private static void DrawArrowEnd(bool gizmos, Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
        {
            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(arrowHeadAngle, 0, 0) * Vector3.back;
            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(-arrowHeadAngle, 0, 0) * Vector3.back;
            Vector3 up = Quaternion.LookRotation(direction) * Quaternion.Euler(0, arrowHeadAngle, 0) * Vector3.back;
            Vector3 down = Quaternion.LookRotation(direction) * Quaternion.Euler(0, -arrowHeadAngle, 0) * Vector3.back;
            Gizmos.color = color;
            Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
            Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
            Gizmos.DrawRay(pos + direction, up * arrowHeadLength);
            Gizmos.DrawRay(pos + direction, down * arrowHeadLength);
        }

    }
}
