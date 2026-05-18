using System;
using System.Collections;
using UnityEngine;

namespace VisnetXR.API
{
    /// <summary>
    /// Auth-specific API facade for the login flow.
    /// </summary>
    public class AuthAPI : MonoBehaviour
    {
        [SerializeField] private APIManager apiManager;

        private void Awake()
        {
            if (apiManager == null)
            {
                apiManager = GetComponent<APIManager>();
            }
        }

        public IEnumerator Login(string username, string password, Action<ApiResult<LoginResponse>> onComplete)
        {
            var request = new LoginRequest
            {
                username = username,
                password = password
            };

            yield return apiManager.Post<LoginRequest, LoginResponse>("/api/login", request, onComplete);
        }
    }
}
