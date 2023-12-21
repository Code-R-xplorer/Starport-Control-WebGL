using Controls;
using UnityEngine;

namespace Managers
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance { get; private set; }
        private PlayerControls _playerControls;
        
        public delegate void BaseAction();
        public delegate void BoolBaseAction(bool canceled);

        public event BoolBaseAction OnPrimary;

        private void Awake()
        {
            _playerControls = new PlayerControls();
            Instance = this;
        }

        private void Start()
        {
            _playerControls.Controls.Primary.started += context => { OnPrimary?.Invoke(context.canceled); };
            _playerControls.Controls.Primary.canceled += context => { OnPrimary?.Invoke(context.canceled); };
        }

        private void OnEnable()
        {
            _playerControls.Enable();
        }

        private void OnDisable()
        {
            _playerControls.Disable();
        }
    }
}