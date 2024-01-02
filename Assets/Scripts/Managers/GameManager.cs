using System;
using Ship;
using UnityEngine;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        
        [SerializeField] private int totalShipsToSpawn;
        [SerializeField] private float timeBetweenSpawns;

        [SerializeField] private ShipSpawner shipSpawner;

        public int ShipsLanded { get; private set; }
        private float _currentTimeBetweenSpawns;
        private bool _canSpawnShips = true;

        private int _spawnedShips;

        public event Action onGameWin;
        public event Action onGameOver;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            _currentTimeBetweenSpawns = timeBetweenSpawns;
        }

        private void Update()
        {
            if (_canSpawnShips)
            {
                if (_spawnedShips < totalShipsToSpawn)
                {
                    _currentTimeBetweenSpawns -= Time.deltaTime;
                    if (_currentTimeBetweenSpawns <= 0)
                    {
                        shipSpawner.SpawnShip();
                        _spawnedShips++;
                        _currentTimeBetweenSpawns = timeBetweenSpawns;
                    }
                }
                else _canSpawnShips = false;
            }
        }

        public void ShipLanded()
        {
            ShipsLanded++;

            if (ShipsLanded == totalShipsToSpawn) GameWin();
        }

        private void GameWin()
        {
            // Show Game Win UI
            // Stop the game
            
            onGameWin?.Invoke();
        }

        public void GameOver()
        {
            // Show Game Over UI
            // Stop the game
            _canSpawnShips = false;
            onGameOver?.Invoke();
        }
    }
}