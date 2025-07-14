using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "APIData", menuName = "ScriptableObjects/APIData", order = 1)]
public class APIData : ScriptableObject
{
    public string gameName;
    public string loginApi;
    public string logOutApi;
    public string userDetailsApi;
    public string gameDataInsertApi;
    public string reportApi;
    public string historyApi;
    public string hotListApi;
    public string hotListGamewiseApi;
    public  string ipAndPortApi;
    public string winnerHotListUpdateApi;
    
}
