using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace asim.unity.helpers
{
    public static class UnityOnGizmoHelper
    {
        public static void DrawTriangleLine(Vector3 pointA, Vector3 pointB, Vector3 pointC)
        {
            Gizmos.DrawLine(pointA, pointB);
            Gizmos.DrawLine(pointB, pointC);
            Gizmos.DrawLine(pointC, pointA);
        }

        public static void DrawTriangleMesh(Vector3 pointA, Vector3 pointB, Vector3 pointC)
        {
            //Front
            Mesh mesh = new Mesh();
            mesh.vertices = new Vector3[3] { pointA , pointB, pointC };
            mesh.triangles = new int[] { 0, 1, 2 };
            mesh.RecalculateNormals();

            //Back
            Mesh mesh2 = new Mesh();
            mesh2.vertices = new Vector3[3] { pointA, pointB, pointC };
            mesh2.triangles = new int[] { 2, 1, 0 };
            mesh2.RecalculateNormals();

            Gizmos.DrawMesh(mesh);
            Gizmos.DrawMesh(mesh2);
        }

        public static void DrawCircle(Vector3 center, float radius, int segmentCount, Quaternion rotation)
        {
            float step = Mathf.PI * 2 / segmentCount;

            Vector3 startPoint = center + (rotation * Vector3.right) * radius;

            for (int i = 1; i < segmentCount + 1; i++)
            {
                float angle = step * i;
                var x = Mathf.Cos(angle);
                var y = Mathf.Sin(angle);

                var endPoint = center + (rotation * new Vector3(x, y, 0)) * radius;
                Gizmos.DrawLine(startPoint, endPoint);

                startPoint = endPoint;
            }
        }
    }
}
