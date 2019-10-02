//This file was created by Zakir Chaudry on June 19th, 2019

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// A place to put the return to menu method
/// </summary>
public class ReturnToMenu : MonoBehaviour
{
    /// <summary>
    /// Loads the menu scene
    /// </summary>
    public void Menu()
    {
        SceneManager.LoadScene(0);
    }
}
