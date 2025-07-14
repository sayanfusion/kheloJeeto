using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DevCommon.Utils;

public class LogTester : MonoBehaviour
{
    private void Start()
    {
        CDebug.Log("DoSomething");
        CDebug.Log("Doing", ELogType.Logic);
        CDebug.LogError("Error");
        CDebug.LogWarning("Warning");
        throw new System.Exception("System Exception");
    }
}
