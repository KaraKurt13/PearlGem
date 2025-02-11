using Assets.Scripts.Helpers;
using Assets.Scripts.Objects;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts.Main
{
    public class PlayerController : MonoBehaviour
    {
        public float NormalizedReloadingTime
        {
            get
            {
                if (!IsReloading)
                    return 0;

                return _ticksTillReload / _maxTickTillReload;
            }
        }

        public int RemainingProjectiles { get; private set; } = 3;

        public bool IsReloading = false;

        private int _ticksTillReload, _maxTickTillReload;

        private PlayerProjectile _currentProjectile;

        [SerializeField]
        private InputController _inputController;


        private void Start()
        {
            _maxTickTillReload = TimeHelper.SecondsToTicks(2f);
            _ticksTillReload = _maxTickTillReload;
            SpawnProjectile();
        }

        private void Update()
        {
            if (_inputController.IsHolding())
                UpdateAimLine();
                
            if (_inputController.IsReleasing() && CanShoot())
                LaunchProjectile();
        }

        private void FixedUpdate()
        {
            if (IsReloading && RemainingProjectiles > 0)
            {
                _ticksTillReload--;
                if (_ticksTillReload <= 0)
                {
                    _ticksTillReload = _maxTickTillReload;
                    IsReloading = false;
                    SpawnProjectile();
                }
            }
        }

        public bool CanShoot()
        {
            return !IsReloading && RemainingProjectiles > 0;
        }

        private void LaunchProjectile()
        {
            Debug.Log("launch");
            var touchPosition = _inputController.GetWorldTouchPosition();
            var rigidbody = _currentProjectile.Rigidbody;
            var direction = CalculateDirection(_currentProjectile.transform.position, touchPosition);
            var force = 20f;
            rigidbody.velocity = direction * force;
            IsReloading = true;
            RemainingProjectiles--;
        }

        private Vector3 CalculateDirection(Vector3 startPosition, Vector3 targetPosition)
        {
            return (targetPosition - startPosition).normalized;
        }

        [SerializeField]
        private Transform _projectileSpawnPoint;

        [SerializeField]
        private GameObject _projectilePrefab;

        private void SpawnProjectile()
        {
            var projectile = Instantiate(_projectilePrefab, _projectileSpawnPoint.transform.position, Quaternion.identity)
                .GetComponent<PlayerProjectile>();
            _currentProjectile = projectile;
            _currentProjectile.SetColor();
        }

        private void UpdateAimLine()
        {

        }
    }
}