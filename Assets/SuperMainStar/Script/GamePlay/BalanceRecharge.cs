using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public interface IBalanceRecharge
{
    void UpdateBalance(float updatedBalance);
}

public class BalanceRecharge : MonoBehaviour
{
    private static BalanceRecharge instance;
    private GameObject balancePopUp;
    private Coroutine coroutine;
    private List<IBalanceRecharge> iBalanceContainer = new List<IBalanceRecharge>();
    [SerializeField] private GameObject balanceRechargePrefab;
    [SerializeField] private float destroyPopUpDelay;

    public static BalanceRecharge Instance => instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    public void ShowUpdatedBalance(string balance,string updatedBalance)
    {
        balancePopUp = Instantiate(balanceRechargePrefab);
        balancePopUp.transform.SetAsLastSibling();
        BalanceRechargePopUp balanceRechargePopUp = balancePopUp.GetComponent<BalanceRechargePopUp>();
        balanceRechargePopUp.SetText(balance, updatedBalance);
        iBalanceContainer.ForEach(x => x.UpdateBalance(float.Parse(balance)));
        DestroyPopUpAfterSomeTime(destroyPopUpDelay);
    }

    public void DestroyPopUpAfterSomeTime(float time)
    {
        if(coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        coroutine = StartCoroutine(DestroyBalancPopUp(time));
    }

    public void DestroyPopUpImmdiately()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        if (balancePopUp != null)
            Destroy(balancePopUp);
    }

    private IEnumerator DestroyBalancPopUp(float time)
    {
        yield return new WaitForSeconds(time);
        if (balancePopUp != null)
            Destroy(balancePopUp);
    }

    public void AddListener(IBalanceRecharge iBalanceRecharge)
    {
        if(!iBalanceContainer.Contains(iBalanceRecharge))
        {
            iBalanceContainer.Add(iBalanceRecharge);
        }
    }

    public void RemoveListener(IBalanceRecharge iBalanceRecharge)
    {
        if (iBalanceContainer.Contains(iBalanceRecharge))
        {
            iBalanceContainer.Remove(iBalanceRecharge);
        }
    }
}
