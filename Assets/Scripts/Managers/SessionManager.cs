using UnityEngine;
using VisnetXR.API;

namespace VisnetXR.Managers
{
    /// <summary>
    /// Stores the active user and selection state for the current XR session.
    /// </summary>
    public class SessionManager : MonoBehaviour
    {
        public string Token { get; private set; }
        public UserData User { get; private set; }
        public ProjectData SelectedProject { get; private set; }
        public string SelectedFloor { get; private set; }

        public void SetLogin(LoginResponse response)
        {
            Token = response.token;
            User = response.user;
        }

        public void SetProject(ProjectData project)
        {
            SelectedProject = project;
            SelectedFloor = string.Empty;
        }

        public void SetFloor(string floor)
        {
            SelectedFloor = floor;
        }

        public void Clear()
        {
            Token = string.Empty;
            User = null;
            SelectedProject = null;
            SelectedFloor = string.Empty;
        }
    }
}
