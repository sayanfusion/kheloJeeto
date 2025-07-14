
using UnityEngine;
using UnityEngine.UI;

public class VersionDisplayer : MonoBehaviour
{
    //private string version = "v 1.0.3";

    private void Start()
    {
        GetComponent<Text>().text = "v " + Application.version;
    }
}
