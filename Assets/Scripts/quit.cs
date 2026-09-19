using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

// Quits the player when the user hits escape

public class Quit : MonoBehaviour
{
    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Application.Quit();
        }
    }
}