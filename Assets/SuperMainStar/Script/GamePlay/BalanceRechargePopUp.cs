using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BalanceRechargePopUp : MonoBehaviour
{
    [SerializeField] private Button closeButton;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private TextMeshProUGUI newBalanceText;

    private void OnEnable()
    {
        closeButton.onClick.AddListener(OnClickCloseButton);
    }

    private void OnDisable()
    {
        closeButton.onClick.RemoveListener(OnClickCloseButton);
    }

    public void SetText(string balance,string updatedBalance)
    {
        messageText.text = "Your account has been updated with Point. " + balance;
        newBalanceText.text = "New balance Point. " + updatedBalance;
    }

    private void OnClickCloseButton()
    {
        Debug.LogError("Close Clicked");
        BalanceRecharge.Instance.DestroyPopUpImmdiately();
    }
}
