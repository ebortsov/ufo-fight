using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class DiagnosticsTestException : MonoBehaviour
{
    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.f9Key.wasPressedThisFrame)
        {
            Debug.LogException(
                new Exception("UFO Fight test exception for Unity Diagnostics")
            );
        }
    }
}