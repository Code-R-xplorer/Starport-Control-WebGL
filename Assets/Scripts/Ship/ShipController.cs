﻿using System;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;


namespace Ship
{
    public class ShipController : MonoBehaviour
    {
        [Header("Sprites")]
        [SerializeField] private GameObject indicator;
        [SerializeField] private GameObject tooCloseAlert;
        [SerializeField] private Transform trails;
        [Header("Movement")]
        [SerializeField] private float speed;
        [SerializeField] private float rotationSpeed;
        [Header("Fuel")]
        [SerializeField] private float fuelDecreaseRate;
        [SerializeField] private float fuelDecreaseAmount;
        [SerializeField] private Color fullFuelColor;
        [SerializeField] private Color emptyFuelColor;
        [Header("Path")]
        [SerializeField] private Material dottedLineMaterial;
        [SerializeField] private Material solidLineMaterial;
        [SerializeField] private float dottedLineWidth;
        [SerializeField] private float solidLineWidth;
        [Header("Screen")]
        [SerializeField] private float boundaryBuffer = 0.05f; // Extra space to trigger the off-screen event
        
        private bool _vip;

        private LineRenderer _lineRenderer;

        private List<Vector3> _path;
        private Vector3 _currentPoint;
        private bool _usePath;
        private bool _canFly; 

        private ShipAnimation _shipAnimation;

        private SpriteRenderer[] _trailSpriteRenderers;

        private float _currentDecreaseTime;
        private float _fuel = 1f;
        private bool _fuelEmpty;

        private bool _waitingForPath;
        private LandingPad _landingPad;
        private Camera _camera;
        private bool _hasEnteredScreen;


        private void Start()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            _shipAnimation = GetComponent<ShipAnimation>();
            _shipAnimation.OnLandingFinished += DestroyShip;
            _shipAnimation.OnIndicatorFinished += () =>
            {
                Destroy(indicator);
                _canFly = true;
            };
            transform.up = Vector3.zero - transform.position;

            _trailSpriteRenderers = new SpriteRenderer[trails.childCount];
            for (int i = 0; i < trails.childCount; i++)
            {
                _trailSpriteRenderers[i] = trails.GetChild(i).GetComponent<SpriteRenderer>();
                _trailSpriteRenderers[i].color = fullFuelColor;
            }

            _currentDecreaseTime = fuelDecreaseRate;
            _vip = gameObject.name.Contains("VIP");
            _camera = Camera.main;
        }

        private void Update()
        {
            if (!_canFly) return;
            UpdateFuel();
            CheckIfOffscreen();
            if (_currentPoint != Vector3.zero) SmoothRotation();
            if (_usePath) FlyAlongPath();
            else transform.position += transform.up * (Time.deltaTime * speed);
        }

        private void SmoothRotation()
        {
            // Calculate the direction to the current point
            Vector3 targetDirection = (_currentPoint - transform.position).normalized;

            // Smoothly interpolate between current direction (transform.up) and target direction
            Vector3 smoothDirection = Vector3.Slerp(transform.up, targetDirection, rotationSpeed * Time.deltaTime);

            // Set the new smoothed direction as the ship's up direction
            transform.up = smoothDirection;
        }

        private void FlyAlongPath()
        {
            if (_path.Count < 3 && _waitingForPath)
            {
                transform.position = Vector3.MoveTowards(transform.position, InputManager.Instance.Position, Time.deltaTime * speed);
                return;
            }
            
            _waitingForPath = false;
            
            _path[0] = transform.position;
            _lineRenderer.SetPosition(0, transform.position);
            if (Vector3.Distance(_currentPoint, transform.position) < 0.1f)
            {
                TravelToNextPoint();
            }
            else transform.position = Vector3.MoveTowards(transform.position, _currentPoint, Time.deltaTime * speed);
        }

        private void UpdateFuel()
        {
            _currentDecreaseTime -= Time.deltaTime;
            if (_currentDecreaseTime <= 0)
            {
                _fuel -= fuelDecreaseAmount;
                if (_fuel <= 0)
                {
                    _fuel = 0;
                    _canFly = false;
                    LevelManager.Instance.GameOver("A ship ran out of fuel!");
                }
                _currentDecreaseTime = fuelDecreaseRate;
                var trailColour = Color.Lerp(emptyFuelColor, fullFuelColor, _fuel);
                foreach (var spriteRenderer in _trailSpriteRenderers)
                {
                    spriteRenderer.color = trailColour;
                }
            }
        }

