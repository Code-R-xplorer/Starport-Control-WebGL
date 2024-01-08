using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;


namespace Ship
{
    public class ShipController : MonoBehaviour
    {
        [SerializeField] private GameObject indicator;
        [SerializeField] private GameObject tooCloseAlert;
        [SerializeField] private Transform trails;
        [SerializeField] private float speed;
        [SerializeField] private float fuelDecreaseRate;
        [SerializeField] private float fuelDecreaseAmount;
        [SerializeField] private Color fullFuelColor;
        [SerializeField] private Color emptyFuelColor;

        [SerializeField] private bool vip;

        private Rigidbody2D _rigidbody;
        private LineRenderer _lineRenderer;

        private List<Vector3> _path;
        private Vector3 _currentPoint;
        private bool _usePath;
        private bool _spawned = true;
        private bool _canFly; 

        private ShipAnimation _shipAnimation;

        private SpriteRenderer[] _trailSpriteRenderers;

        private float _currentDecreaseTime;
        private float _fuel = 1f;
        private bool _fuelEmpty;
        
        
        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _lineRenderer = GetComponent<LineRenderer>();
            _shipAnimation = GetComponent<ShipAnimation>();
            _shipAnimation.OnLandingFinished += DestroyShip;
            _shipAnimation.OnIndicatorFinished += () =>
            {
                Destroy(indicator);
                _canFly = true;
            };
            _shipAnimation.OnExplosionFinished += () =>
            {
                LevelManager.Instance.GameOver("Two ships collided!");
            };
            transform.up = Vector3.zero - transform.position;

            _trailSpriteRenderers = new SpriteRenderer[trails.childCount];
            for (int i = 0; i < trails.childCount; i++)
            {
                _trailSpriteRenderers[i] = trails.GetChild(i).GetComponent<SpriteRenderer>();
                _trailSpriteRenderers[i].color = fullFuelColor;
            }

            _currentDecreaseTime = fuelDecreaseRate;
        }

        private void FixedUpdate()
        {
            if(!_canFly) return;
            _rigidbody.velocity = transform.up * (speed * Time.deltaTime);
            if(!_usePath) return;
            _path[0] = transform.position;
            _lineRenderer.SetPosition(0, _path[0]);
            if (Vector3.Distance(_currentPoint, transform.position) < 0.1f)
            {
                TravelToNextPoint();
            }
        }

        private void Update()
        {
            if (!_canFly) return;
            _currentDecreaseTime -= Time.deltaTime;
            if (_currentDecreaseTime <= 0)
            {
                _fuel -= fuelDecreaseAmount;
                if (_fuel <= 0)
                {
                    _fuelEmpty = true;
                    _fuel = 0;
                }
                _currentDecreaseTime = fuelDecreaseRate;
                var trailColour = Color.Lerp(emptyFuelColor, fullFuelColor, _fuel);
                foreach (var spriteRenderer in _trailSpriteRenderers)
                {
                    spriteRenderer.color = trailColour;
                }
            }

            if (_fuelEmpty)
            {
                _canFly = false;
                LevelManager.Instance.GameOver("A ship ran out of fuel!");
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
            if (other.collider.CompareTag(Tags.Ship))
            {
                Debug.Log("Collided with other ship");
                // Ship collision logic here
                AudioManager.Instance.PlayOneShot("shipCrash");
                _shipAnimation.PlayExplosionAnimation();
                _canFly = false;
                _rigidbody.velocity = Vector2.zero;
            }

            if (other.collider.CompareTag(Tags.Pad))
            {
                Debug.Log("Land at pad");
                var pad = other.gameObject.GetComponent<LandingPad>();
                if (pad.VipPad)
                {
                    if (vip)
                    {
                        pad.VipLanded();
                    }
                    else
                    {
                        LevelManager.Instance.GameOver("Another ship landed on the VIP pad!");
                    }
                }
                else
                {
                    if(vip) LevelManager.Instance.GameOver("The VIP landed on the wrong pad!");
                }
                // Landing logic here
                _canFly = false;
                _rigidbody.simulated = false;
                transform.position = other.transform.position;
                _lineRenderer.SetPositions(Array.Empty<Vector3>());
                _lineRenderer.positionCount = 0;
                _shipAnimation.PlayLandingAnimation();
                AudioManager.Instance.PlayOneShot("shipLand");
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if(!other.CompareTag(Tags.Ship) || !_canFly) return;
            tooCloseAlert.SetActive(true);
            AudioManager.Instance.Play("shipClose");
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            if(!other.CompareTag(Tags.Ship) || !_canFly) return;
            tooCloseAlert.SetActive(false);
            AudioManager.Instance.Stop("shipClose");
        }

        private void DestroyShip()
        {
            LevelManager.Instance.ShipLanded(vip);
            Destroy(gameObject);
        }

        void OnBecameInvisible()
        {
            if (_spawned)
            {
                _spawned = false;
                return;
            }
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

        public bool IsVip()
        {
            return vip;
        }
    }
}