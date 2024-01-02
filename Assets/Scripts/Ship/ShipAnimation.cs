using System;
using Managers;
using UnityEngine;

namespace Ship
{
    public class ShipAnimation : MonoBehaviour
    {
        private Animator _animator;
        private static readonly int ShipLand = Animator.StringToHash("ShipLand");

        public event InputManager.BaseAction OnAnimationFinished;

        private void Start()
        {
            _animator = GetComponent<Animator>();
        }

        public void PlayLandingAnimation()
        {
            _animator.Play(ShipLand, -1, 0f);
        }

        public void AnimationFinished()
        {
            OnAnimationFinished?.Invoke();
        }
    }
}