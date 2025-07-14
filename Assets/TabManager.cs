using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabManager : MonoBehaviour
{
    public GameObject[] Tabs;
    public Image[] TabButtons;

    private void Start()
    {
        SwitchTab(0);
    }

    public void SwitchTab(int TabID)
    {
        foreach (GameObject go in Tabs)
        {
            go.SetActive(false);
        }
        Tabs[TabID].SetActive(true);

        foreach (Image im in TabButtons)
        {
            im.color = Color.grey;
        }
        TabButtons[TabID].color = Color.white;
    }
}
