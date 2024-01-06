﻿using Managers;
using UnityEngine;
using Utilities;

namespace Ship
{
    public class ShipPathManager : MonoBehaviour
    {
        [SerializeField] private float recordInterval;

        private float _currentRecordInterval;
        private bool _recordPath;
        private Camera _camera;

        private ShipController _selectedShipController;
        
        private void Start()
        {
            InputManager.Instance.OnPrimary += RecordPath;
            _camera = Camera.main;
        }

        private void Update()
        {
            if (!_recordPath) return;
            _currentRecordInterval -= Time.deltaTime;
            if (_currentRecordInterval <= 0)
            {
                var pos = _camera.ScreenToWorldPoint(InputManager.Instance.Position);
                pos.z = 0;
                _selectedShipController.AddPathPoint(pos);
                _currentRecordInterval = recordInterval;
            }
        }

        private void RecordPath(bool cancel)
        {
            if (!cancel)
            {
                var rayHit = Physics2D.GetRayIntersection(_camera.ScreenPointToRay(InputManager.Instance.Position));
                if (!rayHit)
                {
                    AudioManager.Instance.PlayOneShot("error");
                    return;
                }
                if (rayHit.collider.CompareTag(Tags.Ship))
                {
                    _selectedShipController = rayHit.transform.GetComponent<ShipController>();
                    if (GameManager.Instance.ShipSpawner.VipSpawned && !_selectedShipController.IsVip())
                    {
                        AudioManager.Instance.PlayOneShot("error");
                        _recordPath = false;
                        return;
                    }
                    _recordPath = true;
                    _currentRecordInterval = recordInterval;
                    var pos = _camera.ScreenToWorldPoint(InputManager.Instance.Position);
                    pos.z = 0;
                    _selectedShipController.StartPath(pos);
                }
                else
                {
                    AudioManager.Instance.PlayOneShot("error");
                }
            }
            else
            {
                if (!_recordPath) return;
                _recordPath = false;
                var pos = _camera.ScreenToWorldPoint(InputManager.Instance.Position);
                pos.z = 0;
                _selectedShipController.AddPathPoint(pos);
                
            }
        }
    }
}