#define DEBUG_REST_CALLS

using DevCommon.Api;
using DevCommon.Utils.Rest;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace DevCommon.Utils
{
    public class RestUtil
    {
        private const long HttpOk = 200;
        private const long HttpCreated = 201;

        public UnityWebRequest CurrentRequest => _currentCall.Request;

        private bool Uploading => _currentCall.Request.method.Equals("POST");

        public float Progress
            => Uploading ? _currentCall.Request.uploadProgress : _currentCall.Request.downloadProgress;

        public ulong TransmittedBytes
            => Uploading ? _currentCall.Request.uploadedBytes : _currentCall.Request.downloadedBytes;

        private readonly MonoBehaviour _monoBehaviour;
        private readonly Queue<Call> _callQueue = new Queue<Call>();
        private int _callCounter;
        private Coroutine _coroutine;
        private Call _currentCall;
        private bool _endGracefully;

        private RestUtil(MonoBehaviour monoBehaviour, bool autoStart = true)
        {
            _monoBehaviour = monoBehaviour;

            if (autoStart)
                Start();
        }

        public static RestUtil Initialize(MonoBehaviour monoBehaviour)
        {
            return new RestUtil(monoBehaviour);
        }

        public void Start()
        {
            if (_coroutine == null)
                _coroutine = _monoBehaviour.StartCoroutine(Run());
        }

        /// <summary>
        /// The utility will stop sending requests when the current request has finished. 
        /// </summary>
        public void StopGracefully()
        {
            _endGracefully = true;
        }

        public void ForceStop()
        {
            if (_coroutine != null)
                _monoBehaviour.StopCoroutine(_coroutine);
        }

        /// <summary>
        /// Sends a web request over the network.
        /// </summary>
        /// <param name="builder">The builder that contains the web request data.</param>
        /// <param name="onCompletion">Function to be called when the request is completed successfully.</param>
        /// <param name="onError">Function to be called when the request fails.</param>
        /// <returns>An integer representing the number of the queued call.</returns>
        public int Send(WebRequestBuilder builder,
            Action<DownloadHandler> onCompletion, Action<RestCallError> onError)
        {
            _callQueue.Enqueue(new Call()
            {
                Builder = builder,
                OnCompletion = onCompletion,
                OnError = onError
            });

            return _callCounter++;
        }

        private IEnumerator Run()
        {
            do
            {
                do
                {
                    yield return new WaitForEndOfFrame();
                } while (_currentCall == null && _callQueue.Count == 0);

                _currentCall = _callQueue.Dequeue();
                _currentCall.Request = _currentCall.Builder.Build();
#if DEBUG_REST_CALLS
                Debug.LogFormat("Making {0} call to: {1}", _currentCall.Request.method, _currentCall.Request.url);
#endif
                yield return _currentCall.Request.SendWebRequest();

#if DEBUG_REST_CALLS
                Debug.LogFormat("Call {0} completed with status {1}", _currentCall.Request.url,
                    _currentCall.Request.responseCode);
#endif
                if (_currentCall.Request.responseCode == HttpOk || _currentCall.Request.responseCode == HttpCreated)
                {
                    _currentCall.OnCompletion(_currentCall.Request.downloadHandler);
                }
                else
                {
#if DEBUG_REST_CALLS
                    Debug.LogFormat("Called: {0}\nResponse: {1}", _currentCall.Request.url,
                        _currentCall.Request.downloadHandler.text);
#endif
                    var restCallError = new RestCallError()
                    {
                        Raw = _currentCall.Request.downloadHandler.text,
                        Code = _currentCall.Request.responseCode,
                        Headers = _currentCall.Request.GetResponseHeaders(),
                    };
                    var oauthResponse =
                        DataConverter.DeserializeObject<ApiResponseFormat<OauthErrorResponse>>(restCallError.Raw);
                    if (oauthResponse == null)
                    {
                        restCallError.Error = "no_connection";
                        restCallError.Description = "no_connection";
                    }
                    else if (oauthResponse.Result != null)
                    {
                        restCallError.Error = oauthResponse.Result.Error;
                        restCallError.Description = oauthResponse.Result.ErrorDescription;
                    }
                    else
                    {
                        var deSerializedData =
                            DataConverter.DeserializeObject<ApiResponseFormat<string>>(restCallError.Raw);
                        restCallError.Error = deSerializedData.Status.ToString();
                        restCallError.Description = deSerializedData.Message;
                    }

                    _currentCall.OnError(restCallError);
                }

                _currentCall.Request.Dispose();
                _currentCall = null;
            } while (!_endGracefully);
        }

        public struct RestCallError
        {
            public string Raw;
            public long Code;
            public string Error;
            public string Description;
            public Dictionary<string, string> Headers;
        }

        private class Call
        {
            public WebRequestBuilder Builder;
            public Action<DownloadHandler> OnCompletion;
            public Action<RestCallError> OnError;
            public UnityWebRequest Request;
        }
    }
}