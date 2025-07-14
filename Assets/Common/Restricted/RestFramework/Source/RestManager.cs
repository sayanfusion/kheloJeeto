using System;
using DevCommon.Utils.Rest;
using DevCommon.Api;
using UnityEngine;
using RestUtil = DevCommon.Utils.RestUtil;
using RestError = DevCommon.Utils.RestUtil.RestCallError;
using UnityEngine.Networking;
using static DevCommon.Utils.Rest.WebRequestBuilder;

namespace DevCommon
{
    public class RestManager : Singleton<RestManager>
    {
        private RestUtil restUtil;

        private static string ClientAccessToken
        {
            get;
            set;
        }

        private static string UserAccessToken
        {
            get;
            set;
        }

        private static string UserRefreshToken
        {
            get;
            set;
        }

        protected override void Awake()
        {
            restUtil = RestUtil.Initialize(this);
        }

        /*public static void CreateAccount(UserInfo data, Action<Account> onCompletion,
            Action<RestError> onError)
        {
            var builder = new WebRequestBuilder()
                .Url(GetApiUrl(Urls.REGISTER))
                .Verb(Verbs.POST)
                .ContentType(ContentTypes.JSON)
                .FormData(Attributes.FIRST_NAME, data.firstName)
                .FormData(Attributes.LAST_NAME, data.lastName)
                .FormData(Attributes.USER_NAME, data.username)
                .FormData(Attributes.EMAIL_ID, data.email)
                .FormData(Attributes.PASSWORD, data.password)
                .FormData(Attributes.CONFIRM_PASSWORD, data.confirmPassword)
                .FormData(Attributes.LOCATION, "Kolkata");

            AddClientAuthHeader(ref builder);
            SendWebRequest(builder, onCompletion, onError);
        }*/

        private static void SendWebRequest(WebRequestBuilder builder, Action onCompletion, Action<RestUtil.RestCallError> onError)
        {
            Instance.restUtil.Send(builder, handler => { onCompletion?.Invoke(); },
                restError => InterceptError(restError, () => onError?.Invoke(restError), onError));
        }

        private static void SendWebRequest<T>(WebRequestBuilder builder, Action<T> onCompletion,
            Action<RestUtil.RestCallError> onError = null)
        {
            Instance.restUtil.Send(builder,
                handler =>
                {
                    var response = DataConverter.DeserializeObject<ApiResponseFormat<T>>(handler.text);
                    onCompletion?.Invoke(response.Result);
                },
                restError => InterceptError(restError, () => onError?.Invoke(restError), onError));
        }

        private static void InterceptError(RestUtil.RestCallError error, Action onSuccess,
            Action<RestUtil.RestCallError> defaultOnError)
        {
            defaultOnError?.Invoke(error);
        }

        private static string GetApiUrl(string path)
        {
            return $"{Config.Api.Host}{path}";
        }

        protected static string FormatApiUrl(string path, params object[] args)
        {
            return string.Format($"{Config.Api.Host}{path}", args);
        }

        private static void AddSecurityHeaders(ref WebRequestBuilder builder)
        {
            //builder.FormData("client_id", Config.ApiClientId)
            //    .FormData("client_secret", Config.ApiClientSecret);
        }

        private static void AddUserAuthHeader(ref WebRequestBuilder builder)
        {
            builder.Header("Authorization", $"Bearer {UserAccessToken}");
        }

        private static void AddClientAuthHeader(ref WebRequestBuilder builder)
        {
            builder.Header("Authorization", $"Bearer {ClientAccessToken}");
        }
    }
}