using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class LevelMenu : MonoBehaviour
    {
        [SerializeField] private GameObject levelItemPrefab;
        [SerializeField] private Transform grid;

        private void Start()
        {
            gameObject.SetActive(false);
        }

        public void CreateLevelItems(List<int> levelIDs)
        {
            foreach (var id in levelIDs)
            {
                var item = Instantiate(levelItemPrefab, grid, false);
                item.GetComponent<LevelItem>().Initialize(id);
            }
        }
    }
}