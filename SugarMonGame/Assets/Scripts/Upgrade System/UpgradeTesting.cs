// This file was created by Zakir Chaudry on June 19th, 2019

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Extra methods for testing upgrade screens
/// </summary>
public class UpgradeTesting : MonoBehaviour
{
    /// <summary>
    /// Swapping screen bool
    /// </summary>
    bool swap;

    private void Start()
    {
        SwapScreen();
    }
    /// <summary>
    /// Swaps screen
    /// </summary>
    public void SwapScreen()
    {
        swap = !swap;
        this.transform.GetChild(0).gameObject.SetActive(swap);
        this.transform.GetChild(1).gameObject.SetActive(!swap);
    }
}
