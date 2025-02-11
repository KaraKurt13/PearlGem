using Assets.Scripts.Helpers;
using Assets.Scripts.Objects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        public float ProjectileForce { get; set; } = 20f;

        public bool IsReloading { get; private set; } = false;

        public Queue<ColorTypeEnum> ProjectilesColorQueue { get; private set; }

        private int _ticksTillReload, _maxTickTillReload;

        private PlayerProjectile _currentProjectile;

        [SerializeField]
        private InputController _inputController;

        [SerializeField]
        private Transform _projectileSpawnPoint;

        [SerializeField]
        private GameObject _projectilePrefab;


        private void Start()
        {
            _maxTickTillReload = TimeHelper.SecondsToTicks(2f);
            _ticksTillReload = _maxTickTillReload;
            InitializeProjectiles();
            ClearAimLine();
            SpawnProjectile();
        }

        private void Update()
        {
            if (_inputController.IsHolding() && CanShoot())
            {
                var touchPosition = _inputController.GetWorldTouchPosition();
                DrawTrajectory(_currentProjectile.transform.position, (touchPosition - _currentProjectile.transform.position).normalized);
            }

                
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
            var touchPosition = _inputController.GetWorldTouchPosition();
            var rigidbody = _currentProjectile.Rigidbody;
            var direction = (touchPosition - _currentProjectile.transform.position).normalized;
            rigidbody.useGravity = true;
            rigidbody.AddForce(direction * ProjectileForce, ForceMode.Impulse);
            IsReloading = true;
            RemainingProjectiles--;
            ClearAimLine();
        }

        private void SpawnProjectile()
        {
            var projectile = Instantiate(_projectilePrefab, _projectileSpawnPoint.transform.position, Quaternion.identity)
                .GetComponent<PlayerProjectile>();
            var nextColor = ProjectilesColorQueue.Dequeue();
            _currentProjectile = projectile;
            _currentProjectile.SetColor(nextColor);
        }

        private void InitializeProjectiles()
        {
            ProjectilesColorQueue = new();
            var allColors = (ColorTypeEnum[])Enum.GetValues(typeof(ColorTypeEnum));
            var rng = new System.Random();
            allColors = allColors.OrderBy(_ => rng.Next()).ToArray();

            foreach (var color in allColors)
            {
                ProjectilesColorQueue.Enqueue(color);
            }
        }


        #region AimLine
        [SerializeField]
        private LineRenderer _aimLine;

        private int _linePoints = 10;
        private float _simulationTimeStep = 0.1f;

        void DrawTrajectory(Vector3 startPoint, Vector3 direction)
        {
            Vector3 velocity = direction * ProjectileForce;
            _aimLine.positionCount = _linePoints;
            
            for (int i = 0; i < _linePoints; i++)
            {
                float simulationTime = i * _simulationTimeStep;
                Vector3 displacement = velocity * simulationTime + 0.5f * Physics.gravity * simulationTime * simulationTime;
                Vector3 drawPoint = startPoint + displacement;
                _aimLine.SetPosition(i, drawPoint);
            }
        }

        private void ClearAimLine()
        {
            _aimLine.positionCount = 0;
        }
        #endregion AimLine
    }
}