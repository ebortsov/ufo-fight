using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System;
using System.Threading.Tasks;

public class ServicesInitializer : MonoBehaviour
{
    private async void Awake()
    {
        await InitializeServices();
    }

    private async Task InitializeServices()
    {
        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            await UnityServices.InitializeAsync();
        }

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            string profileName = "ufo_" + Guid.NewGuid().ToString("N").Substring(0, 8);

            AuthenticationService.Instance.SwitchProfile(profileName);

            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            Debug.Log("Signed in with profile: " + profileName);
            Debug.Log("Player ID: " + AuthenticationService.Instance.PlayerId);
        }
    }
}