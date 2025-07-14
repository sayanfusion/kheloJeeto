using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mousetest : BetSpace
{
    public static bool rightclick = false;

    //public ChipStack stack;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            rightclick = true;
            // Debug.Log("right click");
            // int selectedValue = ChipManager.GetSelectedValue();
            // RemoveBet(selectedValue);
            // ToolTipManager.SelectTarget(stack);
            // ToolTipManager.Deselect();
        }
    }
}
