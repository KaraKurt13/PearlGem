using Assets.Scripts.Helpers;
using Assets.Scripts.Objects;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Main
{
    public class LevelGenerator : MonoBehaviour
    {
        private List<Vector3> _hexCenters;

        private List<SphereElement> _sphereElements;

        [SerializeField]
        private GameObject _spherePrefab;

        public void Generate(int radius)
        {
            _hexCenters = MathHelper.GenerateIcospherePoints(5, radius);
            DrawSphereElements();
            CalculateNeighbours();
        }

        private void DrawSphereElements()
        {
            _sphereElements = new();
            foreach (var center in _hexCenters)
            {
                var element = Instantiate(_spherePrefab, center, Quaternion.identity, transform).GetComponent<SphereElement>();
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
                    .Where(other => other != element && Vector3.Distance(center, other.Center) <= 1.1f)
                    .ToList();
            }
        }
    }
}