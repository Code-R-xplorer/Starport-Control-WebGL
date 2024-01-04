using System;
using Managers;
using UnityEngine;

namespace Ship
{
    public class ShipAnimation : MonoBehaviour
    {
        private Animator _animator;
        private static readonly int ShipLand = Animator.StringToHash("ShipLand");
        private static readonly int HideIndicator = Animator.StringToHash("HideIndicator");

        public event Action OnLandingFinished;
        public event Action OnIndicatorFinished;

        private void Start()
        {
            _animator = GetComponent<Animator>();
            _animator.Play(HideIndicator, -1, 0f);
        }

        public void PlayLandingAnimation()
        {
            _animator.Play(ShipLand, -1, 0f);
        }

        public void AnimationFinished(string input)
        {
            switch (input)
            {
                case "Landing":
                    OnLandingFinished?.Invoke();
                    break;
                case "Indicator":
                    OnIndicatorFinished?.Invoke();
                    break;
            }
        }
    }
}