        public void StartPath(Vector3 point)
        {
            _path = new List<Vector3>()
            {
                transform.position,
                point
            };
            _currentPoint = point;
            transform.up = _currentPoint - transform.position;
            _usePath = true;
            _waitingForPath = true;
            SetLineAppearance(dottedLineMaterial, dottedLineWidth);
        }

        private void SetLineAppearance(Material material, float lineWidth)
        {
            _lineRenderer.material = material;
            _lineRenderer.startWidth = lineWidth;
            _lineRenderer.endWidth = lineWidth;
        }

        private void TravelToNextPoint()
        {
            if (_path.Count == 1)
            {
                ResetPath();
                return;
            }
            
            _currentPoint = _path[1];
            _path.Remove(_path[1]);
            _lineRenderer.positionCount = _path.Count;
            _lineRenderer.SetPositions(_path.ToArray());
        }

        private void ResetPath()
        {
            _lineRenderer.SetPositions(Array.Empty<Vector3>());
            _lineRenderer.positionCount = 0;
            _usePath = false;
            transform.up = _currentPoint - transform.position; 
            _currentPoint = Vector3.zero;
        }

        public void AddPathPoint(Vector3 point)
        {
            _path.Add(point);
            _lineRenderer.positionCount = _path.Count;
            _lineRenderer.SetPositions(_path.ToArray());
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.collider.CompareTag(Tags.Ship))
            {
                SetTooCloseAlert(false);
                LevelManager.Instance.ShipsCollided(transform.position);
                _canFly = false;
                gameObject.SetActive(false);
            }

            if (other.collider.CompareTag(Tags.Pad))
            {
                _landingPad = other.gameObject.GetComponent<LandingPad>();
                _landingPad.ShipLanding(true);
                if (_landingPad.VipPad)
                {
                    if (_vip)
                    {
                        _landingPad.VipLanded();
                    }
                    else
                    {
                        LevelManager.Instance.GameOver("Another ship landed on the VIP pad!");
                    }
                }
                else
                {
                    if(_vip) LevelManager.Instance.GameOver("The VIP landed on the wrong pad!");
                }
                // Landing logic here
                SetTooCloseAlert(false);
                _canFly = false;
                transform.position = other.transform.position;
                _lineRenderer.SetPositions(Array.Empty<Vector3>());
                _lineRenderer.positionCount = 0;
                _shipAnimation.PlayLandingAnimation();
                AudioManager.Instance.PlayOneShotWithRandomPitch("shipLand", 0.9f, 1.1f);
            }
        }

        private void SetTooCloseAlert(bool tooClose)
        {
            if (tooClose)
            {
                tooCloseAlert.SetActive(true);
                AudioManager.Instance.Play("shipClose");
            }
            else
            {
                tooCloseAlert.SetActive(false);
                AudioManager.Instance.Stop("shipClose");
            }
            
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if(!other.CompareTag(Tags.Ship) || !_canFly) return;
            SetTooCloseAlert(true);
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            if(!other.CompareTag(Tags.Ship) || !_canFly) return;
            SetTooCloseAlert(false);
        }

        private void DestroyShip()
        {
            LevelManager.Instance.ShipLanded(_vip);
            _landingPad.ShipLanding(false);
            Destroy(gameObject);
        }

        private void CheckIfOffscreen()
        {
            Vector3 viewportPos = _camera.WorldToViewportPoint(transform.position);

            // Check if the ship has entered the screen for the first time
            if (!_hasEnteredScreen)
            {
                if (viewportPos.x > 0 && viewportPos.x < 1 && viewportPos.y > 0 && viewportPos.y < 1)
                {
                    _hasEnteredScreen = true; // Mark that the ship has entered the screen
                }
                return; // Ignore offscreen check until the ship has entered the screen
            }

            // Check if the ship is outside the visible screen area after entering the screen
            if (viewportPos.x < -boundaryBuffer || viewportPos.x > 1 + boundaryBuffer ||
                viewportPos.y < -boundaryBuffer || viewportPos.y > 1 + boundaryBuffer)
            {
                HandleOffscreen();
            }
        }

        private void HandleOffscreen()
        {
            ResetPath();

            // Randomize a new direction to rotate back on screen
            var randX = Random.Range(-20f, 20f);
            var randY = Random.Range(-20f, 20f);
            Vector3 up = Vector3.zero - transform.position;
            up.x += randX;
            up.y += randY;

            // Rotate the ship back towards the screen
            transform.up = up;
        }
        
        public void AddFinalPoint(Vector3 point)
        {
            _path.Add(point);
            _lineRenderer.positionCount = _path.Count;
            _lineRenderer.SetPositions(_path.ToArray());
            SetLineAppearance(solidLineMaterial, solidLineWidth);
        }
    }
}