using System;

namespace VisnetXR.API
{
    [Serializable]
    public class LoginRequest
    {
        public string username;
        public string password;
    }

    [Serializable]
    public class UserData
    {
        public int id;
        public string name;
    }

    [Serializable]
    public class LoginResponse
    {
        public bool success;
        public string token;
        public UserData user;
        public string message;
    }

    [Serializable]
    public class ProjectData
    {
        public int id;
        public string name;
    }

    [Serializable]
    public class ProjectListResponse
    {
        public ProjectData[] projects;
    }

    [Serializable]
    public class FloorsResponse
    {
        public string[] floors;
    }

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
