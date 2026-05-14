using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class SentryTestException : MonoBehaviour
{
    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.f9Key.wasPressedThisFrame)
        {
            Debug.LogException(new Exception("UFO Fight test exception for Sentry"));
        }
    }
}