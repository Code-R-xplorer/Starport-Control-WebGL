using Managers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Utilities
{
    public class ButtonSounds : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            AudioManager.Instance.PlayOneShot("uiSelect");
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            AudioManager.Instance.PlayOneShot("uiHover");
        }
    }
}