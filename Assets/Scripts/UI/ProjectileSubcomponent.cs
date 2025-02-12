using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class ProjectileSubcomponent : ComponentBase
    {
        [SerializeField] private Image _projectileImage;

        public void SetColor(Color color)
        {
            _projectileImage.color = color;
        }

        private void OnBecameInvisible()
        {
            Destroy(gameObject);
        }
    }
}