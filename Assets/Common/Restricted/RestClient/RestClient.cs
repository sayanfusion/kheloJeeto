using System.Collections;
using System.Collections.Generic;
using BestHTTP;
using Newtonsoft.Json;
using System;
using System.Text;
using DevCommon.Utils.Rest;
using DevCommon.Api;
using UnityEngine;

namespace DevCommon
{
    public sealed class RestClient
    {
        public static void GetRequest<TResponse>(string a_URL, Action<TResponse> a_OnSuccess, Action<RestError> a_OnFailure, params KeyValuePair<string, string>[] a_ExtraHeaders)
        {
            HTTPRequest t_HTTPRequest = new HTTPRequest(new Uri(getApiUrl(a_URL)), HTTPMethods.Get);
            addHeaders(ref t_HTTPRequest, a_ExtraHeaders);
            sendHTTPRequest(t_HTTPRequest, a_OnSuccess, a_OnFailure);
        }

        public static void PostRequest<TRequest>(string a_URL, TRequest a_RequestData, bool a_SendRaw, Action a_OnSuccess, Action<RestError> a_OnFailure, params KeyValuePair<string, string>[] a_Headers)
        {
            HTTPRequest t_HTTPRequest = new HTTPRequest(new Uri(getApiUrl(a_URL)), HTTPMethods.Post);
            addHeaders(ref t_HTTPRequest, a_Headers);
            addDatas(ref t_HTTPRequest, a_RequestData, a_SendRaw);
            sendHTTPRequest(t_HTTPRequest, a_OnSuccess, a_OnFailure);
        }

        public static void PostRequest<TRequest, TResponse>(string a_URL, TRequest a_RequestData, bool a_SendRaw, Action<TResponse> a_OnSuccess, Action<RestError> a_OnFailure, params KeyValuePair<string, string>[] a_Headers)
        {
            HTTPRequest t_HTTPRequest = new HTTPRequest(new Uri(getApiUrl(a_URL)), HTTPMethods.Post);
            addHeaders(ref t_HTTPRequest, a_Headers);
            addDatas(ref t_HTTPRequest, a_RequestData, a_SendRaw);
            sendHTTPRequest(t_HTTPRequest, a_OnSuccess, a_OnFailure);
        }

        private static void addDatas<TRequest>(ref HTTPRequest a_HTTPRequest, TRequest a_RequestData, bool a_AddRaw)
        {
            if (a_AddRaw)
                a_HTTPRequest.RawData = Encoding.UTF8.GetBytes(DataConverter.SerializeObject((TRequest)a_RequestData));
            else
            {
                string t_JsonData = DataConverter.SerializeObject((TRequest)a_RequestData);
                Dictionary<string, string> t_Values = DataConverter.DeserializeObject<Dictionary<string, string>>(t_JsonData);
                foreach (KeyValuePair<string, string> element in t_Values)
                    a_HTTPRequest.AddField(element.Key, element.Value);
            }
        }

        private static void setContentType(ref HTTPRequest a_HTTPRequest, string a_TypeData)
        {
            if (!string.IsNullOrEmpty(a_TypeData) && !string.IsNullOrWhiteSpace(a_TypeData))
                a_HTTPRequest.AddHeader("Content-Type", a_TypeData);
        }

        private static void addHeaders(ref HTTPRequest a_HTTPRequest, KeyValuePair<string, string>[] a_Headers)
        {
            for (int i = 0; a_Headers != null && a_Headers.Length > 0 && i < a_Headers.Length; i++)
                a_HTTPRequest.AddHeader(a_Headers[i].Key, a_Headers[i].Value);
        }

        private static void sendHTTPRequest(HTTPRequest a_HTTPRequest, Action a_OnComplete, Action<RestError> a_OnFailure)
        {
            a_HTTPRequest.Callback = new OnRequestFinishedDelegate((request, response) =>
            {
                RestError t_ErrorResponse;
                if (evaluateRequestStatus(request, response, out t_ErrorResponse))
                    a_OnComplete?.Invoke();
                else
                    a_OnFailure?.Invoke(t_ErrorResponse);
            });
            a_HTTPRequest.Send();
        }

        private static void sendHTTPRequest<TResponse>(HTTPRequest a_HTTPRequest, Action<TResponse> a_OnComplete, Action<RestError> a_OnFailure)
        {
            a_HTTPRequest.Callback = new OnRequestFinishedDelegate((request, response) =>
            {
                RestError t_ErrorResponse;
                if (evaluateRequestStatus(request, response, out t_ErrorResponse))
                {
                    var t_DeserializedResponse = DataConverter.DeserializeObject<ApiResponseFormat<TResponse>>(response.DataAsText);
                    a_OnComplete?.Invoke(t_DeserializedResponse.Result);
                }
                else
                    a_OnFailure?.Invoke(t_ErrorResponse);
            });
            a_HTTPRequest.Send();
        }

        private static bool evaluateRequestStatus(HTTPRequest a_Request, HTTPResponse a_Response, out RestError a_ErrorResponse)
        {
            bool t_Status = false;
            a_ErrorResponse = new RestError() { RequestState = a_Request.State };
            switch (a_Request.State)
            {
                case HTTPRequestStates.Finished:
                    if (a_Response.IsSuccess)
                        t_Status = true;
                    else
                    {
                        a_ErrorResponse.Message = string.Format("Request finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2}", a_Response.StatusCode, a_Response.Message, a_Response.DataAsText);
                        t_Status = false;
                    }
                    break;

                case HTTPRequestStates.Error:
                    a_ErrorResponse.Message = string.Format("Request Finished with Error! " + (a_Request.Exception != null ? (a_Request.Exception.Message + "\n" + a_Request.Exception.StackTrace) : "No Exception"));
                    t_Status = false;
                    break;

                case HTTPRequestStates.Aborted:
                    a_ErrorResponse.Message = "Request Aborted!";
                    t_Status = false;
                    break;

                case HTTPRequestStates.ConnectionTimedOut:
                    a_ErrorResponse.Message = "Connection Timed Out!";
                    t_Status = false;
                    break;

                case HTTPRequestStates.TimedOut:
                    a_ErrorResponse.Message = "Processing the request Timed Out!";
                    t_Status = false;
                    break;
            }
            return t_Status;
        }

        private static string getApiUrl(string a_Path)
        {
            return $"{Config.Api.Host}{a_Path}";
        }
    }

    public class RestError
    {
        public HTTPRequestStates RequestState;
        public string Message;
    }

    public static class Pairing
    {
        public static KeyValuePair<string, string> Of(string a_Key, string a_Value)
        {
            return new KeyValuePair<string, string>(a_Key, a_Value);
        }
    }
}
