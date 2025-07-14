using UnityEngine;
using System.IO;
using System;
using UnityEditor;

namespace DevCommon.Utils
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    public sealed class DebugController : MonoBehaviour
    {
        private class DebugLogOutput
        {
            public string Type;
            public string Time;
            public string Log;
            public string Stack;
        }

        private enum EFileType
        {
            TXT,
            JSON,
        }

        [SerializeField] private string fileId = "SampleScene";
        [SerializeField] private EFileType fileType = EFileType.TXT;
        [SerializeField] private bool isPersistent = false;

        private string filePath;
        private int logCount = 0;

        private StreamWriter streamWriter;
        private DirectoryInfo directoryInfo;

        private void Awake()
        {
            if (isPersistent)
                DontDestroyOnLoad(this);
        }

        private void OnEnable()
        {
#if UNITY_EDITOR
            setFilePath();
            if (Application.isPlaying)
            {
                logCount = 0;
                streamWriter = new StreamWriter(filePath, false);
                streamWriter.AutoFlush = true;
                switch (fileType)
                {
                    case EFileType.JSON:
                        streamWriter.WriteLine("[");
                        break;
                }
                Application.logMessageReceived += handleLog;
            }
#endif
        }

        private void OnDisable()
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                Application.logMessageReceived -= handleLog;
                switch (fileType)
                {
                    case EFileType.JSON:
                        streamWriter.WriteLine("\n]");
                        break;
                }
                streamWriter.Close();
                AssetDatabase.Refresh();
            }
#endif
        }

        private void setFilePath()
        {
            var t_CurrentDirectory = Directory.GetCurrentDirectory();
#if UNITY_EDITOR_OSX
            directoryInfo = Directory.CreateDirectory(t_CurrentDirectory + @"/Assets/CDebugLogs/");
#else
            directoryInfo = Directory.CreateDirectory(t_CurrentDirectory + @"\Assets\CDebugLogs\");
#endif
            filePath = Path.Combine(directoryInfo.FullName, fileId + "." + DateTime.Now.ToString("yyyy.MM.dd.HH.mm.ss") + getFileExtension(fileType));
        }

        private string getFileExtension(EFileType a_Type)
        {
            switch (a_Type)
            {
                case EFileType.TXT: return ".txt";
                case EFileType.JSON: return ".json";
                default: return ".txt";
            }
        }

        private void handleLog(string a_LogString, string a_StackTrace, LogType a_LogType)
        {
            DebugLogOutput t_Output = new DebugLogOutput();
            if (a_LogType == LogType.Assert)
            {
                t_Output.Type = a_LogType.ToString();
                t_Output.Log = a_LogString;
            }
            else if (a_LogType == LogType.Exception)
            {
                t_Output.Type = a_LogType.ToString();
                t_Output.Log = a_LogString;
            }
            else
            {
                int end = a_LogString.IndexOf("]");
                t_Output.Type = a_LogString.Substring(1, end - 1);
                t_Output.Log = a_LogString.Substring(end + 2);
            }

            t_Output.Stack = a_StackTrace;
            t_Output.Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            switch (fileType)
            {
                case EFileType.TXT:
                    streamWriter.WriteLine("Type: " + t_Output.Type);
                    streamWriter.WriteLine("Time: " + t_Output.Time);
                    streamWriter.WriteLine("Log: " + t_Output.Log);
                    streamWriter.WriteLine("Stack: " + t_Output.Stack);
                    break;

                case EFileType.JSON:
                    streamWriter.Write((logCount == 0 ? "" : ",\n") + JsonUtility.ToJson(t_Output));
                    break;
            }

            logCount++;
        }

        public void ClearLogFiles()
        {
#if UNITY_EDITOR
            if (!string.IsNullOrEmpty(directoryInfo.FullName) && Directory.Exists(directoryInfo.FullName))
            {
                Directory.Delete(directoryInfo.FullName, true);
                AssetDatabase.Refresh();
            }
#endif
        }
    }
}
