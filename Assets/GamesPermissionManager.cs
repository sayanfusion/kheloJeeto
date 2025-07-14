using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameMenuEntry
{
    public string gameName;
    public GameObject menuObject;
}

public class GamesPermissionManager : MonoBehaviour
{
    public List<GameMenuEntry> gameNameToMenuObject = new();
    private void Awake()
    {
        foreach (GameMenuEntry gameEntry in gameNameToMenuObject)
        {
            gameEntry.menuObject.SetActive(false);
        }
        ShowGames(UserInfoPersist.Instance.userInfo.game_permission);
    }

    public void ShowGames(string GameString)
    {
        string[] gameNames = GameString.Split(',');
        foreach (string gameName in gameNames)
        {
            GameMenuEntry GME = gameNameToMenuObject.Find(item => item.gameName == gameName);
            GME?.menuObject.SetActive(true);
        }
    }

}
