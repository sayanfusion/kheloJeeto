using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DevCommon
{
    [CreateAssetMenu(menuName = "Api Config")]
    public class Configuration : ScriptableObject
    {
        public ApiConfiguration Api = new ApiConfiguration();
        public SocketConfiguration SocketConfig = new SocketConfiguration();
        public CommonConfiguration Common = new CommonConfiguration();
        public PhotonConfiguration Photon = new PhotonConfiguration();

        [Serializable]
        public class ApiConfiguration
        {
            public string Host = "localhost";
            public string PersonalAccessClientId = "1";
            public string PersonalAccessClientSecret = "5WlJudWoF1C6EBntTJcawgl85UXSbjShz78iUH6b";
            public string PasswordGrantClientId = "2";
            public string PasswordGrantClientSecret = "LtGPZ3GtETGMwaCpGuntflrQ2oI5hop3y7rUMp3N";
        }

        [Serializable]
        public class SocketConfiguration
        {
            public string Host = "localhost";
            public List<string> SocketEvents = new List<string>();
        }

        [Serializable]
        public class CommonConfiguration
        {
            public string DefaultLocale = "en-US";
            public string DefaultLoadingPromptText = "loading_prompt";
        }

        [Serializable]
        public class PhotonConfiguration
        {
            public string Address = "188.117.216.243";
            public int Port = 15055;
        }
    }
}