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
        public LevelGenerator LevelGenerator;

        public Sphere Sphere;

        void Start()
        {
            LevelGenerator.Generate(4, 15, 6);
            Sphere.Activate(10f);
        }
    }
}