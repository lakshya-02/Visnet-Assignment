using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VisnetXR.UI
{
    /// <summary>
    /// Inspector bindings for the final confirmation panel.
    /// </summary>
    public class SummaryUI : MonoBehaviour
    {
        public GameObject panel;
        public TMP_Text userText;
        public TMP_Text projectText;
        public TMP_Text floorText;
        public TMP_Text statusText;
        public Button backButton;
        public Button restartButton;
    }
}
