using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class HUD : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI inboundShipsNumberText;
        [SerializeField] private TextMeshProUGUI shipsLandedNumberText;
        [SerializeField] private TextMeshProUGUI levelNumberText;
        
        private void Start()
        {
            if(SceneManager.GetActiveScene().name == "_Dev") return;
            var levelNumber = int.Parse(SceneManager.GetActiveScene().name.Split("_")[1]);
            levelNumberText.text = levelNumber.ToString("D3");
        }

        public void UpdateInfo(int inboundShips = -1, int shipsLanded = -1)
        {
            if(inboundShips != -1) inboundShipsNumberText.text = inboundShips.ToString("D3");
            if(shipsLanded != -1) shipsLandedNumberText.text = shipsLanded.ToString("D3");
        }
    }
}