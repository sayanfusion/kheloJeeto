using System.Collections.Generic;
using UnityEngine;

namespace DevCommon
{
    public class Config : Singleton<Config>
    {
        #region Serialized Fields
#pragma warning disable 649
        [SerializeField]
        private Configuration configuration;
#pragma warning restore
        #endregion

        public class Api
        {
            public static string Host { get { return Instance.configuration.Api.Host; } }
            public static string PersonalAccessClientId { get { return Instance.configuration.Api.PersonalAccessClientId; } }
            public static string PersonalAccessClientSecret { get { return Instance.configuration.Api.PersonalAccessClientSecret; } }

            public static string PasswordGrantClientId { get { return Instance.configuration.Api.PasswordGrantClientId; } }
            public static string PasswordGrantClientSecret { get { return Instance.configuration.Api.PasswordGrantClientSecret; } }
        }

        public class SocketConfig
        {
            public static string Host { get { return Instance.configuration.SocketConfig.Host; } }
            public static List<string> SocketEvents { get { return Instance.configuration.SocketConfig.SocketEvents; } }
        }

        public class Common
        {
            public static string DefaultLoadingPromptText { get { return Instance.configuration.Common.DefaultLoadingPromptText; } }
            public static string DefaultLocale { get { return Instance.configuration.Common.DefaultLocale; } }

        }

        public class Photon
        {
            public static string Address { get { return Instance.configuration.Photon.Address; } }
            public static int Port { get { return Instance.configuration.Photon.Port; } }
        }
    }
}