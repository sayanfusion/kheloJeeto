using UnityEngine;

public class BalanceManager : MonoBehaviour {

    public static float Balance  = 0;
    public static float ConstBalance  = 0;


    public static void SetBalance(float balance)
    {
        ConstBalance=balance;
        Balance = balance;
        SceneRoulette.UpdateLocalPlayerText();
    }

    public static void ChangeBalance(float value)
    {
        Balance += value;
        SceneRoulette.UpdateLocalPlayerText();
    }

    public void ResetBalance(float balance)
    {
        Balance = balance;
        SceneRoulette.UpdateLocalPlayerText();
    }
}
//{ get; private set; }
