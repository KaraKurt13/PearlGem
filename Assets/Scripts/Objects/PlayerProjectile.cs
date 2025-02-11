using Assets.Scripts.Helpers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Objects
{
    public class PlayerProjectile : MonoBehaviour
    {
        public Rigidbody Rigidbody;

        public SphereCollider Collider;

        public MeshRenderer Renderer;

        public ColorTypeEnum Color;

        public void SetColor()
        {
            var randomIndex = Random.Range(0, Constants.Colors.Count);
            var colorKvp = Constants.Colors.ElementAt(randomIndex);
            Color = colorKvp.Key;
            Renderer.material.color = colorKvp.Value;
        }

        private void OnSuccessfulHit()
        {
            Destroy(gameObject);
        }

        private void OnWrongHit()
        {
            Destroy(gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<SphereElement>(out var sphere))
            {
                if (sphere.ColorType == Color)
                {
                    sphere.OnPlayerHit();
                    OnSuccessfulHit();
                }
                else
                    OnWrongHit();
            }
        }
    }
}