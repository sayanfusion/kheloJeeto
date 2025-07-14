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
        if (Tabs[TabID].activeSelf) return;
        Tabs[TabID].SetActive(true);
        for (int i = 0; i < Tabs.Length; i++)
        {
            if (TabID == i) continue;
            Tabs[i].SetActive(false);

        }
        foreach (Image im in TabButtons)
        {
            im.color = Color.grey;
        }
        TabButtons[TabID].color = Color.white;
    }
}
