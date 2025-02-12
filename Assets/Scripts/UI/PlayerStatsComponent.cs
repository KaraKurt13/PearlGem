using Assets.Scripts.Helpers;
using Assets.Scripts.Main;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class PlayerStatsComponent : ComponentBase
    {
        [SerializeField] private Slider _reloadProgress;
        [SerializeField] private ProjectileSubcomponent _currentProjectile;
        [SerializeField] private GameObject _projectilePrefab;
        [SerializeField] private Transform _projectilesQueueContainer;
        [SerializeField] private PlayerController _playerController;

        private Queue<ProjectileSubcomponent> _projectilesQueue;

        private bool _isInitialized = false;

        private void Update()
        {
            if (_playerController.IsReloading)
            {
                _reloadProgress.gameObject.SetActive(true);
                UpdateReloadProgress();
            }
            else
                _reloadProgress.gameObject.SetActive(false);
        }

        public void Initialize()
        {
            _projectilesQueue = new();
            foreach (var projectile in _playerController.ProjectilesColorQueue)
            {
                var projectileObject = Instantiate(_projectilePrefab, _projectilesQueueContainer).GetComponent<ProjectileSubcomponent>();
                var color = Constants.Colors[projectile];
                projectileObject.SetColor(color);
                _projectilesQueue.Enqueue(projectileObject);
            }

            var currentColor = Constants.Colors[_playerController.CurrentProjectile.Color];
            _currentProjectile.Draw();
            _currentProjectile.SetColor(currentColor);

            _isInitialized = true;
        }

        private int _lastCheckQueueCount;

        public void UpdateQueue()
        {
            if (!_isInitialized) 
                return;

            var currentProjectile = _playerController.CurrentProjectile;
            if (currentProjectile != null)
            {
                var color = Constants.Colors[currentProjectile.Color];
                _currentProjectile.Draw();
                _currentProjectile.SetColor(color);
            }
            else
                _currentProjectile.Hide();

            _lastCheckQueueCount = _playerController.ProjectilesColorQueue.Count;

            if (_projectilesQueue.Count != 0 && _projectilesQueue.Count != _lastCheckQueueCount)
                _projectilesQueue.Dequeue().Hide();
        }

        private void UpdateReloadProgress()
        {
            _reloadProgress.value = _playerController.NormalizedReloadingTime;
        }

        private void ClearQueue()
        {
            foreach (Transform child in _projectilesQueueContainer.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}