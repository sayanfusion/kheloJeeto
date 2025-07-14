using System.Collections.Generic;
using System;
using BestHTTP.SocketIO;
using Newtonsoft.Json;
using BestHTTP.SocketIO.Events;
using System.Linq;

namespace DevCommon
{
    public static class SocketListener
    {
        private static Dictionary<string, List<Action<string>>> listenerDatas = new Dictionary<string, List<Action<string>>>();

        public static void ActivateListener(ref SocketIOCallback a_OnListen)
        {
            a_OnListen += onListen;
        }

        public static void Listen(string a_EventName, Action<string> a_Callback)
        {
            if (!listenerDatas.TryGetValue(a_EventName, out var t_List))
            {
                t_List = new List<Action<string>>();
                listenerDatas.Add(a_EventName, t_List);
            }

            if (!t_List.Contains(a_Callback))
                listenerDatas[a_EventName].Add(a_Callback);
        }

        public static void UnSubscribe(string a_EventName, Action<string> a_Callback)
        {
            KeyValuePair<string, List<Action<string>>> t_Obj = listenerDatas.Where(x => string.Equals(x.Key, a_EventName)).FirstOrDefault();
            if (!t_Obj.Equals(default(KeyValuePair<string, List<Action<string>>>)))
            {
                List<Action<string>> t_Callbacks = t_Obj.Value;
                if (t_Callbacks.Any())
                {
                    Action<string> t_Callback = t_Callbacks.Where(x => Action.Equals(x, a_Callback)).FirstOrDefault();
                    if (t_Callback != null)
                    {
                        t_Callbacks.Remove(t_Callback);
                    }
                }
            }
        }

        private static void onListen(Socket a_Socket, Packet a_Packet, object[] a_Args)
        {
            if (listenerDatas.ContainsKey(a_Packet.EventName))
            {
                listenerDatas[a_Packet.EventName].ForEach(callback => callback?.Invoke(a_Packet.RemoveEventName(true)));
            }
        }
    }
}
