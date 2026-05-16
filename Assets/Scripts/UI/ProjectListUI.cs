using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VisnetXR.UI
{
    public class ProjectListUI : MonoBehaviour
    {
        public GameObject panel;
        public RectTransform listRoot;
        public Button itemButtonTemplate;
        public Button backButton;
        public Button continueButton;
        public TMP_Text selectedProjectText;
        public TMP_Text statusText;
    }
}
