using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    public class Ship : MonoBehaviour
    {
        [SerializeField] private float speed;

        private Rigidbody2D _rigidbody;
        private LineRenderer _lineRenderer;

        private List<Vector3> _path;
        private Vector3 _currentPoint;
        private bool _usePath;
        
        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _lineRenderer = GetComponent<LineRenderer>();
        }

        private void FixedUpdate()
        {
            _rigidbody.velocity = transform.up * (speed * Time.deltaTime);
            if(!_usePath) return;
            _path[0] = transform.position;
            _lineRenderer.SetPosition(0, _path[0]);
            if (Vector3.Distance(_currentPoint, transform.position) < 0.1f)
            {
                TravelToNextPoint();
            }
        }

        public void StartPath(Vector3 point)
        {
            _path = new List<Vector3>()
            {
                transform.position,
                point
            };
            TravelToNextPoint();
            _usePath = true;
        }

        private void TravelToNextPoint()
        {
            if (_path.Count == 1)
            {
                _lineRenderer.SetPositions(Array.Empty<Vector3>());
                _lineRenderer.positionCount = 0;
                _usePath = false;
                return;
            }
            
            _currentPoint = _path[1];
            _path.Remove(_path[1]);

            transform.up = _currentPoint - transform.position; 
            _lineRenderer.positionCount = _path.Count;
            _lineRenderer.SetPositions(_path.ToArray());
        }

        public void AddPathPoint(Vector3 point)
        {
            _path.Add(point);
            _lineRenderer.positionCount = _path.Count;
            _lineRenderer.SetPositions(_path.ToArray());
        }
    }
}