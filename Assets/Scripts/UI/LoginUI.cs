using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VisnetXR.UI
{
    /// <summary>
    /// Inspector bindings for the login panel.
    /// </summary>
    public class LoginUI : MonoBehaviour
    {
        public GameObject panel;
        public TMP_InputField usernameInput;
        public TMP_InputField passwordInput;
        public Button loginButton;
        public TMP_Text statusText;
    }
}
