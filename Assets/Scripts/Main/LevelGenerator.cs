using Assets.Scripts.Helpers;
using Assets.Scripts.Objects;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Search;
using UnityEngine;

namespace Assets.Scripts.Main
{
    public class LevelGenerator : MonoBehaviour
    {
        [SerializeField]
        private GameObject _spherePrefab;

        [SerializeField]
        private Transform _sphereContainer;

        private List<Vector3> _hexCenters;

        private List<SphereElement> _sphereElements;

        private float _distanceBetweenSpheres;

        public void Generate(int radius, int sectorSize, int colorsCount)
        {
            _hexCenters = GenerateIcospherePoints(3, radius);
            DrawSphereElements();
            CalculateNeighbours();
            ColorSphere(sectorSize, colorsCount);
            DivideSphereToSectors();
        }

        private void DrawSphereElements()
        {
            _sphereElements = new();
            foreach (var center in _hexCenters)
            {
                var element = Instantiate(_spherePrefab, center, Quaternion.identity, _sphereContainer).GetComponent<SphereElement>();
                element.Center = center;
                _sphereElements.Add(element);
            }
        }

        private void CalculateNeighbours()
        {
            foreach (var element in _sphereElements)
            {
                var center = element.Center;
                element.Neighbours = _sphereElements
                    .Where(other => other != element && Vector3.Distance(center, other.Center) <= _distanceBetweenSpheres)
                    .ToList();
            }
        }

        private Dictionary<ColorTypeEnum, Color> _colors;

        private void ColorSphere(int sectorSize, int colorsCount)
        {
            _colors = Constants.Colors;
            var visitedElements = new HashSet<SphereElement>();
            var unpaintedElements = new List<SphereElement>(_sphereElements);
            var maxColorIndex = _colors.Count;
            var colors = _colors.Keys.Take(colorsCount).ToArray();

            if (sectorSize < 1) sectorSize = 1;

            int index = 0;
            while (unpaintedElements.Count > 0)
            {
                var startElement = unpaintedElements[Random.Range(0, unpaintedElements.Count)];
                var sectorColor = colors[index % colorsCount];
                index++;
                PaintSector(startElement, sectorColor, visitedElements, unpaintedElements, sectorSize);
            }
        }

        private void PaintSector(SphereElement start, ColorTypeEnum colorEnum, HashSet<SphereElement> visited, List<SphereElement> unpaintedElements, int sectorSize)
        {
            var queue = new Queue<SphereElement>();
            var color = _colors[colorEnum];
            queue.Enqueue(start);
            visited.Add(start);
            unpaintedElements.Remove(start);
            //start.Renderer.material.color = color;
            start.ColorType = colorEnum;

            int paintedCount = 1;
            while (queue.Count > 0 && paintedCount < sectorSize)
            {
                var current = queue.Dequeue();

                foreach (var neighbour in current.Neighbours)
                {
                    if (visited.Add(neighbour))
                    {
                        //neighbour.Renderer.material.color = color;
                        neighbour.ColorType = colorEnum;
                        queue.Enqueue(neighbour);
                        unpaintedElements.Remove(neighbour);
                        paintedCount++;

                        if (paintedCount >= sectorSize) break;
                    }
                }

                if (paintedCount >= sectorSize) break;
            }
        }

        private void DivideSphereToSectors()
        {
            var uncheckedElements = new HashSet<SphereElement>(_sphereElements);

            while (uncheckedElements.Count > 0)
            {
                var startElement = uncheckedElements.First();
                var elementColor = startElement.ColorType;
                var sector = new SphereSector();
                var queue = new Queue<SphereElement>();
                queue.Enqueue(startElement);
                uncheckedElements.Remove(startElement);

                while (queue.Count > 0)
                {
                    var currentElement = queue.Dequeue();
                    sector.Elements.Add(currentElement);
                    currentElement.RelatedSector = sector;

                    foreach (var neighbor in currentElement.Neighbours)
                    {
                        if (uncheckedElements.Contains(neighbor) && neighbor.ColorType == startElement.ColorType)
                        {
                            queue.Enqueue(neighbor);
                            uncheckedElements.Remove(neighbor);
                        }
                    }
                }
            }
        }

        #region Math
        private List<Vector3> GenerateIcospherePoints(int subdivisions, float radius)
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

            _distanceBetweenSpheres = GetEdgeLength(vertices, triangles);

            return vertices;
        }

        private float GetEdgeLength(List<Vector3> vertices, List<int[]> triangles)
        {
            var firstEdge = triangles[0];
            var rawDistance = Vector3.Distance(vertices[firstEdge[0]], vertices[firstEdge[1]]);
            var distanceOffset = rawDistance / 2;
            return rawDistance + distanceOffset;

        }
        #endregion Math
    }
}