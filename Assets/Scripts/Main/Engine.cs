using Assets.Scripts.Helpers;
using Assets.Scripts.Objects;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Main
{
    public class Engine : MonoBehaviour
    {
        public float sphereRadius = 5f;

        public LevelGenerator LevelGenerator;

        public GameObject hexPrefab;
        public List<SphereElement> sphereElements = new List<SphereElement>();


        private List<Vector3> hexCenters = new List<Vector3>();
        private Dictionary<Vector3, List<Vector3>> neighbors = new Dictionary<Vector3, List<Vector3>>();

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider != null)
                    {
                        hit.collider.GetComponent<SphereElement>().Deactivate();
                    }
                }
            }
        }

        void Start()
        {
            GenerateHexGrid();
            DrawHexCenters();
        }

        void GenerateHexGrid()
        {
            hexCenters = MathHelper.GenerateIcospherePoints(5, sphereRadius);
        }

        void DrawHexCenters()
        {
            foreach (var center in hexCenters)
            {
                var element = Instantiate(hexPrefab, center, Quaternion.identity, transform).GetComponent<SphereElement>();
                element.Center = center;
                sphereElements.Add(element);
            }
            CalculateNeighbors();
        }

        private void CalculateNeighbors()
        {
            foreach (var element in sphereElements)
            {
                var center = element.Center;
                element.Neighbours = sphereElements
                    .Where(other => other != element && Vector3.Distance(center, other.Center) <= 1.1f)
                    .ToList();
            }
        }
    }
}