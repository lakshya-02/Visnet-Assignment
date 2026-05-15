using System.Collections.Generic;
using UnityEngine;

namespace VisnetXR.Managers
{
    public class NavigationManager : MonoBehaviour
    {
        private readonly Dictionary<string, GameObject> panels = new();
        private readonly Stack<string> history = new();
        private string currentPanel;

        public void RegisterPanel(string id, GameObject panel)
        {
            panels[id] = panel;
            panel.SetActive(false);
        }

        public void Show(string id, bool rememberCurrent = true)
        {
            if (!panels.ContainsKey(id))
            {
                Debug.LogWarning($"Panel '{id}' has not been registered.");
                return;
            }

            if (!string.IsNullOrEmpty(currentPanel) && rememberCurrent)
            {
                history.Push(currentPanel);
            }

            foreach (GameObject panel in panels.Values)
            {
                panel.SetActive(false);
            }

            currentPanel = id;
            panels[id].SetActive(true);
        }

        public void Back()
        {
            if (history.Count == 0)
            {
                return;
            }

            Show(history.Pop(), false);
        }

        public void ResetTo(string id)
        {
            history.Clear();
            Show(id, false);
        }
    }
}
