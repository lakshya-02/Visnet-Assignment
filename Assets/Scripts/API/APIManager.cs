using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace VisnetXR.API
{
    /// <summary>
    /// Thin UnityWebRequest wrapper for JSON API calls.
    /// </summary>
    public class APIManager : MonoBehaviour
    {
        public IEnumerator Get<T>(string path, Action<ApiResult<T>> onComplete)
        {
            using UnityWebRequest request = UnityWebRequest.Get(APIConfig.BaseUrl + path);
            request.timeout = 15;
            request.SetRequestHeader("Accept", "application/json");
            yield return request.SendWebRequest();
            onComplete(ParseResponse<T>(request));
        }

        public IEnumerator Post<TRequest, TResponse>(string path, TRequest payload, Action<ApiResult<TResponse>> onComplete)
        {
            string json = JsonUtility.ToJson(payload);
            byte[] body = Encoding.UTF8.GetBytes(json);
            using UnityWebRequest request = new UnityWebRequest(APIConfig.BaseUrl + path, UnityWebRequest.kHttpVerbPOST);
            request.uploadHandler = new UploadHandlerRaw(body);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.timeout = 15;
            request.SetRequestHeader("Accept", "application/json");
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();
            onComplete(ParseResponse<TResponse>(request));
        }

        private static ApiResult<T> ParseResponse<T>(UnityWebRequest request)
        {
            string body = request.downloadHandler != null ? request.downloadHandler.text : string.Empty;
            if (request.result != UnityWebRequest.Result.Success)
            {
                string message = string.IsNullOrWhiteSpace(body) ? request.error : body;
                return new ApiResult<T>(message);
            }

            try
            {
                T data = JsonUtility.FromJson<T>(body);
                return new ApiResult<T>(data);
            }
            catch (Exception exception)
            {
                return new ApiResult<T>($"Could not parse server response: {exception.Message}");
            }
        }
    }
}
