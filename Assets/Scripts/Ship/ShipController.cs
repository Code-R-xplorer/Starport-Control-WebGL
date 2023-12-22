using System;
using System.Collections.Generic;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;


namespace Ship
{
    public class ShipController : MonoBehaviour
    {
        [SerializeField] private float speed;

        private Rigidbody2D _rigidbody;
        private LineRenderer _lineRenderer;

        private List<Vector3> _path;
        private Vector3 _currentPoint;
        private bool _usePath;
        private bool _landed;

        private ShipAnimation _shipAnimation;
        
        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _lineRenderer = GetComponent<LineRenderer>();
            _shipAnimation = GetComponent<ShipAnimation>();
            _shipAnimation.OnAnimationFinished += DestroyShip;
        }

        private void FixedUpdate()
        {
            if(_landed) return;
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
                transform.up = _currentPoint - transform.position; 
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

        private void OnCollisionEnter2D(Collision2D other)
        {
            Debug.Log(other.transform.name);
            if (other.collider.CompareTag(Tags.Ship))
            {
                Debug.Log("Collided with other ship");
                // Ship collision logic here
            }

            if (other.collider.CompareTag(Tags.Pad))
            {
                Debug.Log("Land at pad");
                // Landing logic here
                _landed = true;
                _rigidbody.simulated = false;
                transform.position = other.transform.position;
                _lineRenderer.SetPositions(Array.Empty<Vector3>());
                _lineRenderer.positionCount = 0;
                _shipAnimation.PlayLandingAnimation();
            }
        }

        private void DestroyShip()
        {
            Destroy(gameObject);
        }

        void OnBecameInvisible()
        {
            Debug.Log("Out of View");
            // var rot = transform.rotation;
            var randX = Random.Range(-0.5f, 0.5f);
            var randy = Random.Range(-0.5f, 0.5f);
            var up = transform.up;
            up *= -1;
            up.x += randX;
            up.y += randy;
            transform.up = up;
        }
    }
}