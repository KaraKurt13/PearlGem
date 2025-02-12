using Assets.Scripts.Helpers;
using Assets.Scripts.Objects;
using Assets.Scripts.UI;
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

        public PlayerController PlayerController;

        public PlayerStatsComponent PlayerStatsComponent;

        void Start()
        {
            Screen.orientation = ScreenOrientation.Portrait;
            LevelGenerator.Generate(4, 15, 6);
            Sphere.Activate(10f);
            PlayerController.Initialize();
            PlayerStatsComponent.Initialize();
        }
    }
}