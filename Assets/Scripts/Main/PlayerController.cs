using Assets.Scripts.Helpers;
using Assets.Scripts.Objects;
using Assets.Scripts.UI;
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

                return (float)_ticksTillReload / _maxTickTillReload;
            }
        }

        public int RemainingProjectiles { get; private set; } = 6;

        public float ProjectileForce { get; set; } = 20f;

        public float MaxProjectileForce { get; } = 60f;

        public float MinProjectileForce { get; } = 5f;

        public bool IsReloading { get; private set; } = false;

        public Queue<ColorTypeEnum> ProjectilesColorQueue { get; private set; }

        public PlayerProjectile CurrentProjectile;

        private int _ticksTillReload, _maxTickTillReload;

        [SerializeField] private InputController _inputController;
        [SerializeField] private Transform _projectileSpawnPoint;
        [SerializeField] private GameObject _projectilePrefab;
        [SerializeField] private PlayerStatsComponent _playerStatsComponent;

        private bool _isPreparingToShoot = false;

        private void Update()
        {
            if (_inputController.IsHolding() && CanShoot())
            {
                var touchPosition = _inputController.GetWorldTouchPosition();
                DrawTrajectory(CurrentProjectile.transform.position, (touchPosition - CurrentProjectile.transform.position).normalized);
                _isPreparingToShoot = true;
            }

                
            if (_isPreparingToShoot && _inputController.IsReleasing() && CanShoot())
            {
                ClearAimLine();
                LaunchProjectile();
                _playerStatsComponent.UpdateQueue();
            }
        }

        private void FixedUpdate()
        {
            if (IsReloading)
            {
                _ticksTillReload--;
                if (_ticksTillReload <= 0)
                {
                    _ticksTillReload = _maxTickTillReload;
                    IsReloading = false;
                    if (RemainingProjectiles > 0)
                    {
                        SpawnProjectile();
                        _playerStatsComponent.UpdateQueue();
                    }
                }
            }
        }

        public void Initialize()
        {
            _maxTickTillReload = TimeHelper.SecondsToTicks(2f);
            _ticksTillReload = _maxTickTillReload;
            InitializeProjectiles();
            ClearAimLine();
            SpawnProjectile();
        }

        public void SetForcePower(float power)
        {
            ProjectileForce = power;
        }

        public bool CanShoot()
        {
            return !IsReloading && RemainingProjectiles > 0;
        }

        private void LaunchProjectile()
        {
            var touchPosition = _inputController.GetWorldTouchPosition();
            var rigidbody = CurrentProjectile.Rigidbody;
            var direction = (touchPosition - CurrentProjectile.transform.position).normalized;
            rigidbody.useGravity = true;
            rigidbody.AddForce(direction * ProjectileForce, ForceMode.Impulse);
            IsReloading = true;
            RemainingProjectiles--;
            CurrentProjectile = null;
            _isPreparingToShoot = false;
        }

        private void SpawnProjectile()
        {
            var projectile = Instantiate(_projectilePrefab, _projectileSpawnPoint.transform.position, Quaternion.identity)
                .GetComponent<PlayerProjectile>();
            var nextColor = ProjectilesColorQueue.Dequeue();
            CurrentProjectile = projectile;
            CurrentProjectile.SetColor(nextColor);
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