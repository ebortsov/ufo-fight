using System.Collections;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;

public class AnalyticsConsentUI : MonoBehaviour
{
    [SerializeField] private GameObject consentPanel;

    private const string ConsentKey = "AnalyticsConsent";

    private void Start()
    {
        if (PlayerPrefs.HasKey(ConsentKey))
        {
            consentPanel.SetActive(false);

            if (PlayerPrefs.GetInt(ConsentKey) == 1)
            {
                StartCoroutine(StartAnalyticsWhenServicesReady());
            }

            return;
        }

        consentPanel.SetActive(true);
    }

    public void AcceptAnalytics()
    {
        PlayerPrefs.SetInt(ConsentKey, 1);
        PlayerPrefs.Save();

        consentPanel.SetActive(false);

        StartCoroutine(StartAnalyticsWhenServicesReady());
    }

    public void DeclineAnalytics()
    {
        PlayerPrefs.SetInt(ConsentKey, 0);
        PlayerPrefs.Save();

        consentPanel.SetActive(false);

        if (UnityServices.State == ServicesInitializationState.Initialized)
        {
            AnalyticsService.Instance.StopDataCollection();
        }

        Debug.Log("Analytics declined.");
    }

    private IEnumerator StartAnalyticsWhenServicesReady()
    {
        while (UnityServices.State != ServicesInitializationState.Initialized)
        {
            yield return null;
        }

        AnalyticsService.Instance.StartDataCollection();
        Debug.Log("Analytics consent accepted. Data collection started.");
    }
}