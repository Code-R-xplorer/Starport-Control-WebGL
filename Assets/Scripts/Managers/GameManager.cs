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
        
        public ShipSpawner ShipSpawner { get; private set; }

        private void Awake()
        {
            Instance = this;
            ShipSpawner = shipSpawner;
        }

        private void Start()
        {
            Time.timeScale = 1f;
            InputManager.Instance.AllowInput(true);
            shipSpawner.SetupSpawner(totalShipsToSpawn, timeBetweenSpawns);
        }

        public void ShipLanded(bool vip)
        {
            // if(!vip && shipSpawner.VipSpawned) GameOver();
            if (vip)
            {
                shipSpawner.VipLanded();
            }
            _shipsLanded++;
            UIManager.Instance.hud.UpdateInfo(shipsLanded: _shipsLanded);

            if (_shipsLanded == totalShipsToSpawn) GameWin();
        }


        private void GameWin()
        {
            // Show Game Win UI
            // Stop the game
            InputManager.Instance.AllowInput(false);
            onGameWin?.Invoke(_shipsLanded);
            Time.timeScale = 0f;
            AudioManager.Instance.StopAllAudio();
            AppManager.Instance.LevelCompleted();
        }

        public void GameOver()
        {
            // Show Game Over UI
            // Stop the game
            InputManager.Instance.AllowInput(false);
            shipSpawner.StopSpawningShips();
            onGameOver?.Invoke(_shipsLanded);
            Time.timeScale = 0f;
            AudioManager.Instance.StopAllAudio();
        }
    }
}