using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabParent : MonoBehaviour
{

  private void OnEnable()
  {


  }
  private void Start()
  {
    transform.GetChild(0).GetComponent<Tab>().OnClick(false);
  }

  public void ResetAllTabs()
  {
    StartCoroutine(ResetTabs());
  }

  private IEnumerator ResetTabs()
  {
    //Wait for other things to reset then reset this(cause the clients want's smething like this)
    yield return new WaitForSeconds(2.0f);
    for (int i = 0; i < transform.childCount; i++)
    {
      GameObject childGameObject = transform.GetChild(i).gameObject;
      if (childGameObject != null)
      {
        Tab tab = childGameObject.GetComponent<Tab>();
        if (tab != null)
        {
          tab.Reset();
        }
      }
    }
  }

  public void EnableATab(int _index)
  {
    transform.GetChild(_index).GetComponent<Tab>().OnClick(false);
  }
}
