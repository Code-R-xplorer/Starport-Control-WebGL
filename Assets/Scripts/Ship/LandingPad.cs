using UnityEngine;

namespace Ship
{
    public class LandingPad : MonoBehaviour
    {
        private Animator _animator;

        private readonly int _flashLights = Animator.StringToHash("FlashLights");
        private readonly int _baseState = Animator.StringToHash("Base State");

        public bool VipPad { get; private set; }
        public bool Occupied { get; private set; }

        private void Start()
        {
            _animator = GetComponent<Animator>();
        }

        public void ShipLanding(bool landing)
        {
            Occupied = landing;
        }

        public void SetAsVip()
        {
            VipPad = true;
            _animator.Play(_flashLights, -1, 0f);
        }

        public void VipLanded()
        {
            VipPad = false;
            _animator.Play(_baseState, -1, 0f);
        }
    }
}