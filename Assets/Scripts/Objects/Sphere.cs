using Assets.Scripts.Helpers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Objects
{
    public class Sphere : MonoBehaviour
    {
        public Transform Transform;

        public float RotationSpeed { get; private set; }

        private float _rotationPerTick;

        private void FixedUpdate()
        {
            Transform.Rotate(Vector3.right, _rotationPerTick);
        }

        public void Activate(float rotationSpeed)
        {
            _rotationPerTick = rotationSpeed / TimeHelper.TicksPerSecond;
        }
    }
}