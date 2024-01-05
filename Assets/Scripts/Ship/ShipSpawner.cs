using System;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;

namespace Ship
{
    public class ShipSpawner : MonoBehaviour
    {
        [SerializeField] private List<GameObject> ships;
        private GameObject _ship;
        private Camera _camera;

        private bool _canSpawnShip;
        
        private float _currentTimeBetweenSpawns;
        private bool _canSpawnShips;
        private int _totalShipsToSpawn;
        private float _timeBetweenSpawns;

        private bool _canSpawnVip = true;
        
        public bool VipSpawned { get; private set; }

        private void Start()
        {
            _camera = Camera.main;
        }

        public void SetupSpawner(int totalShips, float timeBetweenSpawns)
        {
            _totalShipsToSpawn = totalShips;
            _timeBetweenSpawns = timeBetweenSpawns;
            _currentTimeBetweenSpawns = _timeBetweenSpawns;
            _canSpawnShips = true;
            _canSpawnShip = true;
            UIManager.Instance.hud.UpdateInfo(inboundShips: _totalShipsToSpawn);
        }

        public void StopSpawningShips()
        {
            _canSpawnShips = false;
        }

        private void Update()
        {
            if (_canSpawnShips)
            {
                if(!_canSpawnShip) return;
                if (_totalShipsToSpawn != 0)
                {
                    _currentTimeBetweenSpawns -= Time.deltaTime;
                    if (_currentTimeBetweenSpawns <= 0)
                    {
                        SpawnShip();
                        _totalShipsToSpawn--;
                        _currentTimeBetweenSpawns = _timeBetweenSpawns;
                        UIManager.Instance.hud.UpdateInfo(inboundShips: _totalShipsToSpawn);
                    }
                }
                else _canSpawnShips = false;
            }
        }

        private void SpawnShip()
        {
            _ship = ships[Random.Range(0, ships.Count)];
            if (_ship.name == "VIP_Ship")
            {
                if (_canSpawnVip)
                {
                    _canSpawnVip = false;
                    VipSpawned = true;
                }
                else
                {
                    SpawnShip();
                    return;
                }
            }
            var r = Random.Range(0, 4);
            switch (r)
            {
                case 0:
                    SpawnAtTopEdge();
                    break;
                case 1:
                    SpawnAtBottomEdge();
                    break;
                case 2:
                    SpawnAtLeftEdge();
                    break;
                case 3:
                    SpawnAtRightEdge();
                    break;
            }
        }

        private void SpawnAtRightEdge()
        {
            Vector2 spawnPosition = _camera.ViewportToWorldPoint(new Vector2(1, 0f));
            
            spawnPosition.x += 1;
            
            spawnPosition.y += Random.Range(0f, 1f);
            
            Instantiate(_ship, spawnPosition, Quaternion.identity);
        }
        
        private void SpawnAtLeftEdge()
        {
            Vector2 spawnPosition = _camera.ViewportToWorldPoint(new Vector2(0f, 0f));
            
            spawnPosition.x -= 1;
            
            spawnPosition.y += Random.Range(0f, 1f);
            
            Instantiate(_ship, spawnPosition, Quaternion.identity);
        }
        
        private void SpawnAtTopEdge()
        {
            Vector2 spawnPosition = _camera.ViewportToWorldPoint(new Vector2(0, 0f));
            
            spawnPosition.y -= 1;
            
            spawnPosition.x += Random.Range(0f, 1f);
            
            Instantiate(_ship, spawnPosition, Quaternion.identity);
        }
        
        private void SpawnAtBottomEdge()
        {
            Vector2 spawnPosition = _camera.ViewportToWorldPoint(new Vector2(0, 1f));
            
            spawnPosition.y += 1;
            
            spawnPosition.x += Random.Range(0f, 1f);

            Instantiate(_ship, spawnPosition, Quaternion.identity);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if(!other.CompareTag(Tags.Ship)) return;
            _canSpawnShip = false;

        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if(!other.CompareTag(Tags.Ship)) return;
            _canSpawnShip = false;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if(!other.CompareTag(Tags.Ship)) return;
            _currentTimeBetweenSpawns = _timeBetweenSpawns;
            _canSpawnShip = true;
        }

        public void VipLanded()
        {
            _canSpawnVip = true;
            VipSpawned = false;
        }
    }
}