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
        
        private LevelManager _levelManager;

        private void Start()
        {
            _camera = Camera.main;
        }

        public void SetupSpawner(int totalShips, float timeBetweenSpawns, LevelManager levelManager)
        {
            _totalShipsToSpawn = totalShips;
            _timeBetweenSpawns = timeBetweenSpawns;
            _levelManager = levelManager;
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
                        AudioManager.Instance.PlayOneShot("shipEnter");
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
                    LevelManager.Instance.PickRandomPad();
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
                    SpawnAtEdge(new Vector2(0, 0), Random.Range(0f, 1f), -1);
                    break;
                case 1:
                    SpawnAtEdge(new Vector2(0, 1), Random.Range(0f, 1f), 1);
                    break;
                case 2:
                    SpawnAtEdge(new Vector2(0, 0),-1, Random.Range(0f, 1f));
                    break;
                case 3:
                    SpawnAtEdge(new Vector2(1, 0),1, Random.Range(0f, 1f));
                    break;
            }
        }

        private void SpawnAtEdge(Vector2 startPos, float x, float y)
        {
            Vector2 spawnPosition = _camera.ViewportToWorldPoint(startPos);
            
            spawnPosition.x += x;
            
            spawnPosition.y += y;
            
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