using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using khelojeetonew;
using Newtonsoft.Json;
using System;
using TMPro;
using JeetoJoker;
using UI.Dates;

namespace khelojeetonew
{

    public class UIReport : MonoBehaviour
    {
        [SerializeField] private Text txtDate;
        [SerializeField] private Text txtSalePoint;
        [SerializeField] private Text txtWinPoint;
        [SerializeField] private Text endPoint;
        [SerializeField] private Text txtCommiPoint;
        [SerializeField] private Text txtNtpPoint;
        [SerializeField] private TMP_InputField fromDate;
        [SerializeField] private TMP_InputField toDate;
        [SerializeField] private GameObject loadingPanel;
        [SerializeField] private DatePicker fromDatePicker; // DatePicker for "From Date"
        [SerializeField] private DatePicker toDatePicker;   // DatePicker for "To Date"
        private string _fromDate = ""; // Default "From Date"
        private string _toDate = "";   // Default "To Date"

        private void Start()
        {
            fromDatePicker.Config.Events.OnDaySelected.AddListener(OnFromDateSelected);
            toDatePicker.Config.Events.OnDaySelected.AddListener(OnToDateSelected);
        }

        private void OnEnable()
        {
            DownloadReport();
        }

        private void OnFromDateSelected(DateTime date)
        {
            _fromDate = date.ToString("yyyy-MM-dd");
            Debug.Log($"From Date selected: {_fromDate}");
        }

        private void OnToDateSelected(DateTime date)
        {
            _toDate = date.ToString("yyyy-MM-dd");
            Debug.Log($"To Date selected: {_toDate}");
        }

        public void DownloadReport()
        {
            StartCoroutine(Download($"{Constant.KIBaseURL}game-reports"));
        }

        private IEnumerator Download(string baseUrl)
        {
            string url = "";
            if (string.IsNullOrEmpty(_fromDate) || string.IsNullOrEmpty(_toDate))
            {
                Debug.LogError("No From Date and To Date provided!");
                url = $"{baseUrl}";
            }
            else
                url = $"{baseUrl}?start_date={_fromDate}&end_date={_toDate}&player_id={SocketController.Instance.joinRoomData.playerId}";

            loadingPanel.SetActive(true);
            yield return new WaitForSeconds(0.1f);

            UnityWebRequest unityWebRequest = UnityWebRequest.Get(url);
            Debug.Log($"url for report response: {url}");

            unityWebRequest.SetRequestHeader(Constants.authorization, "Bearer " + PlayerPrefs.GetString(Constants.token));
            yield return unityWebRequest.SendWebRequest();
            loadingPanel.SetActive(false);

            if (unityWebRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(unityWebRequest.error);
                // JeetoJokerManager.instance.ShowMessage(unityWebRequest.error);
                Debug.LogError(unityWebRequest.downloadHandler.text);
            }
            else
            {
                try
                {
                    #region DEBUGS
                    Debug.LogError(unityWebRequest.downloadHandler.text);
                    Debug.Log("report complete data response:" + unityWebRequest.downloadHandler.text);
                    ReportData reportData = JsonConvert.DeserializeObject<ReportData>(unityWebRequest.downloadHandler.text);
                    Debug.Log("txt sale point response::" + reportData.bet_amount.ToString());
                    Debug.Log("txt win point response:" + reportData.win_amount.ToString());
                    Debug.Log("txtcommipoint response:" + reportData.com_pt.ToString());
                    Debug.Log("txtNtpPoint response:" + reportData.NTP.ToString());
                    Debug.Log("txtDate response:" + toDate.text);
                    #endregion
                    if (reportData.status == 200)
                    {
                        txtSalePoint.text = reportData.bet_amount.ToString();
                        txtWinPoint.text = reportData.win_amount.ToString();
                        txtCommiPoint.text = reportData.com_pt.ToString();
                        txtNtpPoint.text = reportData.NTP.ToString();
                        //endPoint.text = reportData.end_point.ToString();
                        txtDate.text = toDate.text;
                    }
                    else
                    {
                        JeetoJokerManager.instance.ShowMessage("report data error status " + reportData.status);
                    }
                }
                catch (Exception e)
                {
                    JeetoJokerManager.instance.ShowMessage(e.ToString());
                }
            }
        }
    }
}
