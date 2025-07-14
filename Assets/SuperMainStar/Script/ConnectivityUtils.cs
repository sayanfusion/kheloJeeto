using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public interface IConnectivityEvent
{
    void InternetStatus(bool a_Status);
    void ApplicationFocus(bool a_HasFocus);
    void ApplicationPause(bool a_PauseStatus);
}

public class ConnectivityUtils : MonoBehaviour
{
    public static ConnectivityUtils Instance = null;

    [SerializeField] private bool isPersistent = false;
    private bool isPaused = false;
    private List<IConnectivityEvent> iConnectivityEventSubscribers = new List<IConnectivityEvent>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            if (isPersistent)
                DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
#if UNITY_ANDROID && UNITY_IOS
        InvokeRepeating(nameof(checkInternetConnection), 0.5f, 0.5f);
#else
        InvokeRepeating(nameof(checkInternetConnectionForPC), 0.5f, 0.5f);
#endif
    }

    public void SubscribeToEvents(IConnectivityEvent a_Subscriber, bool a_Subscribe)
    {
        if (a_Subscribe)
        {
            if (!iConnectivityEventSubscribers.Contains(a_Subscriber))
                iConnectivityEventSubscribers.Add(a_Subscriber);
        }
        else
        {
            iConnectivityEventSubscribers.Remove(a_Subscriber);
        }
    }

    private void checkInternetConnection()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            iConnectivityEventSubscribers.ForEach(x => x.InternetStatus(false));
        }
        else
        {
            iConnectivityEventSubscribers.ForEach(x => x.InternetStatus(true));
        }
    }

    private void checkInternetConnectionForPC()
    {
        StartCoroutine(internetConnectionRoutine(isConnected =>
        {
            if (isConnected)
            {
                iConnectivityEventSubscribers.ForEach(x => x.InternetStatus(true));
            }
            else
            {
                iConnectivityEventSubscribers.ForEach(x => x.InternetStatus(false));
            }
        }));
    }

    private IEnumerator internetConnectionRoutine(Action<bool> a_HitCallback, string a_HitUrl = "https://google.com")
    {
        UnityWebRequest t_Request = new UnityWebRequest(a_HitUrl);
        yield return t_Request.SendWebRequest();
        if (t_Request.error != null)
        {
            a_HitCallback(false);
        }
        else
        {
            a_HitCallback(true);
        }
    }

    private void OnApplicationFocus(bool a_HasFocus)
    {
        iConnectivityEventSubscribers.ForEach(x => x.ApplicationFocus(a_HasFocus));
    }

    private void OnApplicationPause(bool a_PauseStatus)
    {
        iConnectivityEventSubscribers.ForEach(x => x.ApplicationPause(a_PauseStatus));
    }
}
