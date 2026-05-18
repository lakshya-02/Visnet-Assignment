using System;
using System.Collections;
using UnityEngine;

namespace VisnetXR.API
{
    /// <summary>
    /// Project and floor API facade consumed by the selector UI.
    /// </summary>
    public class ProjectAPI : MonoBehaviour
    {
        [SerializeField] private APIManager apiManager;

        private void Awake()
        {
            if (apiManager == null)
            {
                apiManager = GetComponent<APIManager>();
            }
        }

        public IEnumerator GetProjects(Action<ApiResult<ProjectListResponse>> onComplete)
        {
            yield return apiManager.Get<ProjectListResponse>("/api/projects", onComplete);
        }

        public IEnumerator GetFloors(int projectId, Action<ApiResult<FloorsResponse>> onComplete)
        {
            yield return apiManager.Get<FloorsResponse>($"/api/projects/{projectId}/floors", onComplete);
        }
    }
}
