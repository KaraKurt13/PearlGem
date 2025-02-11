using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Objects
{
    public class SphereSector
    {
        public List<SphereElement> Elements = new();

        public void Destroy()
        {
            foreach (var element in Elements)
            {
                element.Rigidbody.useGravity = true;
            }
        }
    }
}
