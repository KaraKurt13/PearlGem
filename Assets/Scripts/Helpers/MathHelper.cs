using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Helpers
{
    public static class MathHelper
    {
        public static List<Vector3> GenerateIcospherePoints(int subdivisions, float radius)
        {
            List<Vector3> vertices = new List<Vector3>();
            List<int[]> triangles = new List<int[]>();

            float t = (1f + Mathf.Sqrt(5f)) / 2f;

            vertices.Add(new Vector3(-1f, t, 0f).normalized * radius);
            vertices.Add(new Vector3(1f, t, 0f).normalized * radius);
            vertices.Add(new Vector3(-1f, -t, 0f).normalized * radius);
            vertices.Add(new Vector3(1f, -t, 0f).normalized * radius);

            vertices.Add(new Vector3(0f, -1f, t).normalized * radius);
            vertices.Add(new Vector3(0f, 1f, t).normalized * radius);
            vertices.Add(new Vector3(0f, -1f, -t).normalized * radius);
            vertices.Add(new Vector3(0f, 1f, -t).normalized * radius);

            vertices.Add(new Vector3(t, 0f, -1f).normalized * radius);
            vertices.Add(new Vector3(t, 0f, 1f).normalized * radius);
            vertices.Add(new Vector3(-t, 0f, -1f).normalized * radius);
            vertices.Add(new Vector3(-t, 0f, 1f).normalized * radius);

            int[] faces = {
                0, 11, 5,  0, 5, 1,  0, 1, 7,  0, 7, 10,  0, 10, 11,
                1, 5, 9,  5, 11, 4,  11, 10, 2,  10, 7, 6,  7, 1, 8,
                3, 9, 4,  3, 4, 2,  3, 2, 6,  3, 6, 8,  3, 8, 9,
                4, 9, 5,  2, 4, 11,  6, 2, 10,  8, 6, 7,  9, 8, 1
            };

            for (int i = 0; i < faces.Length; i += 3)
            {
                triangles.Add(new int[] { faces[i], faces[i + 1], faces[i + 2] });
            }

            Dictionary<long, int> midPointCache = new Dictionary<long, int>();

            int GetMiddlePoint(int p1, int p2)
            {
                long smallerIndex = Mathf.Min(p1, p2);
                long greaterIndex = Mathf.Max(p1, p2);
                long key = (smallerIndex << 32) + greaterIndex;

                if (midPointCache.TryGetValue(key, out int index))
                    return index;

                Vector3 middle = ((vertices[p1] + vertices[p2]) * 0.5f).normalized * radius;
                vertices.Add(middle);
                int newIndex = vertices.Count - 1;
                midPointCache[key] = newIndex;
                return newIndex;
            }

            for (int i = 0; i < subdivisions; i++)
            {
                List<int[]> newTriangles = new List<int[]>();

                foreach (var tri in triangles)
                {
                    int a = tri[0];
                    int b = tri[1];
                    int c = tri[2];

                    int ab = GetMiddlePoint(a, b);
                    int bc = GetMiddlePoint(b, c);
                    int ca = GetMiddlePoint(c, a);

                    newTriangles.Add(new int[] { a, ab, ca });
                    newTriangles.Add(new int[] { b, bc, ab });
                    newTriangles.Add(new int[] { c, ca, bc });
                    newTriangles.Add(new int[] { ab, bc, ca });
                }

                triangles = newTriangles;
            }

            return vertices;
        }
    }
}
