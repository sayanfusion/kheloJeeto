using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackButtonHandler : MonoBehaviour
{
    [SerializeField] private GameObject leavePanel;
    void Update()
    {
        // Check for back button press on Android
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Handle back button press here
            Debug.Log("Back button pressed!");
            leavePanel.SetActive(true);
        }
    }
}
