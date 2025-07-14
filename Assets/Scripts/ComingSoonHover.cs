using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComingSoonHover : MonoBehaviour
{
    public GameObject comingSoon;
    public void Hovertoggle()
    {
        comingSoon.SetActive(true);
    }
    public void ExitHover()
    {
        comingSoon.SetActive(false);
    }
}
