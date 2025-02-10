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
            LevelGenerator.Generate(4, 15);
        }
    }
}