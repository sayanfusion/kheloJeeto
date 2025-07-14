using UnityEngine;
using System;
using System.IO;

public static class HrtzzJsonUtilityHelper
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.gamedata;
    }

    public static string ToJson<T>(T[] array, string playerId, string gameId, string winNumber, string winAmmount,  string gameName, string winLoose, string startPoint, string winPosition/*, string _gamesession, string _deviceType*/)
    {
        Wrapper<T> wrapper = new Wrapper<T>
        {
            player_id = playerId,
            game_id = gameId,
            win_number = winNumber,
            win_ammount = winAmmount,
            game_name = gameName,
            win_loose = winLoose,
            start_point = startPoint,
            win_position = winPosition,
            //gamesession = _gamesession,
            //device_type = _deviceType,
            gamedata = array
        };
        return JsonUtility.ToJson(wrapper);
    }

    // Deserializng with formatting.
    public static string ToJson<T>(T[] array, string _gamesession, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>
        {
            //gamesession = _gamesession,
            gamedata = array
        };
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [Serializable]
    private class Wrapper<T>
    {
        public string player_id;
        public string game_id;
        public string win_number;
        public string win_ammount;
        public string game_name;
        public string win_loose;
        public string start_point;
        public string win_position;
        //public string single_win_number;
        //public string single_win_amount;
        //public string double_win_number;
        //public string double_win_amount;
        //public string gamesession;
        //public string device_type;
        public T[] gamedata;
    }

    #region IO Helper Functions
    // Gets the file path depending on platform.
    private static string GetPath()
    {
        string path = "";
        #if UNITY_EDITOR
        path = Application.dataPath + "/Resources/JsonFiles/";
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        #endif

        #if UNITY_IOS && !UNITY_EDITOR
        path = Application.persistentDataPath;
        #endif

        #if UNITY_ANDROID && !UNITY_EDITOR
        path = Application.persistentDataPath;
        #endif

        //Debug.Log("Path : " + path);
        return path + "/";
    }

    /// <summary>
    /// Saves a JSON string to Json File.
    /// </summary>
    /// <param name="fileName"> Name of File(without .json extension).</param>
    /// <param name="data"> string that needs to be saved</param>
    public static void SaveToJson(string fileName, string data)
    {
        File.WriteAllText(GetPath() + fileName + ".json", data);
        #if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
        #endif
    }

    public static string ReadFromJson(string fileName)
    {
        return File.ReadAllText(GetPath() + fileName + ".json");
    }
    #endregion
}
