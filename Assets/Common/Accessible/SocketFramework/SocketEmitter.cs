using System.Collections.Generic;
using System;
using Newtonsoft.Json;
using BestHTTP.SocketIO;

namespace DevCommon
{
    public static class SocketEmitter
    {
        private static Dictionary<string, List<Action<string>>> ackListenerDatas = new Dictionary<string, List<Action<string>>>();

        public static void Emit<T>(string a_EventName, T a_Data, Action<string> a_AckCallback = null)
        {
            if (a_AckCallback == null)
            {
                SocketManager.Core.Socket.Emit(a_EventName, JsonConvert.SerializeObject(a_Data));
            }
            else
            {
                addListener(a_EventName, a_AckCallback);
                SocketManager.Core.Socket.Emit(a_EventName, onReceiveAckCallback, JsonConvert.SerializeObject(a_Data));
            }
        }

        private static void addListener(string a_EventName, Action<string> a_Callback)
        {
            if (!ackListenerDatas.TryGetValue(a_EventName, out var t_List))
            {
                t_List = new List<Action<string>>();
                ackListenerDatas.Add(a_EventName, t_List);
            }

            if (!t_List.Contains(a_Callback))
                ackListenerDatas[a_EventName].Add(a_Callback);
        }

        private static void onReceiveAckCallback(Socket a_Socket, Packet a_Packet, object[] a_Args)
        {
            if (ackListenerDatas.ContainsKey(a_Packet.EventName))
            {
                ackListenerDatas[a_Packet.EventName].ForEach(callback => callback?.Invoke(a_Packet.Payload));
            }
        }
    }
}
