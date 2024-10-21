using Managers;
using UnityEngine;
using Utilities;

namespace Ship
{
    public class ShipPathManager : MonoBehaviour
    {
        [SerializeField] private float recordDistance = 0.5f; // Minimum distance between points

        private bool _recordPath;
        private Camera _camera;
        private ShipController _selectedShipController;
        private Vector3 _lastRecordedPosition; // Store the last recorded position

        private void Start()
        {
            InputManager.Instance.OnPrimary += RecordPath;
            _camera = Camera.main;
        }

        private void Update()
        {
            if (!_recordPath) return;

            var currentPosition = _camera.ScreenToWorldPoint(InputManager.Instance.Position);
            currentPosition.z = 0;

            // Check distance between last recorded point and current position
            if (Vector3.Distance(_lastRecordedPosition, currentPosition) >= recordDistance)
            {
                _selectedShipController.AddPathPoint(currentPosition);
                _lastRecordedPosition = currentPosition; // Update last recorded position
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
                    _recordPath = true;

                    var pos = _camera.ScreenToWorldPoint(InputManager.Instance.Position);
                    pos.z = 0;
                    _selectedShipController.StartPath(pos);
                    _lastRecordedPosition = pos; // Set the first position
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
                _selectedShipController.AddFinalPoint(pos);
                _selectedShipController = null;
            }
        }
    }
}
