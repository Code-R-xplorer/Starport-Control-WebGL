using System;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace Ship
{
    public class ShipPathManager : MonoBehaviour
    {
        [SerializeField] private float recordInterval;

        private float _currentRecordInterval;
        private bool _recordPath;
        private LineRenderer _lineRenderer;
        
        private List<Vector3> _currentPathPoints;
        private Camera _camera;
        
        private void Start()
        {
            InputManager.Instance.OnPrimary += RecordPath;
            _camera = Camera.main;
            _lineRenderer = GetComponent<LineRenderer>();
        }

        private void Update()
        {
            if (_recordPath)
            {
                _currentRecordInterval -= Time.deltaTime;
                if (_currentRecordInterval <= 0)
                {
                    var pos = _camera.ScreenToWorldPoint(InputManager.Instance.Position);
                    pos.z = 0;
                    _currentPathPoints.Add(pos);
                    _lineRenderer.positionCount++;
                    _lineRenderer.SetPositions(_currentPathPoints.ToArray());
                    _currentRecordInterval = recordInterval;
                }
                
            }
        }

        private void RecordPath(bool cancel)
        {
            if (!cancel)
            {
                var rayHit = Physics2D.GetRayIntersection(_camera.ScreenPointToRay(InputManager.Instance.Position));
                if (!rayHit)
                {
                    Debug.Log("Path did not start with ship");
                    return;
                }
                if (rayHit.collider.CompareTag(Tags.Ship))
                {
                    _recordPath = true;
                    _currentRecordInterval = recordInterval;
                    _currentPathPoints = new List<Vector3>();
                    var pos = _camera.ScreenToWorldPoint(InputManager.Instance.Position);
                    pos.z = 0;
                    _currentPathPoints.Add(pos);
                    _lineRenderer.positionCount = 1;
                    _lineRenderer.SetPositions(_currentPathPoints.ToArray());
                }
                else
                {
                    Debug.Log("Path did not start with ship");
                }
            }
            else
            {
                if (!_recordPath)
                {
                    _lineRenderer.SetPositions(Array.Empty<Vector3>());
                    _lineRenderer.positionCount = 0;
                    return;
                }
                _recordPath = false;
                _currentPathPoints.Add(InputManager.Instance.Position);
                // Assign path to ship logic here

            }
        }
    }
}