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

        public void SetColor(ColorTypeEnum colorType)
        {
            Color = colorType;
            var color = Constants.Colors[colorType];
            Renderer.material.color = color;
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