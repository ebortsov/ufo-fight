using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;

public class AnalyticsTracker : MonoBehaviour
{
    private static AnalyticsTracker instance;

    private const string ConsentKey = "AnalyticsConsent";

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static void LogEvent(string eventName)
    {
        if (instance == null)
            return;

        if (!PlayerPrefs.HasKey(ConsentKey))
        {
            Debug.Log("Cannot send analytics event \"" + eventName + "\" (no consent key)");
            return;
        }

        if (PlayerPrefs.GetInt(ConsentKey) != 1)
        {
            Debug.Log("Cannot send analytics event \"" + eventName + "\" (consent is not given)");
            return;
        }

        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            Debug.Log("Cannot send analytics event \"" + eventName + "\" (unity services are not initialized)");
            return;
        }

        try
        {
            AnalyticsService.Instance.RecordEvent(eventName);
            AnalyticsService.Instance.Flush();

            Debug.Log("Analytics event sent: " + eventName);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Failed to send analytics event: " + eventName);
            Debug.LogWarning(e);
        }
    }
}