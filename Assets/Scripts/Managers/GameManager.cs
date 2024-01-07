using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public AudioMixer audioMixer;
        
        public Dictionary<int, bool> levels;
        private List<int> _levelIDs;
        public string levelPrefix;

        private int _currentSceneID;

        private void Awake()
        {
            // Check if there is already an AppManager instance and if it's different from this instance.
            if (Instance != null && Instance != this)
            {
                // Destroy this game object if there's already an AppManager instance.
                Destroy(gameObject);
                return;
            }

            // Set the AppManager instance to this instance.
            Instance = this;
            // Make sure the AppManager instance is not destroyed when loading a new scene.
            DontDestroyOnLoad(gameObject);
            
            SceneManager.sceneLoaded += (scene, mode) =>
            {
                if (scene.name == "Main_Menu")
                {
                    GameObject.Find("LevelSelect").GetComponent<LevelMenu>().CreateLevelItems(_levelIDs);
                }
            };
            levels = new Dictionary<int, bool>();
            _levelIDs = new List<int>();
            for (int i = 1; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                _levelIDs.Add(i);
            }
            
            foreach (var id in _levelIDs)
            {
                levels.Add(id,false);
            }

            levels[1] = true;
        }
        
        public bool LevelUnlocked(int levelID)
        {
            return levels[levelID];
        }
        
        public void LoadLevel(int levelID)
        {
            _currentSceneID = levelID;
            SceneManager.LoadScene($"{levelPrefix}{levelID}");
        }

        public void RestartLevel()
        {
            SceneManager.LoadScene($"{levelPrefix}{_currentSceneID}");
        }

        public void LoadNextLevel()
        {
            if (_currentSceneID < SceneManager.sceneCountInBuildSettings - 1)
            {
                _currentSceneID++;
                LoadLevel(_currentSceneID);
            }
        }

        public void LoadMainMenu()
        {
            SceneManager.LoadScene("Main_Menu");
            
        }
        
        public void LevelCompleted()
        {
            levels[_currentSceneID + 1] = true;
        }
        
        public bool FinalLevel()
        {
            return _currentSceneID == SceneManager.sceneCountInBuildSettings - 1;
        }
    }
}