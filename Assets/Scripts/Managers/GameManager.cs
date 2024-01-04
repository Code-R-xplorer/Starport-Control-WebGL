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

        private int _shipsLanded;
        private float _currentTimeBetweenSpawns;

        private int _spawnedShips;

        public event Action<int> onGameWin;
        public event Action<int> onGameOver;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            shipSpawner.SetupSpawner(totalShipsToSpawn, timeBetweenSpawns);
        }

        public void ShipLanded()
        {
            _shipsLanded++;
            UIManager.Instance.hud.UpdateInfo(shipsLanded: _shipsLanded);

            if (_shipsLanded == totalShipsToSpawn) GameWin();
        }

        private void GameWin()
        {
            // Show Game Win UI
            // Stop the game
            
            onGameWin?.Invoke(_shipsLanded);
            Time.timeScale = 0f;
        }

        public void GameOver()
        {
            // Show Game Over UI
            // Stop the game
            shipSpawner.StopSpawningShips();
            onGameOver?.Invoke(_shipsLanded);
            Time.timeScale = 0f;
        }
    }
}