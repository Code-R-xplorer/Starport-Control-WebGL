using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class LevelItem : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private GameObject levelLocked, levelUnlocked, levelSelected;
        [SerializeField] private TextMeshProUGUI levelNumberText;

        private int _levelNumber;
        private bool _unlocked;

        public void Initialize(int levelNumber)
        {
            _levelNumber = levelNumber;
            levelNumberText.text = _levelNumber.ToString();
            _unlocked = AppManager.Instance.LevelUnlocked(_levelNumber);
            if (_unlocked)
            {
                levelLocked.SetActive(false);
                levelUnlocked.SetActive(true);
            }
            else
            {
                levelUnlocked.SetActive(false);
            } 
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_unlocked) return;
            AudioManager.Instance.PlayOneShot("uiSelect");
            AppManager.Instance.LoadLevel(_levelNumber);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            levelSelected.SetActive(true);
            AudioManager.Instance.PlayOneShot("uiHover");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            levelSelected.SetActive(false);
        }
    }
}