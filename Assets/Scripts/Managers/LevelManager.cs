using System;
using System.Collections.Generic;
using Ship;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;

namespace Managers
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance { get; private set; }
        
        [SerializeField] private int totalShipsToSpawn;
        [SerializeField] private float timeBetweenSpawns;

        [SerializeField] private ShipSpawner shipSpawner;

        private int _shipsLanded;
        private float _currentTimeBetweenSpawns;

        private int _spawnedShips;

        private List<LandingPad> _landingPads;

        public ShipSpawner ShipSpawner => shipSpawner;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            Time.timeScale = 1f;
            InputManager.Instance.AllowInput(true);
            shipSpawner.SetupSpawner(totalShipsToSpawn, timeBetweenSpawns);
            _landingPads = new List<LandingPad>();
            foreach (var pad in  GameObject.FindGameObjectsWithTag(Tags.Pad))
            {
                _landingPads.Add(pad.GetComponent<LandingPad>());
            }
            Debug.Log(_landingPads.Count);
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
            UIManager.Instance.ShowGameWin(_shipsLanded);
            Time.timeScale = 0f;
            AudioManager.Instance.StopAllAudio();
            GameManager.Instance.LevelCompleted();
        }

        public void GameOver(string reason)
        {
            // Show Game Over UI
            // Stop the game
            InputManager.Instance.AllowInput(false);
            shipSpawner.StopSpawningShips();
            UIManager.Instance.ShowGameOver(_shipsLanded, reason);
            Time.timeScale = 0f;
            AudioManager.Instance.StopAllAudio();
        }

        public void PickRandomPad()
        {
            var r = Random.Range(0, _landingPads.Count);
            _landingPads[r].SetAsVip();
        }
    }
}