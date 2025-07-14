using UnityEngine;

namespace DevCommon.Utils
{
    public enum ELogType
    {
        Assert,
        Error,
        Exception,
        Warning,
        System,
        Log,
        AI,
        Audio,
        Content,
        Logic,
        GUI,
        Input,
        Network,
        Physics
    }

    public static class CDebug
    {
        #region Assert
        public static void Assert(bool a_Condition) { UnityEngine.Debug.Assert(a_Condition); }
        public static void Assert(bool a_Condition, Object a_Context) { UnityEngine.Debug.Assert(a_Condition, a_Context); }
        public static void Assert(bool a_Condition, object a_Message) { UnityEngine.Debug.Assert(a_Condition, a_Message); }
        public static void Assert(bool a_Condition, object a_Message, Object a_Context) { UnityEngine.Debug.Assert(a_Condition, a_Message, a_Context); }
        public static void AssertFormat(bool a_Condition, string a_Format, params object[] a_Args) { UnityEngine.Debug.AssertFormat(a_Condition, a_Format, a_Args); }
        public static void AssertFormat(bool a_Condition, Object a_Context, string a_Format, params object[] a_Args) { UnityEngine.Debug.AssertFormat(a_Condition, a_Context, a_Format, a_Args); }
        public static void LogAssertion(object a_Message) { UnityEngine.Debug.LogAssertion(a_Message); }
        public static void LogAssertion(object a_Message, Object a_Context) { UnityEngine.Debug.LogAssertion(a_Message, a_Context); }
        public static void LogAssertionFormat(string a_Format, params object[] a_Args) { UnityEngine.Debug.LogAssertionFormat(a_Format, a_Args); }
        public static void LogAssertionFormat(Object a_Context, string a_Format, params object[] a_Args) { UnityEngine.Debug.LogAssertionFormat(a_Context, a_Format, a_Args); }
        #endregion

        #region Helper
        public static void Break() { UnityEngine.Debug.Break(); }
        public static void ClearDeveloperConsole() { UnityEngine.Debug.ClearDeveloperConsole(); }
        #endregion

        #region Draw
        public static void DrawRay(Vector3 a_Start, Vector3 a_Direction) { UnityEngine.Debug.DrawRay(a_Start, a_Direction); }
        public static void DrawRay(Vector3 a_Start, Vector3 a_Direction, Color a_Color) { UnityEngine.Debug.DrawRay(a_Start, a_Direction, a_Color); }
        public static void DrawRay(Vector3 a_Start, Vector3 a_Direction, Color a_Color, float a_Duration) { UnityEngine.Debug.DrawRay(a_Start, a_Direction, a_Color, a_Duration); }
        public static void DrawRay(Vector3 a_Start, Vector3 a_Direction, Color a_Color, float a_Duration, bool a_DepthTest) { UnityEngine.Debug.DrawRay(a_Start, a_Direction, a_Color, a_Duration, a_DepthTest); }
        public static void DrawLine(Vector3 a_Start, Vector3 a_End) { UnityEngine.Debug.DrawLine(a_Start, a_End); }
        public static void DrawLine(Vector3 a_Start, Vector3 a_End, Color a_Color) { UnityEngine.Debug.DrawLine(a_Start, a_End, a_Color); }
        public static void DrawLine(Vector3 a_Start, Vector3 a_End, Color a_Color, float a_Duration) { UnityEngine.Debug.DrawLine(a_Start, a_End, a_Color, a_Duration); }
        public static void DrawLine(Vector3 a_Start, Vector3 a_End, Color a_Color, float a_Duration, bool a_DepthTest) { UnityEngine.Debug.DrawLine(a_Start, a_End, a_Color, a_Duration, a_DepthTest); }
        #endregion

        #region Log
        public static void Log(object a_Message, ELogType a_Type = ELogType.Log) { UnityEngine.Debug.Log("[" + a_Type.ToString() + "] " + a_Message); }
        public static void Log(object a_Message, Object a_Context, ELogType a_Type = ELogType.Log) { UnityEngine.Debug.Log("[" + a_Type.ToString() + "] " + a_Message, a_Context); }
        public static void LogFormat(string a_Format, ELogType a_Type = ELogType.Log, params object[] a_Args) { UnityEngine.Debug.LogFormat("[" + a_Type.ToString() + "] " + a_Format, a_Args); }
        public static void LogFormat(Object a_Context, string a_Format, ELogType a_Type = ELogType.Log, params object[] a_Args) { UnityEngine.Debug.LogFormat(a_Context, "[" + a_Type.ToString() + "] " + a_Format, a_Args); }
        #endregion

        #region Error
        public static void LogError(object a_Message, ELogType a_Type = ELogType.Error) { UnityEngine.Debug.LogError("[" + a_Type.ToString() + "] " + a_Message); }
        public static void LogError(object a_Message, Object a_Context, ELogType a_Type = ELogType.Error) { UnityEngine.Debug.Log("[" + a_Type.ToString() + "] " + a_Message, a_Context); }
        public static void LogErrorFormat(string a_Format, params object[] a_Args) { UnityEngine.Debug.LogErrorFormat(a_Format, a_Args); }
        public static void LogErrorFormat(Object a_Context, string a_Format, params object[] a_Args) { UnityEngine.Debug.LogErrorFormat(a_Context, a_Format, a_Args); }
        #endregion

        #region Exception
        public static void LogException(System.Exception exception) { UnityEngine.Debug.LogException(exception); }
        public static void LogException(System.Exception exception, Object context) { UnityEngine.Debug.LogException(exception, context); }
        #endregion

        #region Warning
        public static void LogWarning(object a_Message, ELogType a_Type = ELogType.Warning) { UnityEngine.Debug.LogWarning("[" + a_Type.ToString() + "] " + a_Message); }
        public static void LogWarning(object a_Message, Object a_Context, ELogType a_Type = ELogType.Warning) { UnityEngine.Debug.LogWarning("[" + a_Type.ToString() + "] " + a_Message, a_Context); }
        #endregion
    }
}