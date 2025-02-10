using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Objects
{
    public class SphereElement : MonoBehaviour
    {
        public List<SphereElement> Neighbours;

        public SphereSector RelatedSector;

        public Vector3 Center;

        public Rigidbody Rigidbody;

        public MeshRenderer Renderer;

        public ColorTypeEnum ColorType;

        public void OnPlayerHit()
        {
            RelatedSector.Destroy();
        }

        private void OnTriggerEnter(Collider other)
        {
            OnPlayerHit();
        }
    }
}