using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VisnetXR.UI
{
    public class FloorDropdownUI : MonoBehaviour
    {
        public GameObject panel;
        public TMP_Text projectTitle;
        public RectTransform listRoot;
        public Button itemButtonTemplate;
        public Button backButton;
        public Button backToProjectsButton;
        public Button logoutButton;
        public Button continueButton;
        public TMP_Text selectedFloorText;
        public TMP_Text statusText;
    }
}
