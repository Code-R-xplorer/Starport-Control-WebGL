using TMPro;
using UnityEngine;

namespace Utilities
{
    public class VersionDisplay : MonoBehaviour
    {
        private void Start()
        {
            GetComponent<TextMeshProUGUI>().text = $"Version: {Application.version}";
        }
    }
}