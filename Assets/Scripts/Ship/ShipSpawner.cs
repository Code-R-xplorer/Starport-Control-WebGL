using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Ship
{
    public class ShipSpawner : MonoBehaviour
    {
        [SerializeField] private List<GameObject> ships;
        private GameObject _ship;
        private Camera _camera;

        private void Start()
        {
            _camera = Camera.main;
        }

        private void SpawnShip()
        {
            _ship = ships[Random.Range(0, ships.Count)];
            switch (Random.Range(0,4))
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
    }
}