using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

namespace UI
{
    public class PauseMenu : MonoBehaviour
    {
       
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private GameObject mainMenuButton;

        private bool _paused;
        private bool _inMainMenu;
        private AudioMixer _audioMixer;

        private GameObject _mainMenuCanvas;
        
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += (scene, mode) =>
            {
                Debug.Log("SceneLoaded");
                if (scene.name == "Main_Menu")
                {
                    _inMainMenu = true;
                    title.text = "Options";
                    _mainMenuCanvas = GameObject.Find("MainMenu");
                    mainMenuButton.SetActive(false);
                }
                else
                {
                    _inMainMenu = false;
                    title.text = "Paused";
                    UIManager.Instance.pauseMenu = this;
                    mainMenuButton.SetActive(true);
                }
            };
        }

        private void Start()
        {
            gameObject.SetActive(false);
            _audioMixer = GameManager.Instance.audioMixer;
        }

        public void ToggleMenu()
        {
            _paused = !_paused;
            if (_paused)
            {
                gameObject.SetActive(true);
                if (_inMainMenu)  _mainMenuCanvas.SetActive(false);
                else
                {
                    InputManager.Instance.AllowInput(false);
                    Time.timeScale = 0f;
                    UIManager.Instance.ShowHUD(false);
                    UIManager.Instance.ShowBlur(true);
                }
            }
            else
            {
                gameObject.SetActive(false);
                if(_inMainMenu) _mainMenuCanvas.SetActive(true);
                else
                {
                    InputManager.Instance.AllowInput(true);
                    Time.timeScale = 1f;
                    UIManager.Instance.ShowHUD(true);
                    UIManager.Instance.ShowBlur(false);
                }
            }
        }

        public void BMainMenu()
        {
            ToggleMenu();
            GameManager.Instance.LoadMainMenu();
        }
        
        public void SetMasterVol(float value)
        {
            var volume = Mathf.Log10(value) * 20;
            _audioMixer.SetFloat("MasterVol", volume);
        }
        
        public void SetMusicVol(float value)
        {
            var volume = Mathf.Log10(value) * 20;
            _audioMixer.SetFloat("MusicVol", volume);
        }
        
        public void SetSfxVol(float value)
        {
            var volume = Mathf.Log10(value) * 20;
            _audioMixer.SetFloat("SfxVol", volume);
        }
    }
}