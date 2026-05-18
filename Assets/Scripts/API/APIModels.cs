using System;

namespace VisnetXR.API
{
    /// <summary>
    /// Login payload sent to the backend.
    /// </summary>
    [Serializable]
    public class LoginRequest
    {
        public string username;
        public string password;
    }

    /// <summary>
    /// Minimal user profile returned after login.
    /// </summary>
    [Serializable]
    public class UserData
    {
        public int id;
        public string name;
    }

    /// <summary>
    /// Authentication response used to seed the runtime session.
    /// </summary>
    [Serializable]
    public class LoginResponse
    {
        public bool success;
        public string token;
        public UserData user;
        public string message;
    }

    /// <summary>
    /// Project option shown in the left selector column.
    /// </summary>
    [Serializable]
    public class ProjectData
    {
        public int id;
        public string name;
    }

    /// <summary>
    /// Project collection returned by the projects API route.
    /// </summary>
    [Serializable]
    public class ProjectListResponse
    {
        public ProjectData[] projects;
    }

    /// <summary>
    /// Floor collection returned for a selected project.
    /// </summary>
    [Serializable]
    public class FloorsResponse
    {
        public string[] floors;
    }

    /// <summary>
    /// Lightweight success/error envelope for coroutine API calls.
    /// </summary>
    public readonly struct ApiResult<T>
    {
        public readonly bool IsSuccess;
        public readonly T Data;
        public readonly string Error;

        public ApiResult(T data)
        {
            IsSuccess = true;
            Data = data;
            Error = string.Empty;
        }

        public ApiResult(string error)
        {
            IsSuccess = false;
            Data = default;
            Error = error;
        }
    }
}
